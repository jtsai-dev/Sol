using MySql.Data.MySqlClient;
using RentSpider.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;
using System.Linq;
using System.Configuration;

namespace RentSpider.Repositories
{
    public class RentSummaryRepository
    {
        private static MySqlConnection _connection;
        private static MySqlConnection OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(ConfigurationManager.AppSettings.Get("ConnectionString"));
            }
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            return _connection;
        }

        public bool Insert(RentSummary rentSummary)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO rentsummary(Id, Title, Author, Url) VALUES(@id, @title, @author, @url)";
                int row = conn.Execute(query, rentSummary);
                return row > 0;
            }
        }

        public RentSummary Query(int id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * FROM RentSummary WHERE Id = @id";
                var rentSummary = conn.Query<RentSummary>(query, new { id = id }).FirstOrDefault();
                return rentSummary;
            }
        }
    }
}
