using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MAUI.ViewModels
{
    public class SidePanelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private bool paneVisible;
        private double leftMargin;
        public double LeftMargin
        {
            set
            {
                this.leftMargin = value;
                MessagingCenter.Send<SidePanelViewModel, double>(this, "MarginUpdated", LeftMargin);
            }

            get
            {
                return this.leftMargin;
            }
        }

        public bool PaneVisible
        {
            set
            {
                this.paneVisible = value;
                OnPropertyChanged(nameof(PaneVisible));
            }

            get
            {
                return this.paneVisible;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SidePanelViewModel()
        {
            PaneVisible = true;
            LeftMargin = 257;
        }
    }
}
