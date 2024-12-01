using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Library.Models;
using Microsoft.Maui.Controls;


namespace App.MAUI.ViewModels
{
    public class ShoppingCart : INotifyPropertyChanged
    {
        public ShoppingCart() {
            cartItems = new List<CartItem>();
            cartName = "";
            checkedTrue = false;
        }

        public ShoppingCart(string name)
        {
            if (name != null)
            {
                cartItems = new List<CartItem>();
                cartName = name;
                checkedTrue = false;
            }
        }

        public ShoppingCart(ShoppingCart cart)
        {
            if (cart != null)
            {
                cartItems = cart.cartItems;
                cartName = cart.cartName;
                checkedTrue = cart.checkedTrue;
            }
        }

        private static ShoppingCart? instance;
        private static object instanceLock = new object();
        private static ShoppingCart? wishlistInstance;
        private static object wishlistInstanceLock = new object();
        private string? cartName;
        private bool checkedTrue;

        public bool Checked
        {
            set
            {
                checkedTrue = value;
                MessagingCenter.Send<ShoppingCart, ShoppingCart>(this, "CheckboxClicked", this);
                OnPropertyChanged(nameof(Checked));
            }
            get
            {
                return checkedTrue;
            }
        }

        public bool CartNotEmpty
        {
            get
            {
                if (NumCartItems >= 1)
                    return true;
                return false;
            }
        }

        public string? CartCount
        {
            get
            {
                if (NumCartItems == 1)
                    return $"{NumCartItems} item";
                else
                    return $"{NumCartItems} items";
            }
        }

        public string? CartName
        {
            get
            {
                return cartName;
            }
            set
            {
                cartName = value;
            }
        }

