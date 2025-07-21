using System.Data;

namespace BlasteR.Base.Tests
{
    public static class DbConnectionFactory
    {
        public static IUnitOfWork GetInMemoryUnitOfWork(string username)
        {
            IDbConnection connection = new Microsoft.Data.Sqlite.SqliteConnection("DataSource=file::memory:?cache=shared");

            connection.Open();
            return new UnitOfWork(connection, null, username);
        }
    }
}
