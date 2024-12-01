using Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Library.Models;

namespace App.MAUI.ViewModels
{
    public class ProductManagementViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public List<ProductViewModel> Products
        {
            get
            {
                return ProductCollection.Current?.Products?.Select(p => new ProductViewModel(p)).ToList() ?? new List<ProductViewModel>();
            }
        }
        public ProductViewModel SelectedProduct { get; set; }

        public ProductViewModel SelectedForCart { get; set; }

        public CartItemViewModel SelectedCartItem { get; set; }

        public List<CartItemViewModel> CartItems
        {
            get
            {
                return ShoppingCart.Current?.CartItems?.Select(p => new CartItemViewModel(p)).ToList() ?? new List<CartItemViewModel>();
            }
        }

        public void RefreshProducts()
        {
            MessagingCenter.Send<ProductManagementViewModel, string>(this, "ProductsUpdated", TaxRate);
        }

        public void RefreshCartItems()
        {
            MessagingCenter.Send<ProductManagementViewModel>(this, "CartUpdated");
        }

        public void UpdateProduct()
        {
            Shell.Current.GoToAsync($"//ProductEdit?productId={SelectedProduct.Product?.Id}");
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

        public string NumCartItems
        {
            set
            {
                this.numCartItems = value;
                OnPropertyChanged(nameof(NumCartItems));
            }
            get
            {
                return this.numCartItems;
            }
        }

        public string CartCount
        {
            set
            {
                this.cartCount = value;
                OnPropertyChanged(nameof(CartCount));
            }
            get
            {
                return this.cartCount;
            }
        }

        public int CartCountNum
        {
            set
            {
                this.cartCountNum = value;
                OnPropertyChanged(nameof(CartCountNum));
            }
            get
            {
                return this.cartCountNum;
            }
        }

        public bool CartNotEmpty
        {
            set
            {
                this.cartNotEmpty = value;
                OnPropertyChanged(nameof(CartNotEmpty));
            }
            get
            {
                return this.cartNotEmpty;
            }
        }

        public string SubTotal
        {
            set
            {
                this.subTotal = ParseToDouble(value);
                this.taxNum = Math.Round(this.subTotal * ProductCollection.Current.TaxRate, 2);
                this.TaxText = $"Tax ({Math.Round(ProductCollection.Current.TaxRate * 100, 2)}%):";
                this.Tax = $"${Math.Round(taxNum, 2):0.00}";
                this.Total = $"${Math.Round(subTotal+taxNum,2):0.00}";
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
                    RefreshProducts();
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


        private bool shopperSidePanelVisible;
        public bool ShopperSidePanelVisible
        {
            set
            {
                this.shopperSidePanelVisible = value;
                OnPropertyChanged(nameof(ShopperSidePanelVisible));
            }

            get
            {
                return this.shopperSidePanelVisible;
            }
        }

        private Thickness margins;

        public Thickness Margins
        {
            set
            {
                this.margins = value;
                OnPropertyChanged(nameof(Margins));
            }
            get
            {
                return this.margins;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProductManagementViewModel()
        {
            CartVisible = false;
            Margins = new Thickness(257 + 30, 30, 0, 0);
            TaxRate = $"{ProductCollection.Current.TaxRate}";
            MessagingCenter.Subscribe<SidePanelViewModel, double>(this, "MarginUpdated", (sender, margin) =>
            {
                Margins = new Thickness(margin + 30, 30, 0, 0);
            });
            MessagingCenter.Subscribe<ProductManagementViewModel>(this, "CartUpdated", (sender) =>
            {
                OnPropertyChanged(nameof(CartItems));
                CartCountNum = ShoppingCart.Current.NumCartItems;
                NumCartItems = $"{CartCountNum}";
                if (CartCountNum == 0)
                {
                    CartCount = $"{NumCartItems} items";
                    CartNotEmpty = false;
                }
                else if (CartCountNum == 1)
                {
                    CartCount = $"{NumCartItems} item";
                    CartNotEmpty = true;
                }
                else
                {
                    CartCount = $"{NumCartItems} items";
                    CartNotEmpty = true;
                }
                SubTotal = $"{ShoppingCart.Current.SubtotalNum}";
                TaxRate = $"{ProductCollection.Current.TaxRate}";
            });
            MessagingCenter.Subscribe<ProductManagementViewModel, string>(this, "ProductsUpdated", (sender, taxR8) =>
            {
                TaxRate = taxR8;
                OnPropertyChanged(nameof(Products));
            });
            MessagingCenter.Subscribe<object>(this, "HeartUpdate", (message) =>
            {
                System.Diagnostics.Debug.WriteLine("Entered HeartUpdate Receiver");
                var data = (dynamic)message;
                bool productInAWishlist = data.ProductInAWishlist;
                int clickedId = data.ClickedId;
                int wishlistedProductIndex = Products.FindIndex(c => c.Id == clickedId.ToString());
                System.Diagnostics.Debug.WriteLine($"ProductInAWishlist: {productInAWishlist}");
                System.Diagnostics.Debug.WriteLine($"ClickedID: {clickedId}");
                System.Diagnostics.Debug.WriteLine($"WishlistedProductIndex: {wishlistedProductIndex}");
                Products[wishlistedProductIndex].Wishlisted = productInAWishlist;
                RefreshProducts();
            });
        }
    }
}