        public static ShoppingCart Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ShoppingCart();
                    }
                }
                return instance;
            }
        }
        public static ShoppingCart SelectedWishlist
        {
            get
            {
                lock (wishlistInstanceLock)
                {
                    if (wishlistInstance == null)
                    {
                        wishlistInstance = new ShoppingCart();
                    }
                }
                return wishlistInstance;
            }
        }

        private List<CartItem>? cartItems;

        public List<CartItem>? CartItems
        {
            get
            {
                return cartItems;
            }
        }

        public void SetCart(ShoppingCart passedCart)
        {
            if (passedCart != null)
            {
                CartName = passedCart.CartName;
                cartItems = passedCart.CartItems;
                SubTotal = $"{SubtotalNum}";
                SubTotal = $"{SubtotalNum}";
                TaxRate = $"{TaxRate}";
            }
        }

        public int NumCartItems
        {
            get
            {
                if (cartItems == null)
                    return 0;
                int numCartItems = 0;
                foreach (CartItem item in cartItems)
                    numCartItems += item.Quantity;
                return numCartItems;
            }
        }

        public double SubtotalNum
        {
            get
            {
                if (cartItems == null)
                    return 0;
                double subtotal = 0;
                foreach (CartItem item in cartItems)
                {
                    double salePrice = Math.Round(item.Price - item.Markdown, 2);
                    if (item.IsBOGO)
                    {

                        subtotal += ((item.Quantity / 2) + (item.Quantity - ((item.Quantity / 2) * 2))) * salePrice;
                    }
                    else
                        subtotal += (item.Quantity) * salePrice;
                }
                return Math.Round(subtotal, 2);
            }
        }

        private string numCartItems;
        private string cartCount;
        private int cartCountNum;
        private double subTotal;
        private double taxNum;
        private string taxText;
        private string tax;
        private double taxRate;
        private string total;
        private bool cartNotEmpty;

        public string SubTotal
        {
            set
            {
                this.subTotal = ParseToDouble(value);
                this.taxNum = Math.Round(this.subTotal * ProductCollection.Current.TaxRate, 2);
                this.TaxText = $"Tax ({Math.Round(ProductCollection.Current.TaxRate * 100, 2)}%):";
                this.Tax = $"${Math.Round(taxNum, 2):0.00}";
                this.Total = $"${Math.Round(subTotal + taxNum, 2):0.00}";
                OnPropertyChanged(nameof(SubTotal));
            }
            get
            {
                return $"${this.subTotal:0.00}";
            }
        }

        public string Tax
        {
            set
            {
                this.taxText = value;
                OnPropertyChanged(nameof(Tax));

            }
            get
            {
                return this.taxText;
            }
        }

        public string TaxText
        {
            set
            {
                this.tax = value;
                OnPropertyChanged(nameof(TaxText));

            }
            get
            {
                return this.tax;
            }
        }

        public string TaxRate
        {
            set
            {
                if (ProductCollection.Current.TaxRate != ParseToDouble(value))
                {
                    ProductCollection.Current.TaxRate = ParseToDouble(value);
                    System.Diagnostics.Debug.WriteLine($"\nThe TaxRate is: {ProductCollection.Current.TaxRate}\n");
                    //RefreshProducts();
                }
            }
            get
            {
                return $"{ProductCollection.Current.TaxRate}";
            }
        }

        public string Total
        {
            set
            {
                this.total = value;
                OnPropertyChanged(nameof(Total));
            }
            get
            {
                return this.total;
            }
        }

        private double ParseToDouble(string itemToParse)
        {
            double parsedItem;
            if (double.TryParse(itemToParse, out double value) && value >= 0)
            {
                parsedItem = value;
                return parsedItem;
            }
            return 0;
        }

        private bool cartVisible;
        public bool CartVisible
        {
            set
            {
                this.cartVisible = value;
                OnPropertyChanged(nameof(CartVisible));
            }

            get
            {
                return this.cartVisible;
            }
        }

        public CartItem? FindCartItem(int id)
        {
            if (cartItems == null)
                return null;
            var cartItemFound = cartItems.FirstOrDefault(c => c.Id == id);
            if (cartItemFound != null)
                return cartItemFound;
            return null;
        }

        public CartItem? Update(Product product)
        {
            if (cartItems == null)
                return null;
            var matchingCartItem = cartItems.FindIndex(c => c.Id == product.Id);
            if (matchingCartItem != -1)
            {
                cartItems[matchingCartItem] = new CartItem(cartItems[matchingCartItem].Quantity, product);
                return cartItems[matchingCartItem];
            }
            else
            {
                return null;
            }
        }

        public CartItem? Add(CartItem cartItem)
        {
            if (cartItems == null)
                return null;
            var matchingCartItem = cartItems.FindIndex(c => c.Id == cartItem.Id);
            if (matchingCartItem != -1)
            {
                cartItems[matchingCartItem].Quantity += cartItem.Quantity;
                return cartItems[matchingCartItem];
            }
            else
            {
                cartItems.Add(cartItem);
                return cartItem;
            }
        }

        public void ClearCart()
        {
            for(int i = cartItems?.Count - 1 ?? -1; i >= 0; i--)
                Remove(cartItems[i].Id, cartItems[i].Quantity);
        }

        public void Remove(int id, int quantity)
        {
            if (cartItems == null)
                return;
            var itemToReduce = cartItems.FindIndex(c => c.Id == id);
            if (itemToReduce != -1 && (cartItems[itemToReduce].Quantity - quantity > 0))
                cartItems[itemToReduce].Quantity -= quantity;
            else if (itemToReduce != -1)
            {
                var itemToDelete = cartItems.FirstOrDefault(c => c.Id == id);
                if (itemToDelete != null)
                    cartItems.Remove(itemToDelete);
            }
        }

        public void Delete (int id)
        {
            if (cartItems == null)
                return;
            var cartItemToDelete = cartItems.FirstOrDefault(c => c.Id == id);
            if (cartItemToDelete != null)
                cartItems.Remove(cartItemToDelete);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
