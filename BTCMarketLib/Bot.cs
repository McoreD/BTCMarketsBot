using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTCMarketsBot
{
    public static class Bot
    {
        public static readonly string PersonalFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BTCMarketsBot");
        public static Settings Settings { get; private set; }

        public static void ReadAPIKeys()
        {
            string fp = Path.Combine(Bot.PersonalFolder, "BTCMarketsAuth.txt");
            if (File.Exists(fp))
            {
                using (StreamReader sr = new StreamReader(fp))
                {
                    ApplicationConstants.API_KEY = sr.ReadLine();
                    ApplicationConstants.PRIVATE_KEY = sr.ReadLine();
                    sr.Close();
                }
            }
        }

        public static string SettingsFilePath
        {
            get
            {
                return Path.Combine(PersonalFolder, "Settings.json");
            }
        }

        public static string LogFilePath
        {
            get
            {
                string logsFolder = Path.Combine(PersonalFolder, "Logs");
                string filename = string.Format("BTCmBot-Log-{0:yyyy-MM}.csv", DateTime.Now);
                return Path.Combine(logsFolder, filename);
            }
        }

        public static void LoadSettings()
        {
            Settings = Settings.Load(SettingsFilePath);
        }

        public static void SaveSettings()
        {
            if (Settings != null)
            {
                Settings.Save(SettingsFilePath);
            }
        }
    }
}