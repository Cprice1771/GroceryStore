using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GroceryStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Item> items = new List<Item>();
        private const int SQL_DUPLICATE_PRIMARY_KEY_ERROR_NUM = 2627;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitInventory()
        {
            stockDataGrid.ItemsSource = null;
            items.Clear();
            stackPanelOverview.Children.Clear();
            using (SqlConnection conn =
                    new SqlConnection(
                        @"Data Source=(LocalDB)\v11.0;AttachDbFilename=E:\projects\VS2012\GroceryStore\GroceryStore\Inventory.mdf;Integrated Security=True")
                )
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select * From Stock";

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                items.Add(new Item(reader.GetString(1), reader.GetString(5),
                                    reader.GetInt32(0), reader.GetInt32(3),
                                    reader.GetInt32(4), reader.GetDateTime(2)));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error connecting to the database!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);

                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }

            stockDataGrid.ItemsSource = items;
        }

        private void AddItem(int SKU, string description, string brand, int quantityReceived, int currentStock,
            DateTime dateReceived)
        {
            using (var conn =
                    new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=E:\projects\VS2012\GroceryStore\GroceryStore\Inventory.mdf;Integrated Security=True"))
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "INSERT INTO Stock (SKU, Brand, DateReceived, QuantityReceived, CurrentStock, ItemDescription) VALUES (@SKU, @Brand, @DateReceived, @QuantityReceived, @CurrentStock, @ItemDescription)";
                    cmd.Parameters.AddWithValue("@SKU", SKU);
                    cmd.Parameters.AddWithValue("@ItemDescription", description);
                    cmd.Parameters.AddWithValue("@Brand", brand);
                    cmd.Parameters.AddWithValue("@QuantityReceived", quantityReceived);
                    cmd.Parameters.AddWithValue("@CurrentStock", currentStock);
                    cmd.Parameters.AddWithValue("@DateReceived", dateReceived);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqlException e)
                    {
                        //TODO: just add to the current stock ??
                        if (e.Number == SQL_DUPLICATE_PRIMARY_KEY_ERROR_NUM)
                        {
                            MessageBox.Show("A Product with this SKU already exists!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error connecting to the database!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        }
                       
                    }
                    finally
                    {
                        conn.Close(); 
                    }
                }
            }
        }
        
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitInventory();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddItem(int.Parse(textBoxSKU.Text), textBoxDescription.Text, textBoxBrand.Text, int.Parse(textBoxQuantityReceived.Text), int.Parse(textBoxQuantityReceived.Text), DateTime.Parse(datePickerReceived.Text));
            }
            catch (FormatException)
            {

                MessageBox.Show("One of the inputs is invalid!", "Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
            }
        }
    }
}
