using CommonSpider.Jobs.DoubanRent.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CommonSpider.Jobs.DoubanRent.Repositories
{
    public class DoubanRentRepository : BaseRepository
    {
        public bool Insert(DoubanRentSummary rentSummary)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO rentsummary(Id, Title, Author, Url) VALUES(@id, @title, @author, @url)";
                int row = conn.Execute(query, rentSummary);
                return row > 0;
            }
        }

        public DoubanRentSummary Query(int id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * FROM RentSummary WHERE Id = @id";
                var rentSummary = conn.Query<DoubanRentSummary>(query, new { id = id }).FirstOrDefault();
                return rentSummary;
            }
        }
    }
}
