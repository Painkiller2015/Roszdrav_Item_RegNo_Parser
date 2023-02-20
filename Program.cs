using CheckRuCode.Config;
using CheckRuCode.Objects;
using CheckRuCode.Service;

try
{
    Settings.Init();

    FileWorker fw = new();
    fw.ValidateFile();

    await GetSiteArchive(fw);
    
    List<Item> itemsHaving = new SQL_Connection().GetрHavingItems();
    List<Item> itemsCVS = fw.GetСSVItems();

    List<Item> itemsWitchDiffRuCode = EqualsItems(in itemsHaving, in itemsCVS);

    if (itemsWitchDiffRuCode.Count > 0)
    {
        CSV_Creator CSV = new(Settings.CustomSettingsValues.OutputFileName);
        CSV.ExportToFile(ref itemsWitchDiffRuCode);
        
        SendMail();
    }
    fw.ValidateFile();
}
catch (Exception)
{
    try
    {
        Settings.Init();
        MailSender ms = new();
        ms.SendEmail(
            Settings.CustomSettingsValues.SenderMail,
            Settings.CustomSettingsValues.ToSendMail,
            Settings.CustomSettingsValues.SubjectMail, "Не удалось сформировать, оповестите отдел АСУ, код ошибки: 'CheckRuCode' ",
            Settings.CustomSettingsValues.OutputFileName
            );
    }
    catch (Exception)
    {
        Console.WriteLine("Заданы не верные настройки");
    }
}
List<Item> EqualsItems(in List<Item> Having, in List<Item> CSV)
{
    Item tmpHavingItem = null;
    Item tmpCSVItem = null;
    List<Item> diffItems = new();
    DateTime nowDate = DateTime.Now;
    for (int i = 0; i < Having.Count; i++)
    {
        tmpHavingItem = Having.ElementAt(i);

        if (CSV.Where(el => el.registrationNumber.ToUpper() == tmpHavingItem.registrationNumber.ToUpper()).Any())
        {
            tmpCSVItem = CSV.First();
            if (tmpCSVItem.registrationDateEnd == null)
                continue;
            else
            {
                TimeSpan delta = tmpCSVItem.registrationDateEnd.Value.Subtract(nowDate);
                int days = (int)delta.TotalDays;
                if (days < 60)
                {
                    diffItems.Add(tmpHavingItem);
                    continue;
                }
            }
            continue;
        }
        diffItems.Add(tmpHavingItem);
    }
    return diffItems;
}

static async Task GetSiteArchive(FileWorker fw)
{
    SiteParser sp = new();
    await sp.DownloadArchive();
    fw.ExtractArchive();
}

static void SendMail()
{
    MailSender ms = new();
    ms.SendEmail(
        Settings.CustomSettingsValues.SenderMail,
        Settings.CustomSettingsValues.ToSendMail,
        Settings.CustomSettingsValues.SubjectMail, "Расхождение по кодам",
        Settings.CustomSettingsValues.OutputFileName
        );
}