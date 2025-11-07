using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

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
                    string query = @"
                        SELECT 
                            o.OrderID, 
                            o.CustomerID,
                            c.Name AS CustomerName,
                            o.OrderDate, 
                            o.Status, 
                            o.TotalAmount,
                            p.ProductName,
                            oi.Quantity,
                            oi.Price
                        FROM Orders o
                        INNER JOIN OrderItems oi ON o.OrderID = oi.OrderID
                        INNER JOIN Products p ON oi.ProductID = p.ProductID
                        LEFT JOIN Customers c ON o.CustomerID = c.CustomerID
                        ORDER BY o.OrderDate DESC";

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
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    string insertOrder = @"
                        INSERT INTO Orders (CustomerID, Status, OrderDate, TotalAmount)
                        OUTPUT INSERTED.OrderID
                        VALUES (@CustomerID, @Status, @OrderDate, @TotalAmount)";

                    int orderId;
                    using (SqlCommand cmd = new SqlCommand(insertOrder, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", (object)(order.Customer == null ? DBNull.Value : (object)order.Customer.CustomerID));
                        cmd.Parameters.AddWithValue("@Status", order.Status ?? "Pending");
                        cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        cmd.Parameters.AddWithValue("@TotalAmount", order.CalculateTotal());
                        orderId = (int)cmd.ExecuteScalar();
                    }

                    foreach (var item in order.OrderItems)
                    {
                        string insertItem = @"
                            INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price)
                            VALUES (@OrderID, @ProductID, @Quantity, @Price)";
                        using (SqlCommand cmd = new SqlCommand(insertItem, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@Price", item.PriceAtPurchase);
                            cmd.ExecuteNonQuery();
                        }

                        string updateStock = "UPDATE Products SET Quantity = Quantity - @Quantity WHERE ProductID=@ProductID";
                        using (SqlCommand cmd = new SqlCommand(updateStock, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    throw new Exception("Error adding order: " + ex.Message);
                }
            }
        }

        public static void UpdateOrder(Order order)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // Update Order main info
                    string updateOrder = @"
                        UPDATE Orders 
                        SET CustomerID=@CustomerID, Status=@Status, TotalAmount=@TotalAmount
                        WHERE OrderID=@OrderID";

                    using (SqlCommand cmd = new SqlCommand(updateOrder, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                        cmd.Parameters.AddWithValue("@CustomerID", (object)(order.Customer == null ? DBNull.Value : (object)order.Customer.CustomerID));
                        cmd.Parameters.AddWithValue("@Status", order.Status ?? "Pending");
                        cmd.Parameters.AddWithValue("@TotalAmount", order.CalculateTotal());
                        cmd.ExecuteNonQuery();
                    }

                    // Delete existing items first
                    string deleteItems = "DELETE FROM OrderItems WHERE OrderID=@OrderID";
                    using (SqlCommand cmd = new SqlCommand(deleteItems, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                        cmd.ExecuteNonQuery();
                    }

                    // Re-insert new items and update stock
                    foreach (var item in order.OrderItems)
                    {
                        string insertItem = @"
                            INSERT INTO OrderItems (OrderID, ProductID, Quantity, Price)
                            VALUES (@OrderID, @ProductID, @Quantity, @Price)";
                        using (SqlCommand cmd = new SqlCommand(insertItem, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                            cmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@Price", item.PriceAtPurchase);
                            cmd.ExecuteNonQuery();
                        }

                        string updateStock = "UPDATE Products SET Quantity = Quantity - @Quantity WHERE ProductID=@ProductID";
                        using (SqlCommand cmd = new SqlCommand(updateStock, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                            cmd.Parameters.AddWithValue("@ProductID", item.Product.ProductID);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    throw new Exception("Error updating order: " + ex.Message);
                }
            }
        }

        public static void DeleteOrder(int orderId)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlTransaction transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    string deleteItems = "DELETE FROM OrderItems WHERE OrderID=@OrderID";
                    using (SqlCommand cmd = new SqlCommand(deleteItems, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    string deleteOrder = "DELETE FROM Orders WHERE OrderID=@OrderID";
                    using (SqlCommand cmd = new SqlCommand(deleteOrder, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    throw new Exception("Error deleting order: " + ex.Message);
                }
            }
        }
    }
}
