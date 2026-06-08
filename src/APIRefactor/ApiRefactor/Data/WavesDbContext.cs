using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApiRefactor.Data;

public class WavesDbContext : DbContext
{
    private const string ConnectionString = "Data Source=App_Data/waves.db";

    public SqliteConnection GetConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        return connection;
    }

    public void EnsureConnectionOpen(SqliteConnection connection)
    {
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public void CloseConnection(SqliteConnection connection)
    {
        if (connection != null && connection.State != System.Data.ConnectionState.Closed)
        {
            connection.Close();
        }
    }
}
