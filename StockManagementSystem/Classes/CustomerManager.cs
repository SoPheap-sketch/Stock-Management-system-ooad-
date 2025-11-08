using System;
using System.Data;
using System.Data.SqlClient;

namespace StockManagementSystem.Classes
{
    public static class CustomerManager
    {
        // Get all customers
        public static DataTable GetAllCustomers()
        {
            DataTable dt = new DataTable();
            string query = "SELECT CustomerID, Name, Address, Phone, Email FROM Customers ORDER BY Name ASC";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    conn.Open();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading customers: " + ex.Message);
            }

            return dt;
        }

        // Search customers by name
        public static DataTable GetCustomerByName(string name)
        {
            DataTable dt = new DataTable();
            string query = "SELECT CustomerID, Name, Address, Phone, Email FROM Customers WHERE Name LIKE @Name ORDER BY Name ASC";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.AddWithValue("@Name", "%" + name + "%");
                    conn.Open();
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error searching customers: " + ex.Message);
            }

            return dt;
        }

        // Add new customer
        public static int AddCustomer(string name, string address, string phone, string email)
        {
            int newId = 0;
            string query = "INSERT INTO Customers (Name, Address, Phone, Email) OUTPUT INSERTED.CustomerID VALUES (@Name, @Address, @Phone, @Email)";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);

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

        // Update existing customer
        public static void UpdateCustomer(int customerId, string name, string address, string phone, string email)
        {
            string query = "UPDATE Customers SET Name=@Name, Address=@Address, Phone=@Phone, Email=@Email WHERE CustomerID=@CustomerID";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@Email", email);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating customer: " + ex.Message);
            }
        }

        // Delete a customer
        public static void DeleteCustomer(int customerId)
        {
            string query = "DELETE FROM Customers WHERE CustomerID=@CustomerID";

            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerID", customerId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting customer: " + ex.Message);
            }
        }
    }
}
