using System.Data.SqlClient;

namespace StockManagementSystem.Classes
{
    public static class DatabaseHelper
    {
        private static string connectionString =
            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=StockDB;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
