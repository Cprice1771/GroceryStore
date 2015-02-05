using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;


namespace GroceryStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        List<Item> items = new List<Item>();
        private const int SQL_DUPLICATE_PRIMARY_KEY_ERROR_NUM = 2627;

        public MainWindow()
        {
            InitializeComponent();

            InitInventory();
        }

        /// <summary>
        /// Initializes the main view of all inventory in the database
        /// </summary>
        private void InitInventory()
        {
            //Reset the current DataGridView to be empty
            stockDataGrid.ItemsSource = null;
            items.Clear();

            //Get a connection to the db
            using (SqlConnection conn =
                    new SqlConnection(
                        @"Data Source=(LocalDB)\v11.0;AttachDbFilename=E:\projects\VS2012\GroceryStore\GroceryStore\Inventory.mdf;Integrated Security=True")
                )
            {
                //create our sql command
                using (SqlCommand cmd = new SqlCommand())
                {
                    //Set the commands properties
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "Select * From Stock";

                    try
                    {
                        //Try and open a connection to the db
                        conn.Open();
                        //Execute the sql command and get the results
                        SqlDataReader reader = cmd.ExecuteReader();
                        //If we got anything
                        if (reader.HasRows)
                        {
                            //Read the next result
                            while (reader.Read())
                            {
                                //Add the item to the data grid view
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
                        //Close our connection
                        conn.Close();
                    }
                }
            }
            //Set our data grid view to our list of items
            stockDataGrid.ItemsSource = items;
        }

        /// <summary>
        /// Add an item to the DB
        /// </summary>
        /// <param name="sku">SKU of the item to add</param>
        /// <param name="description">Description of the item to add</param>
        /// <param name="brand">Brand of the item to add</param>
        /// <param name="quantityReceived">How many we received of the item</param>
        /// <param name="currentStock">How many we currently have in stock</param>
        /// <param name="dateReceived">What date did we get the shipment</param>
        private void AddItem(int sku, string description, string brand, int quantityReceived, int currentStock,
            DateTime dateReceived)
        {
            //Create our connection to the DB
            using (var conn =
                    new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=E:\projects\VS2012\GroceryStore\GroceryStore\Inventory.mdf;Integrated Security=True"))
            {
                //Create our SQL command
                using (var cmd = new SqlCommand())
                {
                    //If the item doesn;t already exist in the DB
                    if (!HasSKU(items, sku))
                    {
                        //Set our sql command properties
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "INSERT INTO Stock (SKU, Brand, DateReceived, QuantityReceived, CurrentStock, ItemDescription) VALUES (@SKU, @Brand, @DateReceived, @QuantityReceived, @CurrentStock, @ItemDescription)";
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        cmd.Parameters.AddWithValue("@ItemDescription", description);
                        cmd.Parameters.AddWithValue("@Brand", brand);
                        cmd.Parameters.AddWithValue("@QuantityReceived", quantityReceived);
                        cmd.Parameters.AddWithValue("@CurrentStock", currentStock);
                        cmd.Parameters.AddWithValue("@DateReceived", dateReceived);

                        try
                        {
                            //Open a connection to the db
                            conn.Open();
                            //Execute our command
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            //Oops a item with this SKU already exists
                            if (e.Number == SQL_DUPLICATE_PRIMARY_KEY_ERROR_NUM)
                            {
                                MessageBox.Show("A Product with this SKU already exists!", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }
                            //We go some other error connecting to the DB or executing the command
                            else
                            {
                                MessageBox.Show("Error connecting to the database!", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            }

                        }
                        finally
                        {
                            //Close our connection
                            conn.Close();
                        }
                    }
                    //If the item does exist just quantity recieved to the current stock and update all other information
                    else
                    {
                        Item item = GetBySKU(items, sku);
                        cmd.Connection = conn;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText =
                            "UPDATE Stock SET Brand = @Brand, DateReceived = @DateReceived, QuantityReceived = @QuantityReceived, CurrentStock = @CurrentStock, ItemDescription = @ItemDescription WHERE SKU = @SKU";
                        cmd.Parameters.AddWithValue("@SKU", sku);
                        cmd.Parameters.AddWithValue("@ItemDescription", description);
                        cmd.Parameters.AddWithValue("@Brand", brand);
                        cmd.Parameters.AddWithValue("@QuantityReceived", quantityReceived + item.QuantityReceived);
                        cmd.Parameters.AddWithValue("@CurrentStock", currentStock + item.CurrentStock);
                        cmd.Parameters.AddWithValue("@DateReceived", dateReceived);

                        try
                        {
                            //Open the connection to the db
                            conn.Open();
                            //Execute the command
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException)
                        {
                            //Oops we got an error connecting or executing the command, tell the user
                            MessageBox.Show("Error connecting to the database!", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                        }
                        finally
                        {
                            //Close our connection to the db
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns an item in a list by the items SKU
        /// </summary>
        /// <param name="myTtems">List of items to search</param>
        /// <param name="sku">SKU to look up</param>
        /// <returns>The item if it was found, null otherwise</returns>
        private Item GetBySKU(List<Item> myTtems, int sku)
        {
            foreach (Item item in myTtems)
            {
                if (item.SKU == sku)
                    return item;
            }

            return null;
        }

        /// <summary>
        /// Gets if an item with that SKU is in the list of items
        /// </summary>
        /// <param name="myItems">list of items to search</param>
        /// <param name="sku">SKU to search for</param>
        /// <returns>True if the SKU was found, false otherwise</returns>
        private bool HasSKU(List<Item> myItems, int sku)
        {
            foreach (Item item in myItems)
            {
                if (item.SKU == sku)
                    return true;
            }

            return false;
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
            //Update the list of items in the DB
            InitInventory();
        }
    }
}
