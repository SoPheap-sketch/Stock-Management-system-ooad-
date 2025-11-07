using System;

namespace StockManagementSystem.Classes
{
    public class Customer
    {
        public int CustomerID { get; set; }    // PK
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
                throw new Exception("Customer name is required.");
            if (string.IsNullOrWhiteSpace(Email))
                throw new Exception("Customer email is required.");
        }
    }
}
