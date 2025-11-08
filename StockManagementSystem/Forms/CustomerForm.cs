using StockManagementSystem.Classes;
using System;
using System.Data;
using System.Windows.Forms;

namespace StockManagementSystem.Forms
{
    public partial class CustomerForm : Form
    {
        // Field to store the ID of the currently selected customer. Nullable int.
        private int? selectedCustomerId = null;

        public CustomerForm()
        {
            InitializeComponent();

            // Attach button click events
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnSearch.Click += btnSearch_Click;

            // DataGridView settings
            dgvCustomers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCustomers.MultiSelect = false;
            dgvCustomers.ReadOnly = true;
            dgvCustomers.AllowUserToAddRows = false;

            // Keep DGV at bottom (assuming these UI components exist)
            // Note: These lines might need adjustment based on your actual form designer setup.
            // dgvCustomers.Dock = DockStyle.Bottom;
            // dgvCustomers.Height = 150; 

            // Handle row click
            dgvCustomers.CellClick += dgvCustomers_CellClick;
        }

        // --- Event Handler for Form Load ---
        private void CustomerForm_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        // --- Data Loading Method ---
        private void LoadCustomers()
        {
            try
            {
                DataTable dt = CustomerManager.GetAllCustomers();

                dgvCustomers.DataSource = null;
                dgvCustomers.Columns.Clear();
                dgvCustomers.DataSource = dt;

                // Adjust ID column width
                if (dgvCustomers.Columns.Contains("CustomerID"))
                    dgvCustomers.Columns["CustomerID"].Width = 50;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers: " + ex.Message);
            }
        }

        // --- Input Clearing Method ---
        private void ClearInputs()
        {
            // Assuming the TextBoxes are named txtName, txtAddress, txtPhone, txtEmail, txtSearch
            txtName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtSearch.Clear();
            // Crucially, reset the selected ID
            selectedCustomerId = null;
        }

        // --- DataGridView Cell Click (Selection) Handler ---
        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure a valid row index is clicked (not the header or an invalid row)
            if (e.RowIndex < 0 || e.RowIndex >= dgvCustomers.Rows.Count) return;

            DataGridViewRow row = dgvCustomers.Rows[e.RowIndex];

            // Safely set the selectedCustomerId and populate the text fields
            selectedCustomerId = Convert.ToInt32(row.Cells["CustomerID"].Value);
            txtName.Text = row.Cells["Name"].Value.ToString();
            txtAddress.Text = row.Cells["Address"].Value.ToString();
            txtPhone.Text = row.Cells["Phone"].Value.ToString();
            txtEmail.Text = row.Cells["Email"].Value.ToString();
        }

        // --- Button Click Handlers ---

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                return;
            }

            try
            {
                // Assuming CustomerManager.AddCustomer exists and handles DB logic
                CustomerManager.AddCustomer(name, address, phone, email);
                MessageBox.Show("Customer added successfully!");
                ClearInputs();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer: " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // *** CORRECTION APPLIED HERE: Check if a customer is selected ***
            if (!selectedCustomerId.HasValue)
            {
               
                return;
            }

            string name = txtName.Text.Trim();
            string address = txtAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Input validation for required fields
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Name and Address are required.");
                return;
            }

            try
            {
                // Assuming CustomerManager.UpdateCustomer exists and handles DB logic
                CustomerManager.UpdateCustomer(selectedCustomerId.Value, name, address, phone, email);
                MessageBox.Show("Customer updated successfully!");
                ClearInputs();
                LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // *** CORRECTION APPLIED HERE: Check if a customer is selected ***
            if (!selectedCustomerId.HasValue)
            {
             
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    // Assuming CustomerManager.DeleteCustomer exists and handles DB logic
                    CustomerManager.DeleteCustomer(selectedCustomerId.Value);
                    MessageBox.Show("Customer deleted successfully!");
                    ClearInputs();
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting customer: " + ex.Message);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearch.Text.Trim();
            try
            {
                DataTable dt;
                if (string.IsNullOrWhiteSpace(search))
                    dt = CustomerManager.GetAllCustomers();
                else
                    dt = CustomerManager.GetCustomerByName(search);

                dgvCustomers.DataSource = dt;

                if (dgvCustomers.Columns.Contains("CustomerID"))
                    dgvCustomers.Columns["CustomerID"].Width = 50;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching: " + ex.Message);
            }
        }

    }
}