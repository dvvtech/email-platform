namespace Email.Api.Configuration
{
    public class GoogleRecaptchaConfig
    {
        public const string SectionName = "GoogleRecaptcha";

        public string SecretKeyForOxfordAp { get; init; }

        public string SecretKeyForYashelCenter { get; init; }

        public string SecretKeyForPingmetasks { get; init; }
    }
}
