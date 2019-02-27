using CommonSpider.Jobs.SZWater.Entities;
using Dapper;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSpider.Jobs.SZWater.Repositories
{
    public class SZWaterRepository : BaseRepository
    {
        public async Task<bool> InsertAsync(Notice notice)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO Notice([Id], [Title], [PublishDate], [Content], [Url]) VALUES(@id, @title, @publishDate, @content, @url)";
                return await conn.ExecuteAsync(query, notice) > 0;
            }
        }

        public async Task<Notice> QueryAsync(string id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from [Notice] WHERE Id = @id";
                return await conn.QueryFirstAsync<Notice>(query, new { id = id });
            }
        }
    }
}
