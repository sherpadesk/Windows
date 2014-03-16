using SherpaDesk.Models;
using System;
using System.Diagnostics;
using Windows.Storage;

namespace SherpaDesk.Common
{
    public class AppSettings
    {
        private static AppSettings _appSettings = null;

        public static AppSettings Current
        {
            get
            {
                if (_appSettings == null)
                {
                    _appSettings = new AppSettings();
                }
                return _appSettings;
            }
        }
        private const string KEY = "appSettings";
        private const string API_TOKEN_KEY = "ApiTokenSettings";
        private const string USERNAME_KEY = "UsernameSettings";

        // Our isolated storage settings
        //IsolatedStorageSettings isolatedStore;
        ApplicationDataContainer localSettings;

        public AppSettings()
        {
            try
            {
                localSettings = ApplicationData.Current.LocalSettings;
                // Get the settings for this application.
                //isolatedStore = IsolatedStorageSettings.ApplicationSettings;

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Update a setting value for our application. If the setting does not
        /// exist, then add the setting.
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            //if (localSettings.Contains(Key))
            if (localSettings.Values.ContainsKey(Key))
            {
                // If the value has changed
                if (localSettings.Values[Key] != value)
                {
                    // Store the new value
                    localSettings.Values[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                localSettings.Values.Add(Key, value);
                valueChanged = true;
            }

            return valueChanged;
        }


        /// <summary>
        /// Get the current value of the setting, or if it is not found, set the 
        /// setting to the default setting.
        /// </summary>
        /// <typeparam name="valueType"></typeparam>
        /// <param name="Key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public valueType GetValueOrDefault<valueType>(string Key, valueType defaultValue)
        {
            valueType value;

            // If the key exists, retrieve the value.
            if (localSettings.Values.ContainsKey(Key))
            {
                value = (valueType)localSettings.Values[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }

            return value;
        }


        /// <summary>
        /// Save the settings.
        /// </summary>
        public void Save()
        {
            //isolatedStore.Save();

        }

        public void Clear()
        {
            localSettings.Values.Clear();
        }

        /// <summary>
        /// Property to get and set a ApiToken Setting Key.
        /// </summary>
        public string ApiToken
        {
            get
            {
                return GetValueOrDefault<string>(API_TOKEN_KEY, string.Empty);
            }
            set
            {
                AddOrUpdateValue(API_TOKEN_KEY, value);
                Save();
            }
        }

        /// <summary>
        /// Property to get and set a ApiToken Setting Key.
        /// </summary>
        public string Username
        {
            get
            {
                return GetValueOrDefault<string>(USERNAME_KEY, string.Empty);
            }
            set
            {
                AddOrUpdateValue(USERNAME_KEY, value);
                Save();
            }
        }

    }
}
