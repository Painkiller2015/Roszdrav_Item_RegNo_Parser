using CheckRuCode.Config;
using CheckRuCode.Objects;
using System.Data.SqlClient;

namespace CheckRuCode.Service
{
    public class SQL_Connection
    {
        private const string DBUserName = "";
        private const string DBUserPass = "";
        private const string DBServerName = "";
        private const string DBInstanceName = "";
        private string _ConnectionString;
        
        public SQL_Connection()
        {
            SqlConnectionStringBuilder stringBuilder = new();
            stringBuilder.UserID = DBUserName;
            stringBuilder.DataSource = DBServerName;
            stringBuilder.InitialCatalog = DBInstanceName;
            stringBuilder.Password = DBUserPass;
            stringBuilder.ConnectTimeout = 10000;
            stringBuilder.IntegratedSecurity = false;
            _ConnectionString = stringBuilder.ConnectionString;            
        }

        public List<Item> GetрHavingItems()
        {
            const int RuCodeColumn = 0;
            List<Item> items = new();

            string sqlRequestTextPath = Settings.CustomSettingsValues.SQL_Request;
            string sqlExpression = File.ReadAllText(sqlRequestTextPath);            

            using (SqlConnection connection = new(_ConnectionString))
            {
                connection.Open();
                SqlCommand command = new(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Item item = new(reader.GetString(RuCodeColumn), null);
                    items.Add(item);
                }
                return items;
            }
        }
    }
}
