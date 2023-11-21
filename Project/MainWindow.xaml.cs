using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;


namespace Project
{
    // class of Products with relevant information/attributes such as name, description, price and image source
    public class Product
    {
        public string Name;
        public string Description;
        public decimal Price;
        public ImageSource Source;

        public Product(string name, string description, decimal price, string source)
        {
            this.Name = name;
            this.Description = description;
            this.Price = price;
            this.Source = new BitmapImage(new Uri(source, UriKind.Relative));
        }
    }

    public partial class MainWindow : Window
    {
        //an integer that keeps track of what percent discount we get, this needs to be declared here so we can access it from different points in our program
        public static int discountPercent;

        // text block for the receipt
        public static TextBlock receiptTextBlock;

        // text box for promo codes that grand various discounts
        public static TextBox discountBox;

        //this 
        Label totalSumLabel;

        public static decimal totalSum;

        StackPanel cartPanel;

        //Declaring Wrappanel so we can acces it from a method
        WrapPanel controlPanel;

        // An array of all the products available, loaded from "Products.csv".
        public static Product[] Products;

        // A shopping cart is a dictionary mapping a Product object to the number of copies of that product we have added.
        public static Dictionary<Product, int> Cart;

        public static Dictionary<string, int> Discounts;

        // We store product information in a CSV file in the project directory.
        public const string ProductFilePath = "Products.csv";

        // We store the saved shopping cart in a CSV file outside the project directory, because then it will not be overwritten everytime we start the program.
        public const string CartFilePath = @"C:\Windows\Temp\Cart.csv";

        public const string DiscountFilePath = "Discounts.csv";

        StackPanel receiptStack;

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Start()
        {
            Discounts = LoadDiscountCodes();
            Products = LoadProducts(ProductFilePath);

            // If we have a saved cart, load it first from the text file.
            if (File.Exists(CartFilePath))
            {
                Cart = LoadCart(Products, CartFilePath);
                MessageBox.Show("Your saved cart has been loaded.");
            }
            // Otherwise create an empty cart.
            else
            {
                Cart = new Dictionary<Product, int>();
            }

            // Window options
            Title = "Nik & Jörgen's Shop";
            Width = 1000;
            Height = 600;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Main grid divided into two, where first column is 3 parts
            Grid grid = new Grid();
            Content = grid;
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(3, GridUnitType.Star);
            grid.Margin = new Thickness(5);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            ScrollViewer productScroll = new ScrollViewer();
            productScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            grid.Children.Add(productScroll);
            Grid.SetRow(productScroll, 1);
            Grid.SetColumn(productScroll, 0);
            productScroll.CanContentScroll = true;

            WrapPanel productPanel = CreateProductPanel();
            productScroll.Content = productPanel;

            // Create panel for the cart, payment and reciept
            Grid cartPanel = CreateCartPanel();
            grid.Children.Add(cartPanel);
            Grid.SetRow(cartPanel, 0);
            Grid.SetColumn(cartPanel, 1);
            foreach (KeyValuePair<Product, int> pair in Cart)
            {
                AddToCartGrid(pair);
                totalSumLabel.Content = "Your total: " + totalSum;
            }
        }

