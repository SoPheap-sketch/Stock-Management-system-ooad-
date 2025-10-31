using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using StockManagementSystem.Classes;

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
            // Load products into ComboBox
            LoadProducts();

            // Setup DataGridView columns
            SetupDataGridView();

            // Load orders into DataGridView
            LoadOrders();

            // Wire button events
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += BtnRefresh_Click;

            // Wire DataGridView row click
            dgvOrders.CellClick += DgvOrders_CellClick;

            // Setup Status ComboBox
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Delivered", "Pending", "Cancelled" });
            cmbStatus.SelectedIndex = 1; // default Pending
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
                decimal price = Convert.ToDecimal(row["Price"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                dgvOrders.Rows.Add(
                    row["OrderID"],
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
            cmbProduct.Text = row.Cells["ProductName"].Value.ToString();
            numQuantity.Value = Convert.ToInt32(row.Cells["Quantity"].Value);
            txtPrice.Text = row.Cells["Price"].Value.ToString();
            cmbStatus.Text = row.Cells["Status"].Value.ToString();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Order order = new Order
                {
                    ProductID = (int)cmbProduct.SelectedValue,
                    ProductName = cmbProduct.Text,
                    Quantity = (int)numQuantity.Value,
                    Price = decimal.Parse(txtPrice.Text),
                    Status = cmbStatus.Text,
                    OrderDate = DateTime.Now
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
                Order order = new Order
                {
                    OrderID = selectedOrderId,
                    ProductID = (int)cmbProduct.SelectedValue,
                    ProductName = cmbProduct.Text,
                    Quantity = (int)numQuantity.Value,
                    Price = decimal.Parse(txtPrice.Text),
                    Status = cmbStatus.Text,
                    OrderDate = DateTime.Now
                };

                OrderManager.UpdateOrder(order);
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

            var confirm = MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                OrderManager.DeleteOrder(selectedOrderId);
                LoadOrders();
                ClearInputs();
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void ClearInputs()
        {
            selectedOrderId = -1;
            cmbProduct.SelectedIndex = 0;
            numQuantity.Value = 1;
            txtPrice.Clear();
            cmbStatus.SelectedIndex = 1;
        }
    }
}
