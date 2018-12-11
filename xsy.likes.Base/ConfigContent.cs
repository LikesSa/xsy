using System.Configuration;

namespace xsy.likes.Base
{
    public class ConfigAppSetting
    {

        public static string AppSettingsGet(string key)
        {
            string result = string.Empty;
            try
            {
                result = ConfigurationManager.AppSettings.Get(key);
            }
            catch { }
            return result;
        }

        public static void AppSettingsSet(string key, string value)
        {
            var configure = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configure.AppSettings.Settings[key] != null)
                configure.AppSettings.Settings[key].Value = value;
            else
                configure.AppSettings.Settings.Add(key, value);

            configure.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSetting");
            ConfigurationManager.AppSettings[key] = value;
        }
    }
}
