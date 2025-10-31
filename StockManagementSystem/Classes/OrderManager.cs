using System;
using System.Data;
using System.Data.SqlClient;

namespace StockManagementSystem.Classes
{
    public static class OrderManager
    {
        public static DataTable GetOrders()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Orders";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading orders: " + ex.Message);
            }
            return dt;
        }

        public static void AddOrder(Order order)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO Orders (ProductID, ProductName, Quantity, Price, Status, OrderDate) " +
                                   "VALUES (@pid, @pname, @qty, @price, @status, @date)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pid", order.ProductID);
                        cmd.Parameters.AddWithValue("@pname", order.ProductName);
                        cmd.Parameters.AddWithValue("@qty", order.Quantity);
                        cmd.Parameters.AddWithValue("@price", order.Price);
                        cmd.Parameters.AddWithValue("@status", order.Status);
                        cmd.Parameters.AddWithValue("@date", order.OrderDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding order: " + ex.Message);
            }
        }

        public static void UpdateOrder(Order order)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Orders SET ProductID=@pid, ProductName=@pname, Quantity=@qty, Price=@price, Status=@status, OrderDate=@date " +
                                   "WHERE OrderID=@id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", order.OrderID);
                        cmd.Parameters.AddWithValue("@pid", order.ProductID);
                        cmd.Parameters.AddWithValue("@pname", order.ProductName);
                        cmd.Parameters.AddWithValue("@qty", order.Quantity);
                        cmd.Parameters.AddWithValue("@price", order.Price);
                        cmd.Parameters.AddWithValue("@status", order.Status);
                        cmd.Parameters.AddWithValue("@date", order.OrderDate);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating order: " + ex.Message);
            }
        }

        public static void DeleteOrder(int orderId)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Orders WHERE OrderID=@id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", orderId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting order: " + ex.Message);
            }
        }
    }
}
