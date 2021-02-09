namespace Golap.AppleAuth.Tests
{
    public class TestSettings
    {
        public JwtValidation JwtValidation { get; set; }
    }

    public class JwtValidation
    {
        public string Audience { get; set; }
        public string JsonKey { get; set; }
        public string ValidJwtToken { get; set; }
        public string ValidAppleResponse { get; set; }
        public string InvalidJwtToken { get; set; }
    }
}
