using Email.Api.AppStart;
using Email.Api.AppStart.Extensions;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder);
startup.Initialize();

builder.Services.AddControllers();

var app = builder.Build();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
app.Logger.LogInformation(environment ?? "Empty environment");
// Configure the HTTP request pipeline.

if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.ApplyCors();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
