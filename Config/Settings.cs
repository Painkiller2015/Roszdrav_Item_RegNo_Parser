using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using CheckRuCode.Service;

namespace CheckRuCode.Config
{
    /// <summary>
    /// Void Init() before get values
    /// </summary>
    public static class Settings
    {
        public static SettingsFields CustomSettingsValues { get; private set; }
        public const string CSVFileName = "RuCode.csv";
        public const string ArchiveFileName = "RuCode.zip";        
        public static void Init()
        {
            string json = File.ReadAllText(@".\Config\Config.json");
            CustomSettingsValues = JsonConvert.DeserializeObject<SettingsFields>(json);

            ValidateSettings();            
            CustomSettingsValues.Setting_File = FileWorker.MakePath("Config", CustomSettingsValues.Setting_File);
            CustomSettingsValues.OutputFileName = FileWorker.MakePath(CustomSettingsValues.OutputFileName);
            CustomSettingsValues.SQL_Request = FileWorker.MakePath("SQL_Requests", CustomSettingsValues.SQL_Request);
        }

        private static void ValidateSettings()
        {
            var SettingsValuesFields = CustomSettingsValues.GetType().GetProperties();
            foreach (var Field in SettingsValuesFields)
            {
                if (Field.GetValue(CustomSettingsValues) == null)
                {
                    throw new Exception("Ошибка десериализации настроек, не корректен файл");
                }
            }
        }

        public class SettingsFields
        {
            public string BaseEncoding;
            public string Setting_File;
            public string SQL_Request;
            public string OutputFileName;
            public string ToSendMail;
            public string SenderMail;
            public string SubjectMail;
            public string MessageMail;
            public string RZSiteBaseURL;
            public string RZSiteIncludeFileLink;
            public string RZSiteBaseAdress;
            public string RZSiteRequest;
        }
    }
}
