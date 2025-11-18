using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using StockManagementSystem.Classes;
using StockManagementSystem.Forms;

namespace StockManagementSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Stock Management System - Dashboard";
            LoadDashboardCounts();
            LoadStockQuantityChart();
            StyleDashboard();
        }
        private void LoadDashboardCounts()
        {
            try
            {
                int products = ProductManager.GetTotalProducts();
                int orders = OrderManager.GetTotalOrders();
                int customers = CustomerManager.GetTotalCustomers();

                lblProducts.Text = products.ToString();
                lblOrders.Text = orders.ToString();
                lblCustomer.Text = customers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading dashboard: " + ex.Message);
                lblProducts.Text = "0";
                lblOrders.Text = "0";
                lblCustomer.Text = "0";
            }
        }

        private void StyleDashboard()
        {
            // Make labels big and bold
            Font bigFont = new Font("Segoe UI", 24F, FontStyle.Bold);
            Font mediumFont = new Font("Segoe UI", 12F, FontStyle.Regular);

            lblProducts.Font = bigFont;
            lblProducts.ForeColor = Color.DarkOrange;
            lblProducts.TextAlign = ContentAlignment.MiddleCenter;

            lblOrders.Font = bigFont;
            lblOrders.ForeColor = Color.DarkOrange;
            lblOrders.TextAlign = ContentAlignment.MiddleCenter;

            lblCustomer.Font = bigFont;
            lblCustomer.ForeColor = Color.DarkOrange;
            lblCustomer.TextAlign = ContentAlignment.MiddleCenter;

            // Optional: Add small labels below
            Label lblProdText = new Label { Text = "Total Products", Font = mediumFont, ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter };
            Label lblOrderText = new Label { Text = "Total Orders", Font = mediumFont, ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter };
            Label lblCustText = new Label { Text = "Total Customers", Font = mediumFont, ForeColor = Color.Gray, TextAlign = ContentAlignment.MiddleCenter };

            // Adjust location based on your lblProducts, etc.
            // Example (adjust as needed):
            lblProdText.Location = new Point(lblProducts.Left, lblProducts.Bottom + 5);
            lblOrderText.Location = new Point(lblOrders.Left, lblOrders.Bottom + 5);
            lblCustText.Location = new Point(lblCustomer.Left, lblCustomer.Bottom + 5);

            this.Controls.Add(lblProdText);
            this.Controls.Add(lblOrderText);
            this.Controls.Add(lblCustText);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //this one is btnManageCategories
            new CategoryForm().ShowDialog();
            LoadDashboardCounts();
        }

        private void btnManageProducts_Click(object sender, EventArgs e)
        {
            new ProductForm().ShowDialog();
            LoadDashboardCounts();
        }

        private void btnManageCustomers_Click(object sender, EventArgs e)
        {
            new CustomerForm().ShowDialog();
            LoadDashboardCounts();
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            new UserForm().ShowDialog();
            LoadDashboardCounts();
        }

        private void btnManageOrders_Click(object sender, EventArgs e)
        {
            new OrderForm().ShowDialog();
            LoadDashboardCounts();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblCustomer_Click(object sender, EventArgs e)
        {

        }

        private void lblOrders_Click(object sender, EventArgs e)
        {

        }

        private void lblProducts_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        private void LoadStockQuantityChart()
        {
            // Safely find the chart control (works even if inside panels, tab pages, etc.)
            Chart chartControl = this.Controls.Find("chart1", true).FirstOrDefault() as Chart
                              ?? this.Controls.Find("chartStock", true).FirstOrDefault() as Chart
                              ?? this.Controls.Find("chartStockLevels", true).FirstOrDefault() as Chart;

            if (chartControl == null)
            {
                MessageBox.Show("Chart control not found! Please name your Chart control 'chart1' in the designer.",
                                "Missing Chart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataTable dt = ProductManager.GetAllProducts();
                if (dt.Rows.Count == 0)
                {
                    chartControl.Titles.Clear();
                    chartControl.Titles.Add("No products yet");
                    chartControl.Titles[0].Font = new Font("Segoe UI", 16F, FontStyle.Italic);
                    chartControl.Titles[0].ForeColor = Color.Gray;
                    return;
                }

                // === BEAUTIFUL RESET ===
                chartControl.Series.Clear();
                chartControl.ChartAreas.Clear();
                chartControl.Titles.Clear();
                chartControl.Legends.Clear();
                chartControl.Annotations.Clear();

                // === Chart Area Styling ===
                ChartArea chartArea = new ChartArea("MainArea");
                chartArea.BackColor = Color.FromArgb(245, 248, 252);
                chartArea.BorderDashStyle = ChartDashStyle.Solid;
                chartArea.BorderColor = Color.FromArgb(220, 230, 241);
                chartArea.BorderWidth = 1;

                // Axis styling
                chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
                chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartArea.AxisX.LabelStyle.Font = new Font("Segoe UI", 10F);
                chartArea.AxisX.LabelStyle.Angle = -30;
                chartArea.AxisX.Interval = 1;

                chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(230, 230, 230);
                chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
                chartArea.AxisY.LabelStyle.Font = new Font("Segoe UI", 10F);
                chartArea.AxisY.Title = "Quantity in Stock";
                chartArea.AxisY.TitleFont = new Font("Segoe UI", 12F, FontStyle.Bold);

                chartControl.ChartAreas.Add(chartArea);

                // === Title ===
                Title title = new Title("Current Stock Levels", Docking.Top, new Font("Segoe UI", 18F, FontStyle.Bold), Color.FromArgb(33, 37, 41));
                title.ForeColor = Color.FromArgb(52, 58, 64);
                chartControl.Titles.Add(title);

                // === Series (Column Bars) ===
                Series series = new Series("StockQuantity")
                {
                    ChartType = SeriesChartType.Column,
                    XValueMember = "Name",
                    YValueMembers = "QuantityInStock",
                    IsValueShownAsLabel = true,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    LabelForeColor = Color.White,
                    BorderWidth = 3,
                    BorderColor = Color.White,
                    ["PointWidth"] = "0.8"  // Makes bars thicker and nicer
                };

                chartControl.Series.Add(series);
                chartControl.DataSource = dt;
                chartControl.DataBind();

                // === BEAUTIFUL COLORING + LOW STOCK ALERT ===
                int lowStockCount = 0;
                foreach (DataPoint point in series.Points)
                {
                    int qty = Convert.ToInt32(point.YValues[0]);

                    if (qty <= 3)
                    {
                        lowStockCount++;
                        point.Color = Color.FromArgb(220, 53, 69);        // Deep Red
                        point.BackGradientStyle = GradientStyle.TopBottom;
                        point.BackSecondaryColor = Color.FromArgb(255, 99, 132);
                        point.Label = $"LOW STOCK! ({qty})";
                        point.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                        point.LabelForeColor = Color.White;
                    }
                    else if (qty <= 10)
                    {
                        point.Color = Color.FromArgb(255, 193, 7);        // Warning Amber
                        point.BackSecondaryColor = Color.FromArgb(255, 205, 86);
                        point.BackGradientStyle = GradientStyle.TopBottom;
                        point.Label = qty.ToString();
                    }
                    else
                    {
                        point.Color = Color.FromArgb(40, 167, 69);         // Success Green
                        point.BackSecondaryColor = Color.FromArgb(25, 135, 84);
                        point.BackGradientStyle = GradientStyle.TopBottom;
                        point.LabelForeColor = Color.White;
                        point.Label = qty.ToString();
                    }

                    // Shadow effect
                    point["PixelPointWidth"] = "50";
                }

                // === Legend ===
                Legend legend = new Legend("Legend");
                legend.Docking = Docking.Bottom;
                legend.Alignment = StringAlignment.Center;
                legend.Font = new Font("Segoe UI", 10F);
                legend.ForeColor = Color.FromArgb(73, 80, 87);
                chartControl.Legends.Add(legend);

                // === Low Stock Warning Banner ===
                if (lowStockCount > 0)
                {
                    TextAnnotation warning = new TextAnnotation
                    {
                        Text = $"⚠ {lowStockCount} product{(lowStockCount > 1 ? "s" : "")} with LOW STOCK (≤ 3)",
                        ForeColor = Color.White,
                        BackColor = Color.FromArgb(220, 53, 69),
                        Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                        AxisX = chartArea.AxisX,
                        AxisY = chartArea.AxisY,
                        X = 2,
                        Y = 95,
                        Alignment = ContentAlignment.MiddleLeft,
                        
                        ShadowOffset = 3
                    };
                    chartControl.Annotations.Add(warning);
                }

                // Optional: Make chart background elegant
                chartControl.BackColor = Color.FromArgb(248, 249, 250);
                chartControl.BorderlineDashStyle = ChartDashStyle.Solid;
                chartControl.BorderlineColor = Color.FromArgb(220, 220, 220);
                chartControl.BorderlineWidth = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chart: " + ex.Message, "Chart Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ProductForm().ShowDialog();
            LoadDashboardCounts();
            LoadStockQuantityChart();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            new OrderForm().ShowDialog();
            LoadDashboardCounts();
            LoadStockQuantityChart();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
