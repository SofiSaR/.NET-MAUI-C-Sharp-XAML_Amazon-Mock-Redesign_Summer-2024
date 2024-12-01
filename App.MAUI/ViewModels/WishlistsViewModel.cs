using Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = Library.Models.Product;
using Library.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace App.MAUI.ViewModels
{
    public class WishlistsViewModel : INotifyPropertyChanged
    {

        public WishlistsViewModel()
        {
            ShowWishlistSelectDialog = false;
            if (Carts != null && Carts?.Count > 0)
            {
                SelectedCart = Carts[0];
            }
            else
                SelectedCart = new ShoppingCart();
            MessagingCenter.Subscribe<ProductViewModel, Product>(this, "HeartClicked", (sender, clickedItem) =>
            {
                ClickedItem = clickedItem;
                CheckCheckedWishlists();
                RefreshCarts();
                foreach (ShoppingCart cart in Carts)
                {
                    System.Diagnostics.Debug.WriteLine(cart.CartName);
                    System.Diagnostics.Debug.WriteLine(cart.FindCartItem(ClickedItem.Id) != null);
                }
                ShowWishlistSelectDialog = true;
            });
            MessagingCenter.Subscribe<ShoppingCart, ShoppingCart>(this, "CheckboxClicked", (sender, cart) =>
            {
                Shopper.Current.EditCart(cart);
            });
        }

        public List<ShoppingCart> Carts
        {
            get
            {
                return Shopper.Current?.Carts?.Select(p => new ShoppingCart(p)).ToList() ?? new List<ShoppingCart>();
            }
            set
            {
                Shopper.Current.Carts = value;
            }
        }

        private ShoppingCart selectedCart;

        public ShoppingCart SelectedCart
        {
            get
            {
                return ShoppingCart.SelectedWishlist;
            }
            set
            {
                System.Diagnostics.Debug.WriteLine("Setting SelectedCart");
                ShoppingCart.SelectedWishlist.SetCart(value);
                OnPropertyChanged(nameof(SelectedCart));
                OnPropertyChanged(nameof(SelectedCartItems));
            }
        }

        public List<CartItemViewModel> SelectedCartItems
        {
            get
            {
                return ShoppingCart.SelectedWishlist.CartItems?.Select(p => new CartItemViewModel(p)).ToList() ?? new List<CartItemViewModel>();
            }
        }


        private Product? clickedItem;

        public Product? ClickedItem
        {
            get
            {
                return clickedItem;
            }
            set
            {
                clickedItem = value;
                OnPropertyChanged(nameof(ClickedItem));
            }
        }


        private bool showWishlistSelectDialog;

        public bool ShowWishlistSelectDialog
        {
            get
            {
                return this.showWishlistSelectDialog;
            }
            set
            {
                this.showWishlistSelectDialog = value;
                OnPropertyChanged(nameof(ShowWishlistSelectDialog));
            }
        }


        private bool addingWishlist;

        public bool AddingWishlist
        {
            get
            {
                return addingWishlist;
            }
            set
            {
                addingWishlist = value;
                OnPropertyChanged(nameof(AddingWishlist));
            }
        }


        private string? nameToAdd;

        public string NameToAdd
        {
            get
            {
                return nameToAdd;
            }
            set
            {
                nameToAdd = value;
            }
        }


        public void CheckCheckedWishlists()
        {
            System.Diagnostics.Debug.WriteLine("Checking");
            for (int i = 0; i < Shopper.Current.Carts.Count; i++)
            {
                if (Shopper.Current.Carts[i].FindCartItem(ClickedItem.Id) != null)
                    Shopper.Current.Carts[i].Checked = true;
                else Shopper.Current.Carts[i].Checked = false;
            }
        }

        public void AddWishlist()
        {
            ShoppingCart newWishlist = new ShoppingCart(NameToAdd);
            Shopper.Current?.AddCart(newWishlist);
            NameToAdd = "";
            OnPropertyChanged(nameof(NameToAdd));
            AddingWishlist = false;
            RefreshCarts();
        }

        public void RefreshCarts()
        {
            if (SelectedCart == null && Carts != null && Carts?.Count > 0)
                SelectedCart = Carts[0];
            System.Diagnostics.Debug.WriteLine("Carts Refreshing");
            ShoppingCart.SelectedWishlist.SubTotal = $"{ShoppingCart.SelectedWishlist.SubtotalNum}";
            ShoppingCart.SelectedWishlist.TaxRate = $"{ProductCollection.Current.TaxRate}";
            OnPropertyChanged(nameof(SelectedCart));
            OnPropertyChanged(nameof(SelectedCartItems));
            OnPropertyChanged(nameof(Carts));
        }

        public void AddToCarts()
        {
            System.Diagnostics.Debug.WriteLine("Entered AddToCarts()");
            bool productInAWishlist = false;
            foreach (ShoppingCart cart in Carts)
            {
                System.Diagnostics.Debug.WriteLine("Entered first loop");
                System.Diagnostics.Debug.WriteLine($"{cart.CartName}");
                System.Diagnostics.Debug.WriteLine($"{cart.Checked}");
                System.Diagnostics.Debug.WriteLine($"{cart.FindCartItem(ClickedItem.Id) != null}");
                if (cart.Checked)
                {
                    productInAWishlist = true;
                }
                if (cart.Checked && cart.FindCartItem(ClickedItem.Id) == null)
                {
                    CartItem cartItem = new CartItem(1, ClickedItem);
                    cart.Add(cartItem);
                }
                else if (!cart.Checked && cart.FindCartItem(ClickedItem.Id) != null)
                    cart.Delete(ClickedItem.Id);
            }
            foreach (ShoppingCart cart in Carts)
            {
                foreach (CartItem cartItem in cart.CartItems)
                {
                    System.Diagnostics.Debug.WriteLine($"{cartItem.Name} is in {cart.CartName}");
                }
            }
            System.Diagnostics.Debug.WriteLine("Got to the end of AddToCarts");
            var heartUpdateParams = new { ProductInAWishlist = productInAWishlist, ClickedId = ClickedItem.Id };
            MessagingCenter.Send<object>(heartUpdateParams, "HeartUpdate");
            ShowWishlistSelectDialog = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
