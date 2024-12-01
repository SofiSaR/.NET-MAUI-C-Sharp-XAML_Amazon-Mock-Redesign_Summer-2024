using Library.Models;
using Library.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Transactions;
using System.Xml.Linq;

namespace PrelimConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string menuSelect = "empty";

            var productSvc = ProductServiceProxy.Current;
            var cartItemSvc = CartItemServiceProxy.Current;

            Console.WriteLine("Amazon\n\n");
            while (menuSelect.ToUpper() != "C")
            {
                Console.WriteLine("What are you looking to do today?\n");
                Console.WriteLine("a. Inventory Management\nb. Shop\nc. Close\n");
                menuSelect = Console.ReadLine() ?? "empty";

                switch (menuSelect.ToUpper())
                {
                    case "A": InventoryManagement(productSvc);
                        break;
                    case "B": Shop(productSvc, cartItemSvc);
                        break;
                    case "C": Console.WriteLine("Closing...");
                        break;
                    default: Console.WriteLine("Invalid input. Please enter a, b, or c.\n");
                        break;
                }
            }
        }

        static void InventoryManagement(ProductServiceProxy productSvc)
        {
            string action = "empty";

            while (!((productSvc?.Count > 0 && action.ToUpper() == "D") || (!(productSvc?.Count > 0) && action.ToUpper() == "B")))
            {
                Console.WriteLine("\nInventory Management\n\n");

                Console.WriteLine("Products\n");

                if (productSvc?.Count == 1)
                    Console.WriteLine($"{productSvc?.Count} product total\n");
                else
                    Console.WriteLine($"{productSvc?.Count} total products\n");

                if (productSvc?.Count > 0)
                    productSvc?.Products?.ToList()?.ForEach(Console.WriteLine);

                Console.WriteLine("\n");

                Console.Write("a. Add a Product     ");
                if (productSvc?.Count > 0)
                {
                    Console.Write("b. Update a Product     c. Delete a Product");
                    Console.WriteLine("\n\nd. < Exit Inventory Management\n");
                }
                else
                    Console.WriteLine("\n\nb. < Exit Inventory Management\n");

                action = Console.ReadLine() ?? "empty";

                if (action.ToUpper() == "A")
                {
                    Add(productSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "B" && productSvc?.Count > 0)
                {
                    Update(productSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "B" && !(productSvc?.Count > 0)) ;
                else if (action.ToUpper() == "C" && productSvc?.Count > 0)
                {
                    Delete(productSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "D" && productSvc?.Count > 0) ;
                else if (productSvc?.Count > 0)
                    Console.WriteLine("Invalid input. Please enter a, b, c, or d.\n");
                else
                    Console.WriteLine("Invalid input. Please enter a or b.\n");
            }
        }

        static void Add(ProductServiceProxy productSvc)
        {
            string confirmation = "empty";
            Console.WriteLine("\nAdd a Product\n\n");
            do
            {
                confirmation = "empty";
                Console.WriteLine("Enter all of the following information for the product you'd like to add:\n");
                string name = GetProdInfo("Name", "name");
                string description = GetProdInfo("Description", "description");
                string priceString = GetProdInfo("Price", "number, where any decimal places are preceded with a \".\"");
                double price = Math.Round(ParseToDouble(priceString, "Price", "number, where any decimal places are preceded with a \".\""), 2);
                string numInStockString = GetProdInfo("Inventory", "whole number");
                int numInStock = ParseToInt(numInStockString, "Inventory", "whole number");
                Console.WriteLine("\nHere is the info you provided:\n");
                Console.WriteLine($"{name}\n{description}\n${price:0.00}\n{numInStock} in stock\n");
                Console.WriteLine("Would you like to proceed with adding?\n");
                while (confirmation.ToUpper() != "A" && confirmation.ToUpper() != "B" && confirmation.ToUpper() != "C")
                {
                    Console.WriteLine("a. Add!\nb. Redo\nc. Cancel\n");
                    confirmation = Console.ReadLine() ?? "empty";

                    switch (confirmation.ToUpper())
                    {
                        case "A":
                            Product productAdded = productSvc?.Add(
                                new Product
                                {
                                    Name = name,
                                    Description = description,
                                    Price = price,
                                    Id = 0,
                                    NumInStock = numInStock
                                }
                            );
                            if (productAdded != null)
                                Console.WriteLine($"\nProduct saved.\n\n{productAdded}\n");
                            break;
                        case "B":
                            break;
                        case "C":
                            break;
                        default:
                            Console.WriteLine("Invalid input. Please enter a, b, or c.\n");
                            break;
                    }
                }
            } while (confirmation.ToUpper() == "B");
        }

        static void Update(ProductServiceProxy productSvc)
        {
            Product productToEdit = new Product();
            int id = 0;
            Console.WriteLine("\nUpdate Existing Product\n\n");
            bool foundProduct = false;
            while (foundProduct == false)
            {
                Console.WriteLine("Enter the Product No. of the product you would like to update:\n");
                string idString = GetProdInfo("Product No", "whole number");
                id = ParseToInt(idString, "Product No", "whole number");
                productToEdit = new Product(productSvc?.FindProduct(id));
                if (productToEdit.Name == null)
                    Console.WriteLine("We couldn't find a product with that Product No. Please try again.\n");
                else
                    foundProduct = true;
            }
            string attribute = "empty";
            Console.WriteLine("\nWe found your product!\n");
            while (attribute.ToUpper() != "E" && attribute.ToUpper() != "F")
            {
                Console.WriteLine($"\n{productToEdit}\n");
                Console.WriteLine("Select an attribute you'd like to change:\n");
                Console.WriteLine("a. Name\nb. Description\nc. Price\nd. Inventory\ne. Save\nf. Cancel\n");
                attribute = Console.ReadLine() ?? "empty";
                switch (attribute.ToUpper())
                {
                    case "A":
                        productToEdit.Name = GetProdInfo("Name", "name");
                        break;
                    case "B":
                        productToEdit.Description = GetProdInfo("Description", "description");
                        break;
                    case "C":
                        string priceString = GetProdInfo("Price", "number, where any decimal places are preceded with a \".\"");
                        productToEdit.Price = Math.Round(ParseToDouble(priceString, "Price", "number, where any decimal places are preceded with a \".\""), 2);
                        break;
                    case "D":
                        string numInStockString = GetProdInfo("Inventory", "whole number");
                        productToEdit.NumInStock = ParseToInt(numInStockString, "Inventory", "whole number");
                        break;
                    case "E":
                        Product savedProduct = productSvc?.CommitChanges(id, productToEdit);
                        Console.WriteLine($"\nYour changes have been saved.\n\n{savedProduct}");
                        break;
                    case "F":
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a, b, c, d, e, or f.\n");
                        break;
                }
            }
        }

        static void Delete(ProductServiceProxy productSvc)
        {
            string confirmation = "empty";
            Product productToDelete = new Product();
            int id = 0;
            Console.WriteLine("\nDelete Existing Product\n\n");
            bool foundProduct = false;
            while (foundProduct == false)
            {
                Console.WriteLine("Enter the Product No. of the product you would like to delete:\n");
                string idString = GetProdInfo("Product No", "whole number");
                id = ParseToInt(idString, "Product No", "whole number");
                productToDelete = new Product(productSvc?.FindProduct(id));
                if (productToDelete.Name == null)
                    Console.WriteLine("We couldn't find a product with that Product No. Please try again.\n");
                else
                    foundProduct = true;
            }
            Console.WriteLine($"\nWe found your product!\n\n{productToDelete}\n");
            Console.WriteLine("Would you like to proceed with deletion?\n");
            while (confirmation.ToUpper() != "A" && confirmation.ToUpper() != "B")
            {
                Console.WriteLine("a. Delete!\nb. Cancel\n");
                confirmation = Console.ReadLine() ?? "empty";

                switch (confirmation.ToUpper())
                {
                    case "A":
                        productSvc?.Delete(id);
                        Console.WriteLine("Product deleted.\n");
                        break;
                    case "B":
                        break;
                    default:
                        Console.WriteLine("Invalid input. Please enter a or b.\n");
                        break;
                }
            }
        }

        static string GetProdInfo(string infoField, string infoType)
        {
            string infoEntry = "empty";
            do
            {
                Console.WriteLine($"{infoField}: ");
                infoEntry = Console.ReadLine() ?? "empty";
                if (infoEntry == "empty" || infoEntry == "")
                    Console.WriteLine($"This is a required field. Please enter a {infoType}.\n");
            } while (infoEntry == "empty" || infoEntry == "");
            return infoEntry;
        }

        static int ParseToInt(string itemToParse, string infoField, string infoType)
        {
            int parsedItem;
            while (true)
            {
                if (int.TryParse(itemToParse, out int value) && value >= 0)
                {
                    parsedItem = value;
                    break;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a positive {infoType}.\n");
                    itemToParse = GetProdInfo(infoField, infoType);
                }
            }
            return parsedItem;
        }
        
        static double ParseToDouble(string itemToParse, string infoField, string infoType)
        {
            double parsedItem;
            while (true)
            {
                if (double.TryParse(itemToParse,out double value) && value >= 0)
                {
                    parsedItem = value;
                    break;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a positive {infoType}.\n");
                    itemToParse = GetProdInfo(infoField, infoType);
                }
            }
            return parsedItem;
        }

        static void Shop(ProductServiceProxy productSvc, CartItemServiceProxy cartItemSvc)
        {
            string action = "empty";

            while (!((action.ToUpper() == "A" && !(productSvc?.NumInventoryItems > 0) && !(cartItemSvc?.NumCartItems > 0))
                || (action.ToUpper() == "B" && productSvc?.NumInventoryItems > 0 && !(cartItemSvc?.NumCartItems > 0))
                || (action.ToUpper() == "C" && !(productSvc?.NumInventoryItems > 0) && cartItemSvc?.NumCartItems > 0)
                || (action.ToUpper() == "D" && productSvc?.NumInventoryItems > 0 && cartItemSvc?.NumCartItems > 0)))
            {
                Console.WriteLine("\nShop\n\n");

                Console.WriteLine("Products\n");

                if (productSvc?.Count == 1)
                    Console.WriteLine($"{productSvc?.Count} product total\n");
                else
                    Console.WriteLine($"{productSvc?.Count} total products\n");

                if (productSvc?.Count > 0)
                    productSvc?.Products?.ToList()?.ForEach(Console.WriteLine);

                Console.WriteLine("\nMy Cart\n");

                Console.Write($"{cartItemSvc?.NumCartItems} item");
                if (cartItemSvc?.NumCartItems != 1)
                    Console.Write("s");
                Console.Write("\n\n");

                if (cartItemSvc?.NumCartItems > 0)
                    cartItemSvc?.CartItems?.ToList()?.ForEach(Console.WriteLine);

                Console.WriteLine("\n");

                if (productSvc?.NumInventoryItems > 0)
                {
                    Console.Write("a. Add to Cart     ");
                    if (cartItemSvc?.NumCartItems > 0)
                    {
                        Console.Write("b. Remove from Cart\n");
                        Console.WriteLine("\nc. Checkout");
                        Console.WriteLine("\n\nd. < Exit Shop\n");
                    }
                    else
                        Console.WriteLine("\n\nb. < Exit Shop\n");
                }
                else
                {
                    Console.WriteLine("No products in stock.\n");
                    if (cartItemSvc?.NumCartItems > 0)
                    {
                        Console.Write("a. Remove from Cart\n");
                        Console.WriteLine("\nb. Checkout");
                        Console.WriteLine("\nc. < Exit Shop\n");
                    }
                    else
                        Console.WriteLine("\na. < Exit Shop\n");
                }

                action = Console.ReadLine() ?? "empty";

                if (action.ToUpper() == "A" && productSvc?.NumInventoryItems > 0)
                {
                    AddToCart(productSvc, cartItemSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "A" && !(productSvc?.NumInventoryItems > 0) && cartItemSvc?.NumCartItems > 0)
                {
                    Remove(productSvc, cartItemSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "A" && !(productSvc?.NumInventoryItems > 0) && !(cartItemSvc?.NumCartItems > 0)) ;
                else if (action.ToUpper() == "B" && productSvc?.NumInventoryItems > 0 && cartItemSvc?.NumCartItems > 0)
                {
                    Remove(productSvc, cartItemSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "B" && productSvc?.NumInventoryItems > 0 && !(cartItemSvc?.NumCartItems > 0)) ;
                else if (action.ToUpper() == "B" && !(productSvc?.NumInventoryItems > 0) && cartItemSvc?.NumCartItems > 0)
                {
                    Checkout(cartItemSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "C" && productSvc?.NumInventoryItems > 0 && cartItemSvc?.NumCartItems > 0)
                {
                    Checkout(cartItemSvc);
                    action = "empty";
                }
                else if (action.ToUpper() == "C" && !(productSvc?.NumInventoryItems > 0) && cartItemSvc?.NumCartItems > 0) ;
                else if (action.ToUpper() == "D" && productSvc?.NumInventoryItems > 0 && cartItemSvc?.NumCartItems > 0) ;
                else if (productSvc?.NumInventoryItems > 0 && cartItemSvc?.NumCartItems > 0)
                    Console.WriteLine("Invalid input. Please enter a, b, c, or d.\n");
                else if (!(productSvc?.NumInventoryItems > 0) && cartItemSvc?.NumCartItems > 0)
                    Console.WriteLine("Invalid input. Please enter a, b, or c.\n");
                else if (productSvc?.NumInventoryItems > 0 && !(cartItemSvc?.NumCartItems > 0))
                    Console.WriteLine("Invalid input. Please enter a or b.\n");
                else
                    Console.WriteLine("Invalid input. Please enter \"a\" to exit.\n");
            }
        }

        static void AddToCart(ProductServiceProxy productSvc, CartItemServiceProxy cartItemSvc)
        {
            Product productToAdd = new Product();
            CartItem newCartItem = new CartItem();
            int id = 0;
            int quantity = 0;
            bool foundProduct = false;
            while (foundProduct == false)
            {
                Console.WriteLine("Enter the Product No. of the product you would like to add to your cart:\n");
                string idString = GetProdInfo("Product No", "whole number");
                id = ParseToInt(idString, "Product No", "whole number");
                productToAdd = new Product(productSvc?.FindProduct(id));
                if (productToAdd.Name == null)
                    Console.WriteLine("We couldn't find a product with that Product No. Please try again.\n");
                else if (productToAdd.NumInStock == 0)
                {
                    Console.WriteLine("Product is out of stock.\n");
                    foundProduct = true;
                }
                else
                    foundProduct = true;
            }
            if (productToAdd.NumInStock > 0)
            {
                Console.WriteLine($"{productToAdd.NumInStock} in stock\n");
                Console.WriteLine("Enter the quantity you would like to add to your cart: ");
                string quantityString = GetProdInfo("Quantity", "whole number");
                quantity = ParseToInt(quantityString, "Quantity", "whole number");
                if (quantity != 0)
                {
                    if (productToAdd.NumInStock < quantity)
                        Console.WriteLine("\nNot enough in stock.\n");
                    else
                    {
                        newCartItem = new CartItem(quantity, productToAdd);
                        productSvc?.RemoveInventory(id, quantity);
                        cartItemSvc?.Add(newCartItem);
                        Console.WriteLine("Added to Cart!\n");
                    }
                }
            }
        }

        static void Remove(ProductServiceProxy productSvc, CartItemServiceProxy cartItemSvc)
        {
            CartItem itemToRemove = new CartItem();
            int id = 0;
            int quantity = 0;
            bool foundItem = false;
            while (foundItem == false)
            {
                Console.WriteLine("Enter the Product No. of the product you would like to remove from your cart:\n");
                string idString = GetProdInfo("Product No", "whole number");
                id = ParseToInt(idString, "Product No", "whole number");
                itemToRemove = new CartItem(cartItemSvc?.FindCartItem(id));
                if (itemToRemove.Name == null)
                    Console.WriteLine("We couldn't find an item with that Product No. Please try again.\n");
                else
                    foundItem = true;
            }
            Console.WriteLine($"Quantity: {itemToRemove.Quantity}\n");
            Console.WriteLine("Enter the quantity you would like to remove from your cart: ");
            string quantityString = GetProdInfo("Quantity", "whole number");
            quantity = ParseToInt(quantityString, "Quantity", "whole number");
            if (itemToRemove.Quantity < quantity)
                Console.WriteLine($"\nThe quantity you entered ({quantity}) is more than the quantity in your cart.\n");
            else
            {
                productSvc?.AddInventory(id, quantity);
                cartItemSvc?.Remove(id, quantity);
                Console.WriteLine($"\nSuccessfully removed {quantity} of {itemToRemove.Name} from cart.\n");
            }
        }

        static void Checkout(CartItemServiceProxy cartItemSvc)
        {
            double subtotal = cartItemSvc?.Subtotal ?? 0, 
                taxrate = 0.07,
                tax = Math.Round(subtotal * taxrate, 2),
                total = subtotal + tax;
            Console.WriteLine("\nOrder Summary\n");

            Console.Write($"{cartItemSvc?.NumCartItems} item");
            if (cartItemSvc?.NumCartItems != 1)
                Console.Write("s");
            Console.Write("\n\n");

            if (cartItemSvc?.NumCartItems > 0)
                cartItemSvc?.CartItems?.ToList()?.ForEach(Console.WriteLine);

            Console.WriteLine($"Subtotal: ${subtotal:0.00}\n");
            Console.WriteLine($"Taxes ({Math.Round(taxrate * 100, 1)}%): ${tax:0.00}\n");
            Console.WriteLine($"Total: ${total:0.00}\n");
            cartItemSvc?.ClearCart();
            Console.WriteLine("\nPress any key to\n");
            Console.WriteLine("< Continue Shopping\n");
            Console.ReadKey();
        }
    }
}
