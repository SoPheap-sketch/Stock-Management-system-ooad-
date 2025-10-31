using System;

namespace StockManagementSystem.Classes
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }

        public decimal Total => Quantity * Price;

        public Order() { }

        public Order(int orderId, int productId, string productName, int quantity, decimal price, string status)
        {
            OrderID = orderId;
            ProductID = productId;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
            Status = status;
            OrderDate = DateTime.Now;
        }
    }
}
