using System;
using System.Collections.Generic;

namespace StockManagementSystem.Classes
{
    public class Order
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }        // TEMP: for current form
        public string ProductName { get; set; }   // TEMP: for current form
        public int Quantity { get; set; }         // TEMP: for current form
        public decimal Price { get; set; }        // TEMP: for current form
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount => Quantity * Price;

        // --- New UML fields ---
        public Customer Customer { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var item in OrderItems)
                total += item.CalculateSubtotal();
            return total;
        }

        public void AddItem(OrderItem item)
        {
            OrderItems.Add(item);
        }

        public void CompleteOrder()
        {
            Status = "Delivered";
        }
    }
}
