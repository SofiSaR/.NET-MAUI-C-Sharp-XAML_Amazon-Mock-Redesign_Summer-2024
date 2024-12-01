using Library.Models;
using Library.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MAUI.ViewModels
{
    public class Shopper
    {
        private Shopper()
        {
            carts = new List<ShoppingCart>();
        }

        private List<ShoppingCart>? carts;

        public List<ShoppingCart>? Carts
        {
            get
            {
                return carts;
            }
            set
            {
                carts = value;
            }
        }

        private static Shopper? instance;
        private static object instanceLock = new object();

        public static Shopper Current
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Shopper();
                    }
                }
                return instance;
            }
        }

        public ShoppingCart? AddCart(ShoppingCart cart)
        {
            carts?.Add(cart);
            return cart;
        }

        public ShoppingCart? EditCart(ShoppingCart cart)
        {
            var matchingCart = carts.FindIndex(c => c.CartName == cart.CartName);
            carts[matchingCart] = cart;
            return carts[matchingCart];
        }
    }
}
