using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class Product
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public int Id { get; set; }
        public int? NumInStock { get; set; }
        public bool IsBOGO { get; set; }
        public double Markdown {  get; set; }
        public bool Wishlisted { get; set; }

        public Product()
        {

        }

        public Product(Product productToCopy)
        {
            if (productToCopy != null)
            {
                Name = productToCopy.Name;
                Description = productToCopy.Description;
                Price = productToCopy.Price;
                Id = productToCopy.Id;
                NumInStock = productToCopy.NumInStock;
                IsBOGO = productToCopy.IsBOGO;
                Markdown = productToCopy.Markdown;
                Wishlisted = productToCopy.Wishlisted;
            }
        }

        public override string ToString()
        {
            return $"Product No. {Id}\n{Name}\n{Description}\n${Price:0.00}\n{NumInStock} in stock\n\n";
        }
    }
}
