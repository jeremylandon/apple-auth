using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Colorful;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Console = Colorful.Console;

namespace Golap.AppleAuth.Sample
{
    class Program
    {
        private static AppleVerifier _verifier;
        private static AppSettings _settings;
        private static AppleAuthClient _client;
        static async Task Main()
        {
            #region Check config

            var p8Path = "Files/key.p8";
            if (!File.Exists(p8Path))
            {
                WriteError("Add your key in the 'Files' folder (https://developer.apple.com/account/resources/authkeys)");
                return;
            }

            var appSettingsPath = "appsettings.json";
            if (!File.Exists(appSettingsPath))
            {
                WriteError("Create your 'appsettings.json' file from 'appsettings.template.json'");
                return;
            }

            #endregion

            ConfigureSettings(appSettingsPath);

            // create clients
            _verifier = new AppleVerifier();
            _client = await CreateAppleClient(p8Path);

            // create login url to receive the code and id_token
            var appleCode = GetAppleCode();

            // generate a token with an apple code
            var accessToken = await GetAccessTokenAsync(appleCode);

            // an apple code cannot be reuse
            await CheckAppleCodeReusabilityAsync(appleCode);

            // refresh a token with a refresh token
            await RefreshTokenAsync(accessToken.RefreshToken);
        }

        private static async Task<AppleAuthClient> CreateAppleClient(string p8Path)
        {
            var p8 = await File.ReadAllTextAsync(p8Path);
            var authSetting = new AppleAuthSetting(_settings.TeamId, _settings.ClientId, _settings.RedirectUri);
            var keySetting = new AppleKeySetting(_settings.KeyId, p8);
            return new AppleAuthClient(authSetting, keySetting);
        }

        private static string GetAppleCode()
        {
            WriteTitle("Get Apple Code");

            var loginUri = _client.GetLoginUri();

            WriteInfo("1 - Go to {0}\n2 - Login\n3 - Enter the apple code received on your callback:", loginUri);
            return Console.ReadLine();
        }

        private static async Task<AppleAccessToken> GetAccessTokenAsync(string appleCode)
        {
            WriteTitle("Get AccessToken");

            var accessToken = await _client.GetAccessTokenAsync(appleCode);
            var token = await _verifier.ValidateAsync(accessToken.IdToken, _settings.ClientId);

            WriteInfo("accessToken => {0}\ntoken => {1}", JsonConvert.SerializeObject(accessToken), token);
            return accessToken;
        }

        private static async Task CheckAppleCodeReusabilityAsync(string appleCode)
        {
            Console.WriteLine("Call AccessTokenAsync method with an already used apple code throw an exception", Color.Cyan);
            try
            {
                await _client.GetAccessTokenAsync(appleCode);
            }
            catch (AppleAuthException err)
            {
                WriteInfo("exception => {0}", err);
            }
        }

        private static async Task RefreshTokenAsync(string refreshToken)
        {
            WriteTitle("Refresh Token");

            var refreshedAccessToken = await _client.RefreshTokenAsync(refreshToken);
            var token = await _verifier.ValidateAsync(refreshedAccessToken.IdToken, _settings.ClientId);

            WriteInfo("refreshed accessToken => {0}\nrefreshed token => {1}", JsonConvert.SerializeObject(refreshedAccessToken), token);
        }

        #region Internal helper

        private static void ConfigureSettings(string appSettingsPath)
        {
            _settings = new AppSettings();
            new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath)
                .AddEnvironmentVariables()
                .Build().Bind(_settings);
        }

        private static void WriteError(string str)
        {
            Console.WriteLine(str, Color.Red);
        }

        private static void WriteTitle(string str)
        {
            Console.WriteLine(str, Color.Cyan);
        }

        private static void WriteInfo(string template, params object[] values)
        {
            Console.WriteLineFormatted(template, Color.White, values.Select(e => new Formatter(e, Color.CornflowerBlue)).ToArray());
        }

        #endregion
    }
}
