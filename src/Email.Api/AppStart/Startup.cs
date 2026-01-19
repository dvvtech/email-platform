using Email.Api.Configuration;

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

            // 1. Регистрация HttpClientFactory
            _builder.Services.AddHttpClient();

            InitConfigs();
            AddEmailServices();

            _builder.Services.AddControllers();
        }

        private void InitConfigs()
        {
            _builder.Services.AddOptions<SmtpConfig>()
                    .Bind(_builder.Configuration.GetSection(SmtpConfig.SectionName))
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

            //_builder.Services.Configure<GoogleRecaptchaConfig>(_builder.Configuration.GetSection(GoogleRecaptchaConfig.SectionName));
        }

        private void AddEmailServices()
        {
            //_builder.Services.AddScoped<IEmailBodyGenerator, EmailBodyGenerator>();

            //_builder.Services.AddSingleton<IEmailSender>(provider =>
            //{
            //    var smtpConfig = _builder.Configuration.GetSection(SmtpConfig.SectionName).Get<SmtpConfig>();

            //    var configuration = provider.GetRequiredService<IConfiguration>();
            //    var logger = provider.GetRequiredService<ILogger<EmailSender>>();

            //    return new EmailSender(
            //        smtpConfig.Host,
            //        smtpConfig.Port,
            //        smtpConfig.Username,
            //        smtpConfig.Password,
            //        logger
            //    );
            //});
        }
    }
}
