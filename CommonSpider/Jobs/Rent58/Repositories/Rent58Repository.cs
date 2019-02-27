using CommonSpider.Jobs.Rent58.Entities;
using Dapper;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSpider.Jobs.Rent58.Repositories
{
    public class Rent58Repository : BaseRepository
    {
        public async Task<bool> InsertAsync(Rent58Item item)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO Rent58Item([Id], [Title], [PublishDate], [Content], [Url]) VALUES(@id, @title, @publishDate, @content, @url)";
                return await conn.ExecuteAsync(query, item) > 0;
            }
        }

        public async Task<Rent58Item> QueryAsync(string id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from [Rent58Item] WHERE Id = @id";
                return await conn.QueryFirstAsync<Rent58Item>(query, new { id = id });
            }
        }
    }
}
