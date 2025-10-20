using StockManagementSystem.Classes;
using System;
using System.Data;
using System.Windows.Forms;

namespace StockManagementSystem.Forms
{
    public partial class ProductForm : Form
    {
        private int selectedProductId = -1;

        public ProductForm()
        {
            InitializeComponent();

            // Make sure DataGridView auto-generates columns
            dgvProducts.AutoGenerateColumns = true;
            dgvProducts.AutoGenerateColumns = true;

            dgvProducts.BackgroundColor = System.Drawing.Color.White; // grid background
            dgvProducts.DefaultCellStyle.BackColor = System.Drawing.Color.White; // row background
            dgvProducts.DefaultCellStyle.ForeColor = System.Drawing.Color.Black; // text color
            dgvProducts.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.LightBlue; // selected row
            dgvProducts.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black; // selected text
            dgvProducts.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvProducts.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;

            // Event handlers
            dgvProducts.CellClick += dgvProducts_CellClick;
            btnAdd.Click += btnAdd_Click;
            btnUpdate.Click += btnUpdate_Click;
            btnDelete.Click += btnDelete_Click;
            btnClear.Click += btnClear_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnSearch.Click += btnSearch_Click;
        }

        private void ProductForm_Load(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                dgvProducts.DataSource = ProductManager.GetAllProducts();
                CleanUpDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CleanUpDataGridView()
        {
            try
            {
                if (dgvProducts.Columns.Contains("ProductID"))
                    dgvProducts.Columns["ProductID"].Visible = false;

                if (dgvProducts.Columns.Contains("ProductName"))
                    dgvProducts.Columns["ProductName"].HeaderText = "Product Name";

                if (dgvProducts.Columns.Contains("Category"))
                    dgvProducts.Columns["Category"].HeaderText = "Category";

                if (dgvProducts.Columns.Contains("Quantity"))
                    dgvProducts.Columns["Quantity"].HeaderText = "Quantity";

                if (dgvProducts.Columns.Contains("Price"))
                    dgvProducts.Columns["Price"].HeaderText = "Price";

                dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DataGridView cleanup error: {ex.Message}");
            }
        }

        // --- CRUD Operations ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateFields(out int quantity, out decimal price)) return;

            Product product = new Product
            {
                ProductName = txtProductName.Text,
                Category = txtCategory.Text,
                Quantity = quantity,
                Price = price
            };

            string result = ProductManager.AddProduct(product);
            if (result == "OK")
            {
                MessageBox.Show("Product added successfully!");
                ClearFields();
                LoadProducts();
            }
            else
            {
                MessageBox.Show(result, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedProductId <= 0)
            {
                MessageBox.Show("Please select a product to update.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateFields(out int quantity, out decimal price)) return;

            Product product = new Product
            {
                ProductID = selectedProductId,
                ProductName = txtProductName.Text,
                Category = txtCategory.Text,
                Quantity = quantity,
                Price = price
            };

            if (ProductManager.UpdateProduct(product))
            {
                MessageBox.Show("Product updated successfully!");
                ClearFields();
                LoadProducts();
            }
            else
            {
                MessageBox.Show("Failed to update product.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedProductId <= 0)
            {
                MessageBox.Show("Please select a product to delete.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                if (ProductManager.DeleteProduct(selectedProductId))
                {
                    MessageBox.Show("Product deleted successfully!");
                    ClearFields();
                    LoadProducts();
                }
                else
                {
                    MessageBox.Show("Failed to delete product.", "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
                LoadProducts();
            else
            {
                try
                {
                    dgvProducts.DataSource = ProductManager.SearchProducts(keyword);
                    CleanUpDataGridView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Search failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvProducts.Rows.Count) return;

            DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

            // Set the selected product ID
            if (row.Cells["ProductID"].Value != DBNull.Value)
                selectedProductId = Convert.ToInt32(row.Cells["ProductID"].Value);
            else
                selectedProductId = -1;

            txtProductName.Text = row.Cells["ProductName"].Value?.ToString() ?? "";
            txtCategory.Text = row.Cells["Category"].Value?.ToString() ?? "";
            txtQuantity.Text = row.Cells["Quantity"].Value?.ToString() ?? "";
            txtPrice.Text = row.Cells["Price"].Value?.ToString() ?? "";
        }

        // --- Helpers ---
        private void ClearFields()
        {
            txtProductName.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            txtPrice.Clear();
            txtSearch.Clear();
            selectedProductId = -1;
        }

        private bool ValidateFields(out int quantity, out decimal price)
        {
            quantity = 0;
            price = 0;

            if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text) ||
                string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out quantity) ||
                !decimal.TryParse(txtPrice.Text, out price))
            {
                MessageBox.Show("Quantity and Price must be valid numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
