using CommonSpider.Jobs.DoubanRent.Entities;
using Dapper;
using System;
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
                try
                {
                    const string query = "INSERT INTO RentSummary(Id, Title, Author, Url) VALUES(@id, @title, @author, @url)";
                    return await conn.ExecuteAsync(query, rentSummary) > 0;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        public async Task<DoubanRentSummary> QueryAsync(int id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                try
                {
                    const string query = "SELECT * FROM RentSummary WHERE Id = @id LIMIT 1;";
                    return (await conn.QueryAsync<DoubanRentSummary>(query, new { id = id }))?.FirstOrDefault() ?? null;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
    }
}
