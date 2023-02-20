using CheckRuCode.Config;
using CheckRuCode.Objects;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace CheckRuCode.Service
{
    public class FileWorker
    {
        readonly string _fileArchivePath;
        readonly string _fileCSVPath;
        readonly string _fileOut;
        readonly Encoding _encoder;

        public FileWorker()
        {
            _fileArchivePath = Settings.ArchiveFileName;
            _fileCSVPath = Settings.CSVFileName;
            _fileOut = Settings.CustomSettingsValues.OutputFileName;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _encoder = Encoding.GetEncoding("windows-1251");

        }
        public List<Item> GetСSVItems()
        {
            Regex regexDate = new(@"(3[01]|[12][0-9]|0?[1-9])\.(1[012]|0?[1-9])\.((?:19|20)\d{2})");//new(@"[^\s]*(3[01]|[12][0-9]|0?[1-9])\.(1[012]|0?[1-9])\.((?:19|20)\d{2})[^\s]*");
            List<Item> items = new();
            DateTime dateEnd = new();


            using (var reader = new StreamReader(_fileCSVPath, _encoder))
            {
                List<string> badRecord = new List<string>();
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    Mode = CsvMode.NoEscape,
                    BadDataFound = context => badRecord.Add(context.RawRecord)
                };

                using (var csv = new CsvReader(reader, config))
                {
                    Regex _regexCode = new(@"[^""]*[^""]");
                    int registrationNumberColumn = 1;
                    int registrationDateEndColumn = 3;
                    DateTime date = new();

                    csv.Read();

                    while (csv.Parser.Read())
                    {
                        if (csv.Parser.Record.Length > 3)
                        {
                            if (!String.IsNullOrEmpty(csv.Parser.Record[registrationNumberColumn]))
                            {
                                string tmdDate = regexDate.Match(csv.Parser.Record[registrationDateEndColumn]).Value;
                                string validateCode = _regexCode.Match(csv.Parser.Record[registrationNumberColumn]).Value;

                                if (validateCode.Contains(','))
                                {
                                    validateCode.Split(',');
                                }

                                if (DateTime.TryParseExact(tmdDate, "dd.MM.yyyy", new CultureInfo("ru-RU"), DateTimeStyles.None, out date))
                                {
                                    items.Add(new Item(validateCode, date));
                                }
                                else
                                {
                                    items.Add(new Item(validateCode, null));
                                }
                            }
                        }
                    }
                    return items;
                }
            }
        }

        public void ExtractArchive()
        {
            using (FileStream fs = new(_fileArchivePath, FileMode.Open))
            {
                using (ZipArchive zip = new(fs))
                {
                    zip.ExtractToDirectory(@"./", true);
                    string newCSVPath = Directory.GetFiles(@"./", "*.csv").FirstOrDefault();
                    if (newCSVPath != default)
                    {
                        File.Move(newCSVPath, _fileCSVPath);
                    }
                };
            };
        }
        public void ValidateFile()
        {
            string currentPath = Directory.GetCurrentDirectory();
            List<string> filteredFiles = Directory
                .EnumerateFiles(currentPath)
                .Where(file => file.ToLower().EndsWith("csv") || file.ToLower().EndsWith("zip"))
                .ToList();
            foreach (var file in filteredFiles)
            {
                File.Delete(file);
            }
        }
        public static string MakePath(params string[] path )
        {
            string currPath = Directory.GetCurrentDirectory();       
            return Path.Combine(currPath, Path.Combine(path));           
        }
    }
}
