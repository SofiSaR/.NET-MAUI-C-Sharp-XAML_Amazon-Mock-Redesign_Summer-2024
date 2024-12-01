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


namespace Library.Services
{
    public class CartItemServiceProxy : INotifyPropertyChanged
    {
        public CartItemServiceProxy() {
            cartItems = new List<CartItem>();
            cartName = "";
            checkedTrue = false;
        }

        public CartItemServiceProxy(string name)
        {
            if (name != null)
            {
                cartItems = new List<CartItem>();
                cartName = name;
                checkedTrue = false;
            }
        }

        public CartItemServiceProxy(CartItemServiceProxy cart)
        {
            if (cart != null)
            {
                cartItems = cart.cartItems;
                cartName = cart.cartName;
                checkedTrue = cart.checkedTrue;
            }
        }

        private static CartItemServiceProxy? instance;
        private static object instanceLock = new object();
        private string? cartName;
        private bool checkedTrue;

        public bool Checked
        {
            set
            {
                checkedTrue = value;
                MessagingCenter.Send<CartItemServiceProxy, CartItemServiceProxy>(this, "CheckboxClicked", this);
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

        public static CartItemServiceProxy Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CartItemServiceProxy();
                    }
                }
                return instance;
            }
        }

        private List<CartItem>? cartItems;

        public ReadOnlyCollection<CartItem>? CartItems
        {
            get
            {
                return cartItems?.AsReadOnly();
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

        public double Subtotal
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
