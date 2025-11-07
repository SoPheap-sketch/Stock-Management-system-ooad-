using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using StockManagementSystem.Classes;
using System.Collections.Generic;

namespace StockManagementSystem.Forms
{
    public partial class OrderForm : Form
    {
        private int selectedOrderId = -1;

        public OrderForm()
        {
            InitializeComponent();
            InitializeForm();
        }

        private void InitializeForm()
        {
            LoadCustomers();
            LoadProducts();
            SetupDataGridView();
            LoadOrders();

            // Buttons
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnSearch.Click += BtnSearch_Click;

            // DataGridView click
            dgvOrders.CellClick += DgvOrders_CellClick;

            // Status ComboBox
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Delivered", "Pending", "Cancelled" });
            cmbStatus.SelectedIndex = 1; // Default to Pending

            // Allow free text input for Customer
            cmbCustomer.DropDownStyle = ComboBoxStyle.DropDown;
        }

        private void LoadCustomers()
        {
            try
            {
                DataTable customers = CustomerManager.GetAllCustomers();

                // Add walk-in customer option
                DataRow newRow = customers.NewRow();
                newRow["CustomerID"] = DBNull.Value;
                newRow["Name"] = "--- Walk-in Customer ---";
                customers.Rows.InsertAt(newRow, 0);

                cmbCustomer.DataSource = customers;
                cmbCustomer.DisplayMember = "Name";
                cmbCustomer.ValueMember = "CustomerID";
                cmbCustomer.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
                cmbCustomer.DataSource = null;
            }
        }

        private void LoadProducts()
        {
            DataTable products = ProductManager.GetAllProducts();
            cmbProduct.DataSource = products;
            cmbProduct.DisplayMember = "ProductName";
            cmbProduct.ValueMember = "ProductID";
        }

        private void SetupDataGridView()
        {
            dgvOrders.Columns.Clear();
            dgvOrders.Columns.Add("OrderID", "Order ID");
            dgvOrders.Columns.Add("CustomerName", "Customer");
            dgvOrders.Columns.Add("ProductName", "Product");
            dgvOrders.Columns.Add("Quantity", "Quantity");
            dgvOrders.Columns.Add("Price", "Price");
            dgvOrders.Columns.Add("Total", "Total");
            dgvOrders.Columns.Add("Status", "Status");
            dgvOrders.Columns.Add("OrderDate", "Date");

            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrders.ReadOnly = true;
            dgvOrders.AllowUserToAddRows = false;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.EnableHeadersVisualStyles = false;
            dgvOrders.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgvOrders.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvOrders.CellFormatting += DgvOrders_CellFormatting;
        }

