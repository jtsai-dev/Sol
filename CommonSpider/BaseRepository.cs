using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace CommonSpider
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
