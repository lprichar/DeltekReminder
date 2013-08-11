using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace DeltekReminder.Lib
{
    [Serializable]
    public class DeltekReminderSettings
    {
        public DeltekReminderSettings()
        {
            CheckTime = "5:00 PM";
        }
        
        private readonly object _lock = new object();
        private const string SETTINGS_CONFIG = @"Settings.config";

        public string Username { get; set; }
        
        public string EncryptedPassword { get; set; }        

        public string Domain { get; set; }
        
        public string BaseUrl { get; set; }

        public DateTime? LastSuccessfulLogin { get; set; }

        [XmlIgnore]
        public string Password
        {
            get { return new TripleDesStringEncryptor().DecryptString(EncryptedPassword); }
            set { EncryptedPassword = new TripleDesStringEncryptor().EncryptString(value); }
        }

        public string CheckTime { get; set; }

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
                XmlSerializer mySerializer = new XmlSerializer(typeof (DeltekReminderSettings));
                FileInfo fi = new FileInfo(fileName);
                if (fi.Exists)
                {
                    myFileStream = fi.OpenRead();
                    DeltekReminderSettings settings = (DeltekReminderSettings) mySerializer.Deserialize(myFileStream);
                    return settings;
                }
            }
            catch (Exception)
            {
                throw new SerializationException();
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

        public string GetLastStatusAsText()
        {
            if (LastSuccessfulLogin == null) return null;
            return LastSuccessfulLogin.Value.ToLongDateString() + " " + LastSuccessfulLogin.Value.ToLongTimeString();
        }

        public void SetCheckTime(string value)
        {
            if (CheckTime != value)
            {
                CheckTime = value;
                Save();
            }
        }

        public DateTime GetCheckTimeForToday(DeltekReminderContext ctx)
        {
            var checkTime = DateTime.Parse(ctx.Settings.CheckTime);
            var todayAtCheckTime = new DateTime(ctx.Now.Year, ctx.Now.Month, ctx.Now.Day, checkTime.Hour, checkTime.Minute, 0);
            return todayAtCheckTime;
        }
    }
}
