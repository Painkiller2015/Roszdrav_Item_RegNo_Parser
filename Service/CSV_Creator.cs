using CheckRuCode.Objects;
using System.Text;

namespace CheckRuCode.Service
{
    internal class CSV_Creator
    {
        string _FilePath;

        public CSV_Creator(string filePath)
        {
            _FilePath = filePath;
        }
        public void ExportToFile(ref List<Item> items)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string csv = String.Join("\n", items.Select(x => x.registrationNumber?.ToString()).ToArray());
            File.WriteAllText(_FilePath, csv, Encoding.GetEncoding("windows-1251"));
        }
    }
}
