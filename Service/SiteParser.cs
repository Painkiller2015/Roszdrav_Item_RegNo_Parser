using System.Text.RegularExpressions;
using System.Net;
using CheckRuCode.Config;


namespace CheckRuCode.Service
{
    internal class SiteParser
    {
        private string _URL;
        private Regex _regex = new(@"\/opendata\/[^\s]*.zip");//new(@"^https:\/\/roszdravnadzor.gov.ru\/opendata\/[^\s]*.zip$");
        //private Regex _regex = new(@"[^\s]*Win[^\s]*.exe$"); //@"^\Qhttps://roszdravnadzor.gov.ru/opendata/\E[^\s]*.zip$"
        //public SiteParser(string URL)
        //{
        //    _URL = URL;
        //}

        public async Task DownloadArchive()
        {                                    
            string downloadLink = await GetArchiveLink();
            
            if (string.IsNullOrEmpty(downloadLink))
            {
                throw new Exception("Не удалось спарсить ссылку на скачивание");
            }

            using (var client = new WebClient())
            {
                Console.WriteLine("Скачивание архива началось");
                
                string archivePath = FileWorker.MakePath(Settings.ArchiveFileName);
                client.DownloadFile(downloadLink, archivePath);

                Console.WriteLine("Скачивание архива завершено");
            }
        }
        private async Task<string> GetArchiveLink()
        {
            string request = Settings.CustomSettingsValues.RZSiteRequest;                

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(Settings.CustomSettingsValues.RZSiteBaseAdress),
            };

            using HttpResponseMessage response = await httpClient.GetAsync(request);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return Settings.CustomSettingsValues.RZSiteBaseURL + _regex.Match(jsonResponse).Value;             
        }
    }
}
