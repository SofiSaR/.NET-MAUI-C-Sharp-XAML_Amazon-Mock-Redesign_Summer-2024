using Library.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CartItem = Library.Models.CartItem;
using Product = Library.Models.Product;


namespace App.MAUI.ViewModels
{
    public class CartItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand? EditCommand { get; private set; }

        public ICommand? DeleteCommand { get; private set; }

        public ICommand? ChangePickerVisibility {  get; private set; }

        public ICommand? WishlistDelete { get; private set; }


        public CartItem? CartItem;

        public string? Id
        {
            get
            {
                return $"{CartItem?.Id}";
            }
            set
            {
                if (CartItem != null)
                {
                    CartItem.Id = ParseToInt(value);
                }
            }
        }

        public int IdNum
        {
            get
            {
                return CartItem.Id;
            }
            set
            {
                CartItem.Id = value;
            }
        }

        public string? Name
        {
            get
            {
                return CartItem?.Name ?? string.Empty;
            }
            set
            {
                if (CartItem != null)
                {
                    CartItem.Name = value;
                }
            }
        }

        public string? TrimmedName
        {
            get
            {
                return CartItem?.Name?.Length <= 25 ? CartItem?.Name : CartItem?.Name?.Substring(0, 25) + "...";
            }
        }

        public string? TrimmedDescription
        {
            get
            {
                return CartItem?.Description?.Length <= 60 ? CartItem?.Description : CartItem?.Description?.Substring(0, 60) + "...";
            }
        }

        public string? Description
        {
            get
            {
                return CartItem?.Description ?? string.Empty;
            }
            set
            {
                if (CartItem != null)
                {
                    CartItem.Description = value;
                }
            }
        }

        public string? DisplayPrice
        {
            get
            {
                return $"${CartItem?.Price:0.00}";
            }
        }

        public bool HasMarkdown
        {
            get
            {
                if (CartItem?.Markdown != 0)
                    return true;
                return false;
            }
        }

        public string? DisplayMarkdown
        {
            get
            {
                return $"-${Math.Round(CartItem.Markdown, 2):0.00}";
            }
        }

        public string? Price
        {
            get
            {
                return $"{CartItem?.Price:0.00}";
            }
            set
            {
                if (CartItem != null)
                {
                    CartItem.Price = Math.Round(ParseToDouble(value ?? string.Empty), 2);
                }
            }
        }

        public string? Quantity
        {
            get
            {
                return $"{CartItem?.Quantity}";
            }
            set
            {
                if (CartItem != null)
                {
                    CartItem.Quantity = ParseToInt(value);
                }
            }
        }

        public bool IsBOGO
        {
            get
            {
                return CartItem.IsBOGO;
            }
        }

        public string? QuantityEdit
        {
            get
            {
                return $"{CartItem?.Quantity}";
            }
            set
            {
                if (CartItem != null)
                {
                    int difference = ParseToInt(value) - CartItem.Quantity;
                    if (difference > 0)
                    {
                        Product coorespondingProduct = ProductCollection.Current.FindProduct(CartItem.Id);
                        if (coorespondingProduct == null || coorespondingProduct.NumInStock == 0)
                            return;
                        CartItem newCartItem = new CartItem(difference, coorespondingProduct);
                        ShoppingCart.Current.Add(newCartItem);
                        ProductCollection.Current.RemoveInventory(coorespondingProduct.Id, difference);
                    }
                    else if (difference < 0)
                    {
                        Product coorespondingProduct = ProductCollection.Current.FindProduct(CartItem.Id);
                        if (coorespondingProduct == null)
                            return;
                        ShoppingCart.Current.Remove(CartItem.Id, -difference);
                        ProductCollection.Current.AddInventory(coorespondingProduct.Id, -difference);
                    }
                }
            }
        }

