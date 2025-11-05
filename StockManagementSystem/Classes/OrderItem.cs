using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManagementSystem.Classes
{
    internal class OrderItem
    {
        public int OrderItemId { get; set; }    
        public int Quantity { get; set;  }
        public decimal PriceAtPurchase { get; set; }
        public Product Product { get; set; }

        public decimal CalculateSubtotal() => Quantity * PriceAtPurchase;
    }
}
