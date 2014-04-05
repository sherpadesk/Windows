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
        private const string API_TOKEN_SETTING = "ApiTokenSettings";
        private const string USER_ID_SETTING = "UserIdSettings";
        private const string EMAIL_SETTING = "EmailSettings";
        private const string FIRST_NAME_SETTING = "FirstNameSettings";
        private const string LAST_NAME_SETTING = "LastNameSettings";
        private const string ROLE_SETTING = "RoleSettings";
        private const string ORGANIZATION_KEY_SETTING = "OrganizationKeySettings";
        private const string ORGANIZATION_NAME_SETTING = "OrganizationNameSettings";
        private const string INSTANCE_KEY_SETTING = "InstanceKeySettings";
        private const string INSTANCE_NAME_SETTING = "InstanceNameSettings";
        private const string SINGLE_SETTING = "SingleSettings";
        private const string BETA_SETTING = "BetaSettings";

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

        public int UserId
        {
            get
            {
                return GetValueOrDefault<int>(USER_ID_SETTING, 0);
            }
            set
            {
                AddOrUpdateValue(USER_ID_SETTING, value);
                Save();
            }
        }

        public string FirstName
        {
            get
            {
                return GetValueOrDefault<string>(FIRST_NAME_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(FIRST_NAME_SETTING, value);
                Save();
            }
        }

        public string LastName
        {
            get
            {
                return GetValueOrDefault<string>(LAST_NAME_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(LAST_NAME_SETTING, value);
                Save();
            }
        }

        public string Email
        {
            get
            {
                return GetValueOrDefault<string>(EMAIL_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(EMAIL_SETTING, value);
                Save();
            }
        }

        public string Role
        {
            get
            {
                return GetValueOrDefault<string>(ROLE_SETTING, string.Empty);
            }
            set
            {
                AddOrUpdateValue(ROLE_SETTING, value);
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

        public void AddToken(string token, string email)
        {
            this.AddOrUpdateValue(API_TOKEN_SETTING, token);
            this.AddOrUpdateValue(EMAIL_SETTING, email);
            this.Save();
        }

        public void AddOrganization(string orgKey, string orgName, string instKey, string instName, bool single)
        {
            this.AddOrUpdateValue(ORGANIZATION_KEY_SETTING, orgKey);
            this.AddOrUpdateValue(ORGANIZATION_NAME_SETTING, orgName);
            this.AddOrUpdateValue(INSTANCE_KEY_SETTING, instKey);
            this.AddOrUpdateValue(INSTANCE_NAME_SETTING, instName);
            this.AddOrUpdateValue(SINGLE_SETTING, single);
            this.Save();
        }

        public void AddUser(int userId, string firstName, string lastName, string role)
        {
            this.AddOrUpdateValue(USER_ID_SETTING, userId);
            this.AddOrUpdateValue(FIRST_NAME_SETTING, firstName);
            this.AddOrUpdateValue(LAST_NAME_SETTING, lastName);
            this.AddOrUpdateValue(ROLE_SETTING, role);
            this.Save();
        }
    }
}
