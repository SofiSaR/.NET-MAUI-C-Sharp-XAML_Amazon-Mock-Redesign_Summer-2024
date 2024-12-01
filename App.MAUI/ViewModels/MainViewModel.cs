using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MAUI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private Color? shopperButtonColor;
        private Color? sellerButtonColor;
        private string? pageToGo;
        public Color? ShopperButtonColor
        {
            set
            {
                this.shopperButtonColor = value;
                OnPropertyChanged(nameof(ShopperButtonColor));
            }

            get
            {
                return this.shopperButtonColor;
            }
        }

        public Color? SellerButtonColor
        {
            set
            {
                this.sellerButtonColor = value;
                OnPropertyChanged(nameof(SellerButtonColor));
            }

            get
            {
                return this.sellerButtonColor;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string? PageToGo
        {
            set
            {
                this.pageToGo = value;
            }

            get
            {
                return this.pageToGo;
            }
        }
        public MainViewModel()
        {
            PageToGo = "//ShopHome";
            ShopperButtonColor = (Color)Application.Current.Resources["Secondary"];
            SellerButtonColor = (Color)Application.Current.Resources["Transparent"];
        }
    }
}
