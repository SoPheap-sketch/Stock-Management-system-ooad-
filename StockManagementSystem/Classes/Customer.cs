using System;

namespace StockManagementSystem.Classes
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public Customer() { }

        public Customer(int id, string name, string address, string phone, string email)
        {
            CustomerID = id;
            Name = name;
            Address = address;
            Phone = phone;
            Email = email;
        }

        public void ValidateContactInfo()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Customer name cannot be empty.");
            if (string.IsNullOrWhiteSpace(Address))
                throw new ArgumentException("Customer address cannot be empty.");
        }
    }
}
