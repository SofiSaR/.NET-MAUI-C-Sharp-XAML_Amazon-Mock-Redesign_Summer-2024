using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Models;


namespace App.MAUI.ViewModels
{
    public class ProductCollection
    {
        private ProductCollection() {
            products = new List<Product>
            {
                new Product {
                    Id = 1,
                    Name="Salicylic Acid + Tea Tree After Shave Lotion",
                    Description="Soothing lotion helps prevent razor bumps",
                    Price=7.99,
                    NumInStock=7,
                    IsBOGO=false,
                    Markdown=3
                },
                new Product {
                    Id = 2,
                    Name="Rejuvinating Night Moisturizer",
                    Description="Use this moisturizer at night to hydrate your skin and fade scarring",
                    Price=4.99,
                    NumInStock=11,
                    IsBOGO=true,
                    Markdown=0
                }
            };
        }

        private static ProductCollection? instance;
        private static object instanceLock = new object();

        public static ProductCollection Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductCollection();
                    }
                }
                return instance;
            }
        }

        private List<Product>? products;

        public ReadOnlyCollection<Product>? Products
        {
            get
            {
                return products?.AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                if (products == null)
                    return 0;
                return products.Count;
            }
        }

        public int NumInventoryItems
        {
            get
            {
                if (products == null)
                    return 0;
                int numInventoryItems = 0;
                foreach (Product item in products)
                    numInventoryItems += item.NumInStock ?? 0;
                return numInventoryItems;
            }
        }

        private double taxRate;

        public double TaxRate { get; set; }

        public int LastId
        {
            get
            {
                if (products?.Any() ?? false)
                {
                    return products?.Select(c => c.Id)?.Max() ?? 0;
                }
                return 0;
            }
        }

        public Product? FindProduct(int id)
        {
            if (products == null)
                return null;
            var productFound = products.FirstOrDefault(c => c.Id == id);
            if (productFound != null)
                return productFound;
            return null;
        }

        public Product? CommitChanges(int id, Product productToSave)
        {
            if (productToSave == null)
                return null;
            var productToChange = products.FindIndex(c => c.Id == id);
            if (productToChange != -1)
            {
                products[productToChange] = productToSave;
                return products[productToChange];
            }
            return null;
        }

        public void AddInventory(int id, int quantity)
        {
            if (products == null)
                return;
            var productToIncrease = products.FindIndex(c => c.Id == id);
            if (productToIncrease != -1)
                products[productToIncrease].NumInStock += quantity;
        }

        public Product? Add(Product product)
        {
            if (products == null)
                return null;
            product.Id = LastId + 1;
            products.Add(product);

            return product;
        }

        public void RemoveInventory(int id, int quantity)
        {
            if (products == null)
                return;
            var productToReduce = products.FindIndex(c => c.Id == id);
            if (productToReduce != -1)
            {
                if (products[productToReduce].NumInStock - quantity < 0)
                    products[productToReduce].NumInStock = 0;
                else
                    products[productToReduce].NumInStock -= quantity;
            }
        }

        public void Delete (int id)
        {
            if (products == null)
                return;
            var productToDelete = products.FirstOrDefault(c => c.Id == id);
            if (productToDelete != null)
                products.Remove(productToDelete);
        }
    }
}
