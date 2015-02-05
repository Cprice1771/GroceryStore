using System;

namespace GroceryStore
{
    public class Item
    {

        #region Public properties
        /// <summary>
        /// The Item's SKU
        /// </summary>
        public int SKU { get; set; }
        /// <summary>
        /// How many we recevied of the item in total
        /// </summary>
        public int QuantityReceived { get; set; }
        /// <summary>
        /// How many we currently have of the item
        /// </summary>
        public int CurrentStock { get; set; }
        /// <summary>
        /// The date we recevied the item
        /// </summary>
        public DateTime DateRecieved { get; set; }
        /// <summary>
        /// The Brand of the item
        /// </summary>
        public string Brand { get; set; }
        /// <summary>
        /// What the item is
        /// </summary>
        public string Description { get; set; }
        #endregion

        /// <summary>
        /// Constructor to make a new Item
        /// </summary>
        /// <param name="brand">Brand of the item</param>
        /// <param name="description">Description of the item, what it is</param>
        /// <param name="sku">The item's SKU number</param>
        /// <param name="quantityReceived">How many we've received of the item</param>
        /// <param name="currentStock">How many we currently have in stock</param>
        /// <param name="dateReceived">The date we received the item</param>
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
