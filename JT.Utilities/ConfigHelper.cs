using System;
using System.Configuration;

namespace JT.Infrastructure
{
    public static class ConfigHelper
    {
        /// <summary>
        /// Get the appSetting from config
        /// </summary>
        /// <param name="key">The keyName of the config</param>
        /// <param name="isThrowNullException">Throw exception if couldn't found the 
        /// config by the key if true, or it will return string.Empty as default</param>
        /// <returns></returns>
        public static string GetString(string key, bool isThrowNullException = false)
        {
            var value = string.Empty;
            value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrEmpty(value) && isThrowNullException)
            {
                throw new ConfigurationErrorsException(string.Format("Couldn't get the config by key: [{0}]", key));
            }

            return value;
        }

        /// <summary>
        /// Get the appSetting from config, and try parse it into int or just return 0 as default
        /// </summary>
        /// <param name="key">The keyName of the config</param>
        /// <param name="isThrowNullException">Throw exception if couldn't found the 
        /// config by the key if true, or it will return 0 as default</param>
        /// <returns></returns>
        public static int GetInt32(string key, bool isThrowNullException = false)
        {
            var value = GetString(key, isThrowNullException);

            int result = 0;
            int.TryParse(ConfigurationManager.AppSettings[key], out result);

            return result;
        }

        /// <summary>
        /// Get the appSetting from config, and try parse it into decimal or just return 0 as default
        /// </summary>
        /// <param name="key">The keyName of the config</param>
        /// <param name="isThrowNullException">Throw exception if couldn't found the 
        /// config by the key if true, or it will return 0 as default</param>
        /// <returns></returns>
        public static decimal GetDecimal(string key, bool isThrowNullException = false)
        {
            var value = GetString(key, isThrowNullException);

            decimal result = 0;
            decimal.TryParse(ConfigurationManager.AppSettings[key], out result);

            return result;
        }

        /// <summary>
        /// Get the appSetting from config, and try parse it into bool or just return false as default
        /// </summary>
        /// <param name="key">The keyName of the config</param>
        /// <param name="isThrowNullException">Throw exception if couldn't found the 
        /// config by the key if true, or it will return false as default</param>
        /// <remarks>It will return true when the config value is [true | yes | y | 1] (ignore case)</remarks>
        /// <returns></returns>
        public static bool GetBool(string key, bool isThrowNullException = false)
        {
            var value = GetString(key, isThrowNullException);

            bool result = false;
            bool.TryParse(ConfigurationManager.AppSettings[key], out result);

            if (string.Equals(value, "yes", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value, "y", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(value, "1", StringComparison.InvariantCultureIgnoreCase))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Get the connectionString from config
        /// </summary>
        /// <param name="name">The keyName of the config</param>
        /// <param name="isThrowNullException">Throw exception if couldn't found the 
        /// config by the key if true, or it will return string.Empty as default</param>
        /// <returns></returns>
        public static string GetConnectionString(string name, bool isThrowNullException = false)
        {
            var value = string.Empty;
            value = ConfigurationManager.ConnectionStrings[name].ConnectionString;

            if (string.IsNullOrEmpty(value) && isThrowNullException)
            {
                throw new ConfigurationErrorsException(string.Format("Couldn't get the config by name: {0}", name));
            }

            return value;
        }
    }
}
