using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class CartItem
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Id { get; set; }
        public int Quantity { get; set; }
        public bool IsBOGO { get; set; }
        public double Markdown { get; set; }

        public string? Display
        {
            get
            {
                return ToString();
            }
        }

        public CartItem()
        {

        }

        public CartItem(CartItem cartItemToCopy)
        {
            if (cartItemToCopy != null)
            {
                Name = cartItemToCopy.Name;
                Description = cartItemToCopy.Description;
                Price = cartItemToCopy.Price;
                Id = cartItemToCopy.Id;
                Quantity = cartItemToCopy.Quantity;
                IsBOGO = cartItemToCopy.IsBOGO;
                Markdown = cartItemToCopy.Markdown;
            }
        }

        public CartItem(int quantity, Product productToCopy)
        {
            if (productToCopy != null)
            {
                Name = productToCopy.Name;
                Description = productToCopy.Description;
                Price = productToCopy.Price;
                Id = productToCopy.Id;
                Quantity = quantity;
                IsBOGO = productToCopy.IsBOGO;
                Markdown = productToCopy.Markdown;
            }
        }

        public override string ToString()
        {
            return $"Product No. {Id}\n{Name}\n{Description}\n${Price:0.00}\nQuantity: {Quantity}\n\n";
        }
    }
}
