using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace DeltekReminder.DesktopApp
{
    [Serializable]
    public class DeltekReminderSettings
    {
        private readonly object _lock = new object();
        private const string SETTINGS_CONFIG = @"Settings.config";

        public string Username { get; set; }
        
        public string Password { get; set; }        

        public string Domain { get; set; }
        
        public string BaseUrl { get; set; }

        private static string GetConfigFileName()
        {
            string path = GetDeltekReminderAppDataFolder();
            Directory.CreateDirectory(path);
            return Path.Combine(path, SETTINGS_CONFIG);
        }

        private static string GetDeltekReminderAppDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Deltek Reminder");
        }

        public static DeltekReminderSettings GetSettings()
        {
            string fileName = GetConfigFileName();
            return GetSettings(fileName);
        }

        private static DeltekReminderSettings GetSettings(string fileName)
        {
            FileStream myFileStream = null;
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(DeltekReminderSettings));
                FileInfo fi = new FileInfo(fileName);
                if (fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    DeltekReminderSettings settings = (DeltekReminderSettings)mySerializer.Deserialize(myFileStream);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                var dialogResult = MessageBox.Show("There was an error deserializing the settings file.  Click OK to revert the file and start over or cancel to close the app and fix the problem yourself.  Here's the error: " + ex, "Drat!", MessageBoxButton.OKCancel);
                if (dialogResult == MessageBoxResult.Cancel)
                {
                    Application.Current.Shutdown();
                    return null;
                }
            }
            finally
            {
                if (myFileStream != null)
                {
                    myFileStream.Close();
                }
            }
            DeltekReminderSettings defaultSettings = GetDefaultSettings();
            defaultSettings.Save();
            return defaultSettings;
        }

        private static DeltekReminderSettings GetDefaultSettings()
        {
            return new DeltekReminderSettings();
        }

        public virtual void Save()
        {
            string fileName = GetConfigFileName();
            Save(fileName);
        }

        private void Save(string fileName)
        {
            lock (_lock)
            {
                StreamWriter myWriter = null;
                try
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(DeltekReminderSettings));
                    myWriter = new StreamWriter(fileName, false);
                    mySerializer.Serialize(myWriter, this);
                }
                finally
                {
                    if (myWriter != null)
                    {
                        myWriter.Close();
                    }
                }
            }
        }
    }
}
