using CommonSpider.Jobs.DoubanRent.Entities;
using Dapper;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CommonSpider.Jobs.DoubanRent.Repositories
{
    public class DoubanRentRepository : BaseRepository
    {
        public async Task<bool> InsertAsync(DoubanRentSummary rentSummary)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO rentsummary(Id, Title, Author, Url) VALUES(@id, @title, @author, @url)";
                return await conn.ExecuteAsync(query, rentSummary) > 0;
            }
        }

        public async Task<DoubanRentSummary> QueryAsync(int id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * FROM RentSummary WHERE Id = @id LIMIT 1;";
                return await conn.QueryFirstAsync<DoubanRentSummary>(query, new { id = id });
            }
        }
    }
}