        // Create the visible store product collection
        private WrapPanel CreateProductPanel()
        {
            controlPanel = new WrapPanel { Orientation = Orientation.Horizontal, };


            foreach (Product p in Products)
            {
                AddProduct(p);
            }

            return controlPanel;
        }
        private Grid CreateCartPanel()
        {
            // The main layout is a grid with two columns and four rows.
            // All rows are sized to their content ("auto") except the second row, which takes up all the remaining space.

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); //title 1
            grid.RowDefinitions.Add(new RowDefinition()); //items in our cart 2
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); //Total 3
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); //discount 4
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); //Button to pay 5
            RowDefinition c1 = new RowDefinition();
            c1.Height = new GridLength(150, GridUnitType.Pixel);
            grid.RowDefinitions.Add(c1);//reciept 5

            //border spanning our cart and reciept
            Border myBorder1 = new Border();
            myBorder1.Background = Brushes.LightGray;
            myBorder1.BorderBrush = Brushes.Black;
            myBorder1.BorderThickness = new Thickness(2);
            grid.Children.Add(myBorder1);
            Grid.SetColumn(myBorder1, 0);
            Grid.SetRow(myBorder1, 0);
            Grid.SetColumnSpan(myBorder1, 2);
            Grid.SetRowSpan(myBorder1, 6);

            // A text heading.
            TextBlock heading = new TextBlock
            {
                Text = "Cart",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                FontFamily = new FontFamily("Constantia"),
                FontSize = 20,
                TextAlignment = TextAlignment.Center
            };
            grid.Children.Add(heading);
            Grid.SetColumn(heading, 0);
            Grid.SetRow(heading, 0);
            Grid.SetColumnSpan(heading, 2);

            //Cart

            ScrollViewer cartScroll = new ScrollViewer();
            cartScroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            cartScroll.Padding = new Thickness(5);
            cartScroll.Margin = new Thickness(5);
            grid.Children.Add(cartScroll);
            Grid.SetRow(cartScroll, 1);
            Grid.SetColumn(cartScroll, 0);
            Grid.SetColumnSpan(cartScroll, 2);

            cartPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            cartScroll.Content = cartPanel;

            totalSumLabel = new Label
            {
                Content = "Your total: " + totalSum,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Bottom
            };
            grid.Children.Add(totalSumLabel);
            Grid.SetColumn(totalSumLabel, 2);
            Grid.SetRow(totalSumLabel, 2);

            Button clearCartButton = new Button
            {
                Content = "Clear cart",
                Margin = new Thickness(5)
            };
            grid.Children.Add(clearCartButton);
            Grid.SetColumn(clearCartButton, 0);
            Grid.SetRow(clearCartButton, 2);
            clearCartButton.Click += ClearCartButton_Click;

            Label promoLabel = new Label
            {
                Content = "Promo code:",
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            grid.Children.Add(promoLabel);
            Grid.SetRow(promoLabel, 3);
            Grid.SetColumn(promoLabel, 0);

            discountBox = new TextBox
            {
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            grid.Children.Add(discountBox);
            Grid.SetColumn(discountBox, 1);
            Grid.SetRow(discountBox, 3);

            Button paymentButton = new Button
            {
                 Content = "Pay",
                 Margin = new Thickness(5)
            };
            grid.Children.Add(paymentButton);
            Grid.SetColumn(paymentButton, 0);
            Grid.SetRow(paymentButton, 4);
            Grid.SetColumnSpan(paymentButton, 2);
            paymentButton.Click += PaymentButton_Click;

            ScrollViewer scroll = new ScrollViewer();
            scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scroll.Padding = new Thickness(5);
            scroll.Margin = new Thickness(5);
            grid.Children.Add(scroll);
            Grid.SetRow(scroll, 5);
            Grid.SetColumn(scroll, 0);
            Grid.SetColumnSpan(scroll, 2);

            receiptStack = new StackPanel
            {
                Width = 2000,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(5)
            };
            scroll.Content = receiptStack;

            return grid;
        }

        // When "Clear Cart" is clicked, all of the contents in the cart are removed
        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            cartPanel.Children.Clear();
            Cart.Clear();
            totalSum = 0;
            totalSumLabel.Content = "Your total: 0";
            if (File.Exists(CartFilePath))
            {
                File.Delete(CartFilePath);
            }
        }

        // When "Pay" is clicked, transaction process is initiated
        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            // clear the receipt so that a new one can appear
            receiptStack.Children.Clear();

            // if the cart is empty
            if (Cart.Count == 0)
            {
                // do nothing
                return;
            }
            // if the promo code entered is valid
            else if (Discounts.ContainsKey(discountBox.Text.ToUpper()))
            {
                // extract discount percentage and print some messages
                Discounts.TryGetValue(discountBox.Text.ToUpper(), out discountPercent);
                MessageBox.Show("The promo code " + discountBox.Text.ToUpper() + " gets you a " + discountPercent + "% discount!");
                MessageBox.Show("Transaction complete.");
            }
            // if no promo code has been entered
            else if (discountBox.Text.Length == 0)
            {
                MessageBox.Show("Transaction complete.");
            }
            // if the promo code entered is wrong print an error message
            else if (!Discounts.ContainsKey(discountBox.Text.ToUpper()))
            {
                MessageBox.Show("Invalid discount code!");
                return;
            }

            // prepare the receipt and its output information
            receiptTextBlock = new TextBlock();
            receiptTextBlock.FontFamily = new FontFamily("Consolas");
            receiptTextBlock.Text = CartToString();
            receiptStack.Children.Add(receiptTextBlock);

            // empty the text box for promo codes
            discountBox.Text = String.Empty;

            cartPanel.Children.Clear();
            Cart.Clear();
            totalSum = 0;
            totalSumLabel.Content = "Your total: 0";
            if (File.Exists(CartFilePath))
            {
                File.Delete(CartFilePath);
            }
        }

        // Create an image based on its file path
        private Image CreateImage(string filePath)
        {
            ImageSource source = new BitmapImage(new Uri(filePath, UriKind.Relative));
            Image image = new Image
            {
                Width = 100,
                Height = 100,
                Source = source,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };
            // A small rendering tweak to ensure maximum visual appeal.
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.HighQuality);
            return image;
        }

        // Read the CSV file "Products.csv" and create product objects from it.
        public static Product[] LoadProducts(string productPath)
        {
            // If the file doesn't exist, stop the program completely.
            if (!File.Exists(productPath))
            {
                MessageBox.Show(productPath + " does not exist, or has not been set to 'Copy Always'.");
                Environment.Exit(1);
            }

            // Create an empty list of products, then go through each line of the file to fill it.
            List<Product> products = new List<Product>();
            string[] lines = File.ReadAllLines(productPath);
            foreach (string line in lines)
            {
                try
                {
                    // First, split the line on commas (CSV means "comma-separated values").
                    string[] parts = line.Split(',');

                    // Then create a product with its values set to the different parts of the line.
                    Product p = new Product(parts[0], parts[1], decimal.Parse(parts[2]), parts[3]);
                    products.Add(p);
                }
                catch
                {
                    MessageBox.Show("Error when loading product.");
                }
            }

            // The method returns an array rather than a list (because the products are fixed after the program has started), so we need to convert it before returning.
            return products.ToArray();
        }

        // Read the discount codes from the CSV file "Discounts.csv" and create a dictionary with the codes and their discount percentages
        public static Dictionary<string, int> LoadDiscountCodes()
        {
            Dictionary<string, int> Discount = new Dictionary<string, int>();
            string[] lines = File.ReadAllLines(DiscountFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string code = parts[0];
                int percent = int.Parse(parts[1]);

                Discount.Add(code, percent);
            }
            return Discount;
        }

        // Save the shopping cart to "Cart.csv".
        public static void SaveCart(Dictionary<Product, int> cart, string cartPath)
        {
            // Create an empty list of text lines that we will fill with strings and then write to a textfile using `WriteAllLines`.
            List<string> lines = new List<string>();
            foreach (KeyValuePair<Product, int> pair in cart)
            {
                Product p = pair.Key;
                int amount = pair.Value;

                // For each product, we only save the name and the amount.
                // The other info (source, price, description) is already in "Products.csv" and we can look it up when we load the cart.
                lines.Add(p.Name + "," + amount);
            }
            File.WriteAllLines(cartPath, lines);
        }

        // Build a string describing the contents of the shopping cart.
        public static string CartToString()
        {
            string discountCode = "";

            decimal discount = 1 - ((decimal)discountPercent / 100);

            decimal discountAmount = (decimal)discountPercent / 100;

            // Create an empty string to build from.
            string s = "";

            // Loop over the products in the cart (a dictionary) and add their info to the string while also calculating the total sum.
            decimal sum = 0;
            foreach (KeyValuePair<Product, int> pair in Cart)
            {
                Product p = pair.Key;
                int amount = pair.Value;

                // The number to add to the sum is this product's price multiplied by the number of copies we added.
                sum += p.Price * amount;
                

                s += amount + "x " + p.Name + "  (" + p.Price + ":-/x)  " + p.Price * amount + ":-\n";
                
            }

            // Based on whether a promo code has been entered or not, print specific corresponding information to the string
            if (discountBox.Text == String.Empty)
            {
                s += "\nTotal sum: " + sum + ":-";
            }
            else if (Discounts.ContainsKey(discountBox.Text.ToUpper()))
            {
                Discounts.TryGetValue(discountBox.Text.ToUpper(), out discountPercent);
                discountCode += discountBox.Text.ToUpper();

                s += "\nDiscount Code: " + discountCode + "\nSum: " + sum + ":-" + "\nDiscount: " + discountAmount * sum + ":- (" + discountPercent + "%)" + "\nTotal sum: " + sum * discount + ":-";
            }

            return s;
        }

        

        // Load a saved cart from "Cart.csv". This method is similar to `LoadProducts` but with some notable differences.
        public static Dictionary<Product, int> LoadCart(Product[] products, string cartPath)
        {
            // A cart is a dictionary (as described earlier), so create an empty one to fill as we read the CSV file.
            Dictionary<Product, int> savedCart = new Dictionary<Product, int>();

            // Go through each line and split it on commas, as in `LoadProducts`.
            string[] lines = File.ReadAllLines(cartPath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                string name = parts[0];
                int amount = int.Parse(parts[1]);

                // We only store the product's code in the CSV file, but we need to find the actual product object with that code.
                // To do this, we access the static `products` variable and find the one with the matching code, then grab that product object.
                Product current = null;
                foreach (Product p in products)
                {
                    if (p.Name == name)
                    {
                        current = p;
                        totalSum += p.Price * amount;
                    }
                }
                
                // Now that we have the product object (and not just the code), we can save it in the dictionary.
                savedCart[current] = amount;
            }

            return savedCart;
        }

        // Add a product to the store collection
        private Grid AddProduct(Product p)
        {
            Grid productGrid = new Grid();
            controlPanel.Children.Add(productGrid);
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.ColumnDefinitions.Add(new ColumnDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            productGrid.RowDefinitions.Add(new RowDefinition());
            var image = CreateImage(@"Images\" + p.Source);
            productGrid.Children.Add(image);
            Grid.SetRow(image, 0);
            Grid.SetColumn(image, 0);
            Grid.SetColumnSpan(image, 1);

            Label name = new Label
            {
                Content = p.Name,
                FontWeight = FontWeights.Bold
            };

            productGrid.Children.Add(name);
            Grid.SetRow(name, 1);
            Grid.SetColumn(name, 0);

            Label price = new Label
            {
                Content = p.Price + ":-",
                HorizontalAlignment = HorizontalAlignment.Right
            };
            productGrid.Children.Add(price);
            Grid.SetRow(price, 1);
            Grid.SetColumn(price, 0);

            Label description = new Label
            {
                Content = p.Description,
                FontStyle = FontStyles.Italic

            };
            productGrid.Children.Add(description);
            Grid.SetRow(description, 2);
            Grid.SetColumn(description, 0);

            Button addToCartButton = new Button
            {
                Content = "Add to cart",
                Margin = new Thickness(5),
                Tag = p
            };
            productGrid.Children.Add(addToCartButton);
            Grid.SetRow(addToCartButton, 3);
            Grid.SetColumn(addToCartButton, 0);
            Grid.SetColumnSpan(addToCartButton, 1);
            addToCartButton.Click += AddToCartButton_Click;

            return productGrid;
        }

        // When you press "Add to Cart" on a product, add it to the Cart
        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            totalSum += p.Price;
            // if the cart already contains the product, add another - increasing its amount
            if (Cart.ContainsKey(p))
            {
                Cart[p]++;
            }
            // otherwise, add it to the cart with 1 as the amount
            else
            {
                Cart.Add(p, 1);
            }

            cartPanel.Children.Clear();
            foreach (KeyValuePair<Product, int> pair in Cart)
            {
                AddToCartGrid(pair);
                totalSumLabel.Content = "Your total: " + totalSum;
            }
            SaveCart(Cart, CartFilePath);
        }

        // Grid inside the Cart with its contents - products
        private Grid AddToCartGrid(KeyValuePair<Product, int> p)
        {

            Grid cartGrid = new Grid();
            cartGrid.RowDefinitions.Add(new RowDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition());
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            cartPanel.Children.Add(cartGrid);
            Label productName = new Label
            {
                Content = p.Key.Name
            };
            cartGrid.Children.Add(productName);
            Grid.SetRow(productName, 0);
            Grid.SetColumn(productName, 0);

            Label amount = new Label
            {
                Content = p.Value + "x"
            };
            cartGrid.Children.Add(amount);
            Grid.SetRow(amount, 0);
            Grid.SetColumn(amount, 1);

            Label priceLabel = new Label
            {
                Content = (p.Key.Price * p.Value) + ":-"

            };
            cartGrid.Children.Add(priceLabel);
            Grid.SetRow(priceLabel, 0);
            Grid.SetColumn(priceLabel, 2);

            Button deleteButton = new Button
            {
                Content = "X",
                Width = 25,
                Tag = p.Key
            };
            cartGrid.Children.Add(deleteButton);
            Grid.SetRow(deleteButton, 0);
            Grid.SetColumn(deleteButton, 3);

            deleteButton.Click += DeleteButton_Click;

            return cartGrid;
        }
        // When the "X" button next to a product in the cart is pressed, remove it from the cart
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Product p = (Product)button.Tag;
            totalSum -= p.Price;
            // if there are more than one of a product, decrease its amount
            if (Cart[p] > 1)
            {
                Cart[p]--;
            }
            // otherwise, remove the product
            else
            {
                Cart.Remove(p);
            }

            cartPanel.Children.Clear();
            foreach (KeyValuePair<Product, int> pair in Cart)
            {
                AddToCartGrid(pair);
            }
            if(Cart.Count == 0)
            {
                totalSumLabel.Content = "Your total: 0"; 
            }
            else
            {
                totalSumLabel.Content = "Your total: " + totalSum;
            }
        }
    }
}