        private void DgvOrders_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvOrders.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                switch (e.Value.ToString())
                {
                    case "Delivered": e.CellStyle.ForeColor = Color.Green; break;
                    case "Pending": e.CellStyle.ForeColor = Color.Orange; break;
                    case "Cancelled": e.CellStyle.ForeColor = Color.Red; break;
                }
            }
        }

        private void LoadOrders()
        {
            dgvOrders.Rows.Clear();
            DataTable dt = OrderManager.GetOrders();

            foreach (DataRow row in dt.Rows)
            {
                string customerName = row["CustomerName"] != DBNull.Value
                                      ? row["CustomerName"].ToString()
                                      : "--- Walk-in Customer ---";

                decimal price = Convert.ToDecimal(row["Price"]);
                int quantity = Convert.ToInt32(row["Quantity"]);

                dgvOrders.Rows.Add(
                    row["OrderID"],
                    customerName,
                    row["ProductName"],
                    quantity,
                    price.ToString("C2"),
                    (price * quantity).ToString("C2"),
                    row["Status"],
                    Convert.ToDateTime(row["OrderDate"]).ToString("MM/dd/yyyy")
                );
            }
        }

        private void DgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvOrders.Rows[e.RowIndex];
            selectedOrderId = Convert.ToInt32(row.Cells["OrderID"].Value);

            // Customer
            string custName = row.Cells["CustomerName"].Value.ToString();
            int custIndex = cmbCustomer.FindStringExact(custName);
            cmbCustomer.SelectedIndex = custIndex >= 0 ? custIndex : 0;

            // Product
            cmbProduct.Text = row.Cells["ProductName"].Value.ToString();

            // Quantity
            numQuantity.Value = Convert.ToInt32(row.Cells["Quantity"].Value);

            // Price
            decimal cellPrice;
            if (decimal.TryParse(row.Cells["Price"].Value.ToString(), System.Globalization.NumberStyles.Currency, System.Globalization.CultureInfo.CurrentCulture, out cellPrice))
            {
                txtPrice.Text = cellPrice.ToString("F2");
            }

            // Status
            cmbStatus.Text = row.Cells["Status"].Value.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProduct.SelectedValue == null || numQuantity.Value <= 0 || !decimal.TryParse(txtPrice.Text, out decimal priceAtPurchase))
                {
                    MessageBox.Show("Please select a product, quantity, and valid price.");
                    return;
                }

                string custName = cmbCustomer.Text.Trim();
                Customer selectedCustomer = null;

                if (!string.IsNullOrEmpty(custName) && custName != "--- Walk-in Customer ---")
                {
                    DataTable dtCust = CustomerManager.GetCustomerByName(custName);
                    if (dtCust.Rows.Count > 0)
                    {
                        selectedCustomer = new Customer { CustomerID = Convert.ToInt32(dtCust.Rows[0]["CustomerID"]) };
                    }
                    else
                    {
                        int newId = CustomerManager.AddCustomer(custName);
                        selectedCustomer = new Customer { CustomerID = newId };
                    }
                }

                Product selectedProduct = new Product
                {
                    ProductID = (int)cmbProduct.SelectedValue,
                    ProductName = cmbProduct.Text,
                    Price = priceAtPurchase
                };

                OrderItem newItem = new OrderItem
                {
                    Product = selectedProduct,
                    Quantity = (int)numQuantity.Value,
                    PriceAtPurchase = priceAtPurchase
                };

                Order order = new Order
                {
                    Customer = selectedCustomer,
                    Status = cmbStatus.Text,
                    OrderDate = DateTime.Now,
                    OrderItems = new List<OrderItem> { newItem }
                };

                OrderManager.AddOrder(order);
                LoadOrders();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding order: " + ex.Message);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedOrderId <= 0) return;

            try
            {
                if (cmbProduct.SelectedValue == null || numQuantity.Value <= 0 || !decimal.TryParse(txtPrice.Text, out decimal priceAtPurchase))
                {
                    MessageBox.Show("Please select a product, quantity, and valid price.");
                    return;
                }

                string custName = cmbCustomer.Text.Trim();
                Customer selectedCustomer = null;

                if (!string.IsNullOrEmpty(custName) && custName != "--- Walk-in Customer ---")
                {
                    DataTable dtCust = CustomerManager.GetCustomerByName(custName);
                    if (dtCust.Rows.Count > 0)
                        selectedCustomer = new Customer { CustomerID = Convert.ToInt32(dtCust.Rows[0]["CustomerID"]) };
                    else
                    {
                        int newId = CustomerManager.AddCustomer(custName);
                        selectedCustomer = new Customer { CustomerID = newId };
                    }
                }

                Order updatedOrder = new Order
                {
                    OrderID = selectedOrderId,
                    Customer = selectedCustomer,
                    Status = cmbStatus.Text,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Product = new Product { ProductID = (int)cmbProduct.SelectedValue },
                            Quantity = (int)numQuantity.Value,
                            PriceAtPurchase = priceAtPurchase
                        }
                    }
                };

                OrderManager.UpdateOrder(updatedOrder);
                LoadOrders();
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating order: " + ex.Message);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedOrderId <= 0) return;

            var confirm = MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                OrderManager.DeleteOrder(selectedOrderId);
                LoadOrders();
                ClearInputs();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
            ClearInputs();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim().ToLower();
            dgvOrders.Rows.Clear();

            DataTable dt = OrderManager.GetOrders();
            foreach (DataRow row in dt.Rows)
            {
                string customerName = row["CustomerName"] != DBNull.Value
                                      ? row["CustomerName"].ToString()
                                      : "--- Walk-in Customer ---";
                string productName = row["ProductName"].ToString();

                if (!customerName.ToLower().Contains(search) && !productName.ToLower().Contains(search))
                    continue;

                decimal price = Convert.ToDecimal(row["Price"]);
                int quantity = Convert.ToInt32(row["Quantity"]);

                dgvOrders.Rows.Add(
                    row["OrderID"],
                    customerName,
                    productName,
                    quantity,
                    price.ToString("C2"),
                    (price * quantity).ToString("C2"),
                    row["Status"],
                    Convert.ToDateTime(row["OrderDate"]).ToString("MM/dd/yyyy")
                );
            }
        }

        private void ClearInputs()
        {
            selectedOrderId = -1;
            cmbCustomer.Text = "--- Walk-in Customer ---";
            cmbProduct.SelectedIndex = cmbProduct.Items.Count > 0 ? 0 : -1;
            numQuantity.Value = 1;
            txtPrice.Clear();
            cmbStatus.SelectedIndex = 1;
        }
    }
}
