using CommonSpider.Jobs.Rent58.Entities;
using Dapper;
using System.Data;
using System.Linq;

namespace CommonSpider.Jobs.Rent58.Repositories
{
    public class Rent58Repository : BaseRepository
    {
        public bool Insert(Rent58Item item)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "INSERT INTO Rent58Item([Id], [Title], [PublishDate], [Content], [Url]) VALUES(@id, @title, @publishDate, @content, @url)";
                int row = conn.Execute(query, item);
                return row > 0;
            }
        }

        public Rent58Item Query(string id)
        {
            using (IDbConnection conn = OpenConnection())
            {
                const string query = "SELECT * from [Rent58Item] WHERE Id = @id";
                var item = conn.Query<Rent58Item>(query, new { id = id }).FirstOrDefault();
                return item;
            }
        }
    }
}
