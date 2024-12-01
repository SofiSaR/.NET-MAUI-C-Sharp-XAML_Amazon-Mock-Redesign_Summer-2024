using Library.Models;
using Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Product = Library.Models.Product;
using CartItem = Library.Models.CartItem;


namespace App.MAUI.ViewModels
{
    public class ProductViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
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

        public ICommand AddToWishlistCommand { get; private set; }

        public ICommand? EditCommand { get; private set; }

        public ICommand? DeleteCommand { get; private set; }

        public ICommand? BuyCommand { get; private set; }


        public Product? Product;

        public string? Id
        {
            get
            {
                return $"{Product?.Id}";
            }
            set
            {
                if (Product != null)
                {
                    Product.Id = ParseToInt(value);
                }
            }
        }

        public string? Name
        {
            get
            {
                return Product?.Name ?? string.Empty;
            }
            set
            {
                if(Product != null)
                {
                    Product.Name = value;
                }
            }
        }

        public string? TrimmedName
        {
            get
            {
                return Product?.Name?.Length <= 60 ? Product?.Name : Product?.Name?.Substring(0, 60) + "...";
            }
        }

        public string? TrimmedDescription
        {
            get
            {
                return Product?.Description?.Length <= 48 ? Product?.Description : Product?.Description?.Substring(0, 48) + "...";
            }
        }

        public string? Description
        {
            get
            {
                return Product?.Description ?? string.Empty;
            }
            set
            {
                if (Product != null)
                {
                    Product.Description = value;
                }
            }
        }

        public string? DisplayPrice
        {
            get
            {
                double salePrice = Math.Round(Product.Price - Product.Markdown, 2);
                return $"${salePrice:0.00}";
            }
        }

        public string? Price
        {
            get
            {
                return $"{Product?.Price:0.00}";
            }
            set
            {
                if (Product != null)
                {
                    Product.Price = Math.Round(ParseToDouble(value ?? string.Empty),2);
                }
            }
        }

        public bool HasMarkdown
        {
            get
            {
                if (Product?.Markdown != 0)
                    return true;
                return false;
            }
        }

        public string? DisplayMarkdown
        {
            get
            {
                return $"-${Math.Round(Product.Markdown, 2):0.00}";
            }
        }

        public string? Markdown
        {
            get
            {
                return $"{Product?.Markdown}";
            }
            set
            {
                if (Product != null)
                {
                    Product.Markdown = ParseToDouble(value);
                }
            }
        }

        public bool IsBOGO
        {
            get
            {
                return Product.IsBOGO;
            }
            set
            {
                if (Product != null)
                {
                    Product.IsBOGO = value;
                    OnPropertyChanged(nameof(Product.IsBOGO));
                }
            }
        }

        public string? NumInStock
        {
            get
            {
                return $"{Product?.NumInStock}";
            }
            set
            {
                if (Product != null)
                {
                    Product.NumInStock = ParseToInt(value);
                    OnPropertyChanged(nameof(Product.NumInStock));
                }
            }
        }

        public string CartButtonText
        {
            get
            {
                if (Product.NumInStock == 0)
                    return "Out of Stock";
                else return "Add to Cart";
            }
        }

        public Color CartButtonColor
        {
            get
            {
                if (Product.NumInStock == 0)
                    return (Color)Application.Current.Resources["PrimaryLight"];
                else return (Color)Application.Current.Resources["SignatureBlue"];
            }
        }

        public bool Wishlisted
        {
            get
            {
                return Product.Wishlisted;
            }
            set
            {
                Product.Wishlisted = value;
            }
        }

        public ImageSource HeartIconSource
        {
            get
            {
                if (Product.Wishlisted)
                    return "heart_filled.png";
                else
                    return "heart_outline.png";
            }
        }

        private int ParseToInt(string itemToParse)
        {
            int parsedItem;
            if (int.TryParse(itemToParse, out int value) && value >= 0)
            {
                parsedItem = value;
                return parsedItem;
            }
            return 0;
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

        private void ExecuteAddToWishlist(ProductViewModel? p)
        {
            if (p?.Product == null)
                return;
            MessagingCenter.Send<ProductViewModel, Product>(this, "HeartClicked", p?.Product);
        }

        private void ExecuteEdit(ProductViewModel? p)
        {
            if (p?.Product == null)
                return;
            Shell.Current.GoToAsync($"//ProductEdit?productId={p.Product.Id}");
        }

        private void ExecuteDelete(ProductViewModel? p)
        {
            if (p?.Product == null)
                return;
            ProductCollection.Current.Delete(p.Product.Id);
            ShoppingCart.Current.Delete(p.Product.Id);
        }

        private void ExecuteAddToCart(ProductViewModel? p)
        {
            if (p?.Product == null || p?.Product.NumInStock == 0)
                return;
            CartItem newCartItem = new CartItem(1, p.Product);
            ShoppingCart.Current.Add(newCartItem);
            ProductCollection.Current.RemoveInventory(p.Product.Id, 1);
        }

        public void SetupCommands()
        {
            AddToWishlistCommand = new Command((p) => ExecuteAddToWishlist(p as ProductViewModel));
            EditCommand = new Command((p) => ExecuteEdit(p as ProductViewModel));
            DeleteCommand = new Command((p) => ExecuteDelete(p as ProductViewModel));
            BuyCommand = new Command((p) => ExecuteAddToCart(p as ProductViewModel));
        }

        public void Add()
        {
            ProductCollection.Current.Add(Product);
        }

        public void Update()
        {
            ProductCollection.Current.CommitChanges(Product.Id, Product);
            ShoppingCart.Current.Update(Product);
        }

        public ProductViewModel()
        {
            Product = new Product();
            SetupCommands();
            Margins = new Thickness(257 + 70, 50, 70, 50);
            MessagingCenter.Subscribe<SidePanelViewModel, double>(this, "MarginUpdated", (sender, margin) =>
            {
                Margins = new Thickness(margin + 70, 50, 70, 50);
            });
        }

        public ProductViewModel(int id)
        {
            Product = new Product(ProductCollection.Current?.Products?.FirstOrDefault(p => p.Id == id));
            if (Product == null)
                Product = new Product();
            SetupCommands();
            Margins = new Thickness(257 + 70, 50, 70, 50);
            MessagingCenter.Subscribe<SidePanelViewModel, double>(this, "MarginUpdated", (sender, margin) =>
            {
                Margins = new Thickness(margin + 70, 50, 70, 50);
            });
        }

        public ProductViewModel(Product p)
        {
            Product = p;
            SetupCommands();
            Margins = new Thickness(257 + 70, 50, 70, 50);
            MessagingCenter.Subscribe<SidePanelViewModel, double>(this, "MarginUpdated", (sender, margin) =>
            {
                Margins = new Thickness(margin + 70, 50, 70, 50);
            });
        }

        public string? Display
        {
            get
            {
                return ToString();
            }
        }
    }
}
