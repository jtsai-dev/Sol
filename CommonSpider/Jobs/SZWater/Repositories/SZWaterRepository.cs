using CommonSpider.Jobs.SZWater.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
using System.Linq;

namespace CommonSpider.Jobs.SZWater.Repositories
{
    public class SZWaterRepository: BaseRepository
    {
        public bool Insert(Notice notice)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO Notice([Id], [Title], [PublishDate], [Content], [Url]) VALUES(@id, @title, @publishDate, @content, @url)";
                int row = conn.Execute(query, notice);
                return row > 0;
            }
        }

        public Notice Query(string id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from [Notice] WHERE Id = @id";
                var notice = conn.Query<Notice>(query, new { id = id }).FirstOrDefault();
                return notice;
            }
        }
    }
}