        public string? QuantityEditWishlist
        {
            get
            {
                return $"{CartItem?.Quantity}";
            }
            set
            {
                if (CartItem != null)
                {
                    int difference = ParseToInt(value) - CartItem.Quantity;
                    if (difference > 0)
                    {
                        Product coorespondingProduct = ProductCollection.Current.FindProduct(CartItem.Id);
                        if (coorespondingProduct == null || coorespondingProduct.NumInStock == 0)
                            return;
                        CartItem newCartItem = new CartItem(difference, coorespondingProduct);
                        ShoppingCart.SelectedWishlist.Add(newCartItem);
                        ProductCollection.Current.RemoveInventory(coorespondingProduct.Id, difference);
                    }
                    else if (difference < 0)
                    {
                        Product coorespondingProduct = ProductCollection.Current.FindProduct(CartItem.Id);
                        if (coorespondingProduct == null)
                            return;
                        ShoppingCart.SelectedWishlist.Remove(CartItem.Id, -difference);
                        ProductCollection.Current.AddInventory(coorespondingProduct.Id, -difference);
                    }
                }
            }
        }

        private List<int> quantityList;

        public List<int>? QuantityList
        {
            get
            {
                return quantityList;
            }
            set
            {
                quantityList = value;
            }
        }

        private bool pickerVisible;

        public bool PickerVisible
        {
            get
            {
                return pickerVisible;
            }
            set
            {
                pickerVisible = value;
                OnPropertyChanged(nameof(PickerVisible));

            }
        }

        private bool dropdownVisible;

        public bool DropdownVisible
        {
            get
            {
                return dropdownVisible;
            }
            set
            {
                dropdownVisible = value;
                OnPropertyChanged(nameof(DropdownVisible));
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

        private void ExecuteEdit(CartItemViewModel? p)
        {
            if (p?.CartItem == null)
                return;
            Shell.Current.GoToAsync($"//CartItemEdit?CartItemId={p.CartItem.Id}");
        }

        private void ExecuteDelete(CartItemViewModel? p)
        {
            if (p?.CartItem == null)
                return;
            ShoppingCart.Current.Delete(p.CartItem.Id);
            ProductCollection.Current.AddInventory(p.CartItem.Id, p.CartItem.Quantity);
        }

        private void ExecuteWishlistDelete(CartItemViewModel? p)
        {
            if (p?.CartItem == null)
                return;
            ShoppingCart.SelectedWishlist.Delete(p.CartItem.Id);
            ProductCollection.Current.AddInventory(p.CartItem.Id, p.CartItem.Quantity);
        }

        private void ShowPicker(CartItemViewModel? p)
        {
            PickerVisible = true;
            DropdownVisible = false;
        }

        public void SetupCommands()
        {
            EditCommand = new Command((p) => ExecuteEdit(p as CartItemViewModel));
            DeleteCommand = new Command((p) => ExecuteDelete(p as CartItemViewModel));
            ChangePickerVisibility = new Command((p) => ShowPicker(p as CartItemViewModel));
            WishlistDelete = new Command((p) => ExecuteWishlistDelete(p as CartItemViewModel));
        }

        public void Add()
        {
            ShoppingCart.Current.Add(CartItem);
        }

        public CartItemViewModel()
        {
            CartItem = new CartItem();
            SetQuantityList();
            SetupCommands();
        }

        public CartItemViewModel(int id)
        {
            CartItem = ShoppingCart.Current?.CartItems?.FirstOrDefault(p => p.Id == id);
            if (CartItem == null)
                CartItem = new CartItem();
            SetQuantityList();
            SetupCommands();
        }

        private void SetQuantityList()
        {
            Product coorespondingProduct = ProductCollection.Current.FindProduct(CartItem.Id);
            for (int i = 0; i <= (CartItem.Quantity + coorespondingProduct.NumInStock) && i <= 10; i++)
            {
                QuantityList.Add(i);
            }
        }

        public CartItemViewModel(CartItem p)
        {
            CartItem = p;
            QuantityList = new List<int>();
            PickerVisible = false;
            DropdownVisible = true;
            SetQuantityList();
            SetupCommands();
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

