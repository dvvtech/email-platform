using Email.Api.AppStart.Extensions;
using Email.Api.BLL.Abstract;
using Email.Api.BLL.Services;
using Email.Api.BLL.Services.MppTests;
using Email.Api.BLL.Services.Sites;
using Email.Api.Configuration;
using Email.Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using System.Threading.RateLimiting;

namespace Email.Api.AppStart
{
    public class Startup
    {
        private readonly WebApplicationBuilder _builder;

        public Startup(WebApplicationBuilder builder)
        {
            _builder = builder;
        }

        public void Initialize()
        {
            if (_builder.Environment.IsDevelopment())
            {
                _builder.Services.AddSwaggerGen();
            }
            else
            {
                _builder.Services.ConfigureCors();
            }

            // Регистрация HttpClientFactory
            _builder.Services.AddHttpClient();

            InitConfigs();
            RegisterValidators();
            AddServices();
            ConfigureRateLimiting();

            _builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            if (!_builder.Environment.IsDevelopment())
            {
                _builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
            }

            _builder.Services.AddOptions<SmtpConfig>()
                    .Bind(_builder.Configuration.GetSection(SmtpConfig.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            _builder.Services.Configure<GoogleRecaptchaConfig>(_builder.Configuration.GetSection(GoogleRecaptchaConfig.SectionName));


            //var logger = _builder.Services.BuildServiceProvider().GetService<ILogger<Startup>>();
            //var smtpConfig = _builder.Configuration.GetSection(SmtpConfig.SectionName).Get<SmtpConfig>();
            //logger.LogInformation("host:" + smtpConfig.Host);
            //logger.LogInformation("port:" + smtpConfig.Port);
            //logger.LogInformation("username:" + smtpConfig.Username);
            //logger.LogInformation("pswd:" + smtpConfig.Password);

            //var config = _builder.Configuration.GetSection(GoogleRecaptchaConfig.SectionName).Get<GoogleRecaptchaConfig>();
            //logger.LogInformation("s1:" + config.SecretKeyForPingmetasks);
            //logger.LogInformation("s2:" + config.SecretKeyForYashelCenter);
            //logger.LogInformation("s3:" + config.SecretKeyForOxfordAp);
        }

        private void RegisterValidators()
        {
            // Регистрируем все валидаторы из сборки, где находится EmailRequestValidator
            _builder.Services.AddValidatorsFromAssemblyContaining<EmailRequestValidator>();
        }

        private void AddServices()
        {
            _builder.Services.AddScoped<IAnalyticsTrackingService, AnalyticsTrackingService>();

            // Configure file upload limits
            _builder.Services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue;
                options.MemoryBufferThreshold = int.MaxValue;
            });

            _builder.Services.AddScoped<IEmailBodyGenerator, EmailBodyGenerator>();

            _builder.Services.AddSingleton<IEmailSender>(provider =>
            {
                var smtpConfig = _builder.Configuration.GetSection(SmtpConfig.SectionName).Get<SmtpConfig>();

                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<EmailSender>>();

                return new EmailSender(
                    smtpConfig.Host,
                    smtpConfig.Port,
                    smtpConfig.Username,
                    smtpConfig.Password,
                    logger
                );
            });
        }

        private void ConfigureRateLimiting()
        {
            _builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        Success = false,
                        ErrorMessage = "Превышено количество запросов. Попробуйте позже."
                    }, cancellationToken);
                };

                options.AddPolicy("EmailRequests", httpContext =>
                {
                    var clientIp = httpContext.GetRealClientIp();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: clientIp,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                });
            });
        }
    }
}
