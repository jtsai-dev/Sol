using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace CommonSpider.Jobs
{
    public class BaseRepository
    {
        private static MySqlConnection _connection;
        public static MySqlConnection OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("ConnectionString"));
            }
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            return _connection;
        }
    }
}
