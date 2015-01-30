using SherpaDesk.Models;
using SherpaDesk.Models.Response;
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
        private const string API_TOKEN_SETTING = "ApiTokenSettings";
        //private const string USER_ID_SETTING = "UserIdSettings";
        //private const string EMAIL_SETTING = "EmailSettings";
        //private const string FIRST_NAME_SETTING = "FirstNameSettings";
        //private const string LAST_NAME_SETTING = "LastNameSettings";
        //private const string ROLE_SETTING = "RoleSettings";
        private const string CONFIG_SETTING = "ConfigSettings";
        private const string ORGANIZATION_KEY_SETTING = "OrganizationKeySettings";
        private const string ORGANIZATION_NAME_SETTING = "OrganizationNameSettings";
        private const string INSTANCE_KEY_SETTING = "InstanceKeySettings";
        private const string INSTANCE_NAME_SETTING = "InstanceNameSettings";
        private const string SINGLE_SETTING = "SingleSettings";
        private const string BETA_SETTING = "BetaSettings";
        private const string DEFAULT_TASK_TYPE_SETTING = "DefaultTaskTypeSettings";
        private const string SUPPORT_EMAIL_SETTINGS = "microsoft@sherpadesk.com";

        public string SupportEmail
        {
            get
            {
                return SUPPORT_EMAIL_SETTINGS;
            }
        }

        public string ApiToken
        {
            get
            {
                return GetValueOrDefault<string>(API_TOKEN_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(API_TOKEN_SETTING, value);
                Save();
            }
        }

        private ConfigResponse _configuration = null;

        public ConfigResponse Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var stringSettings = GetValueOrDefault<string>(CONFIG_SETTING, null);

                    return !string.IsNullOrEmpty(stringSettings)
                         ? Helper.FromXml<ConfigResponse>(stringSettings)
                         : new ConfigResponse();
                }
                return _configuration;
            }
            set
            {
                _configuration = value;

                AddOrUpdateValue(CONFIG_SETTING, Helper.ToXML(value));

                Save();
            }
        }

        public string OrganizationKey
        {
            get
            {
                return GetValueOrDefault<string>(ORGANIZATION_KEY_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(ORGANIZATION_KEY_SETTING, value);
                Save();
            }
        }

        public string OrganizationName
        {
            get
            {
                return GetValueOrDefault<string>(ORGANIZATION_NAME_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(ORGANIZATION_NAME_SETTING, value);
                Save();
            }
        }

        public string InstanceKey
        {
            get
            {
                return GetValueOrDefault<string>(INSTANCE_KEY_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(INSTANCE_KEY_SETTING, value);
                Save();
            }
        }

        public string InstanceName
        {
            get
            {
                return GetValueOrDefault<string>(INSTANCE_NAME_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(INSTANCE_NAME_SETTING, value);
                Save();
            }
        }

        public bool Single
        {
            get
            {
                return GetValueOrDefault<bool>(SINGLE_SETTING, true);
            }
            set
            {
                AddOrUpdateValue(SINGLE_SETTING, value);
                Save();
            }
        }

        public int DefaultTaskType
        {
            get
            {
                return GetValueOrDefault<int>(DEFAULT_TASK_TYPE_SETTING, 0);
            }
            set
            {
                AddOrUpdateValue(DEFAULT_TASK_TYPE_SETTING, value);
                Save();
            }
        }
        public bool Beta
        {
            get
            {
                return GetValueOrDefault<bool>(BETA_SETTING, false);
            }
            set
            {
                AddOrUpdateValue(BETA_SETTING, value);
                Save();
            }
        }

        // Our isolated storage settings
        //IsolatedStorageSettings isolatedStore;
        ApplicationDataContainer localSettings;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public AppSettings()
        {
            try
            {
                localSettings = ApplicationData.Current.LocalSettings; //RoamingSettings;
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
        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (localSettings.Values.ContainsKey(Key))
            {
                value = (T)localSettings.Values[Key];
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void Save()
        {
            //isolatedStore.Save();
        }

        public void Clear()
        {
            localSettings.Values.Clear();
        }

        public void AddToken(string token)
        {
            this.AddOrUpdateValue(API_TOKEN_SETTING, token);
            this.Save();
        }

        public void AddOrganization(string orgKey, string orgName, string instanceKey, string instanceName, bool single)
        {
            this.AddOrUpdateValue(ORGANIZATION_KEY_SETTING, orgKey);
            this.AddOrUpdateValue(ORGANIZATION_NAME_SETTING, orgName);
            this.AddOrUpdateValue(INSTANCE_KEY_SETTING, instanceKey);
            this.AddOrUpdateValue(INSTANCE_NAME_SETTING, instanceName);
            this.AddOrUpdateValue(SINGLE_SETTING, single);
            this.Save();
        }

        public void AddConfiguration(ConfigResponse config)
        {
            this.AddOrUpdateValue(CONFIG_SETTING, config);
            this.Save();
        }
    }
}
