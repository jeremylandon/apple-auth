using System;
using Microsoft.Extensions.Configuration;

namespace Golap.AppleAuth.Tests.Core
{
    public static class TestSettingsHelper
    {
        public static T GetTestSettings<T>(T testSettings = null, string configFileName = "testsettings.json") where T : class
        {
            testSettings ??= (T) Activator.CreateInstance(typeof(T));
            new ConfigurationBuilder().AddJsonFile(configFileName).Build().Bind(testSettings, options => options.BindNonPublicProperties = true);
            return testSettings;
        }
    }
}
