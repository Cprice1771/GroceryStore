using System;
using System.Collections.Generic;
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
    public partial class Item
    {

        #region Public properties
        public int SKU { get; set; }
        public int QuantityReceived { get; set; }
        public int CurrentStock { get; set; }
        public DateTime DateRecieved { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        #endregion

        public Item(string brand, string description, int sku, int quantityReceived, int currentStock, DateTime dateReceived)
        {
            Brand = brand;
            Description = description;
            SKU = sku;
            QuantityReceived = quantityReceived;
            CurrentStock = currentStock;
            DateRecieved = dateReceived;
        }
    }
}
