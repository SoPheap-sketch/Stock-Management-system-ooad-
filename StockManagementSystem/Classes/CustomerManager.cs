using System;
using System.Data;
using System.Data.SqlClient;

namespace StockManagementSystem.Classes
{
    public static class CustomerManager
    {
        public static DataTable GetAllCustomers()
        {
            DataTable dt = new DataTable();
            string query = "SELECT CustomerID, Name FROM Customers ORDER BY Name ASC";

            try
            {
                using (SqlConnection connection = DatabaseHelper.GetConnection())
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    connection.Open();
                    adapter.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading customers: {ex.Message}");
            }

            return dt;
        }

        public static DataTable GetCustomerByName(string name)
        {
            DataTable dt = new DataTable();
            string query = "SELECT CustomerID, Name FROM Customers WHERE Name=@Name";
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    conn.Open();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting customer by name: " + ex.Message);
            }
            return dt;
        }

        public static int AddCustomer(string name)
        {
            int newId = 0;
            string query = "INSERT INTO Customers (Name) OUTPUT INSERTED.CustomerID VALUES (@Name)";
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    conn.Open();
                    newId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding customer: " + ex.Message);
            }
            return newId;
        }
    }
}
