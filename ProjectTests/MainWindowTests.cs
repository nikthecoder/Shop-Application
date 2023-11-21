using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Project.Tests
{
    [TestClass()]
    public class MainWindowTests
    {

        [TestMethod()]
        public void FirstProductNameTest()
        {
            Product[] Products = MainWindow.LoadProducts("tests.csv");
            Assert.AreEqual("Laptop", Products[0].Name);
            Assert.AreEqual(5, Products.Length);
        }

        [TestMethod()]
        public void AllProductDescriptionsTest()
        {
            Product[] Products = MainWindow.LoadProducts("tests.csv");
            Assert.AreEqual("Computer", Products[0].Description);
            Assert.AreEqual("Computer", Products[1].Description);
            Assert.AreEqual("Sound", Products[2].Description);
            Assert.AreEqual("Sound", Products[3].Description);
            Assert.AreEqual("Device", Products[4].Description);
        }

        [TestMethod()]
        public void CreateCartFileTest()
        {
            bool exists;
            Dictionary<Product, int> Cart = new Dictionary<Product, int>();
            MainWindow.SaveCart(Cart, @"C:\Windows\Temp\testCart.csv");
            if (File.Exists(@"C:\Windows\Temp\testCart.csv"))
            {
                exists = true;
            }
            else
            {
                exists = false;
            }
            Assert.AreEqual(true, exists);
        }

        [TestMethod()]
        public void AddAndWriteToCartFileTest()
        {
            bool exists;
            Dictionary<Product, int> Cart = new Dictionary<Product, int>();

            var products = MainWindow.LoadProducts("tests.csv");
            foreach (Product p in products)
            {
                Cart.Add(p, 1);
            }

            MainWindow.SaveCart(Cart, @"C:\Windows\Temp\testCart.csv");
            if (File.Exists(@"C:\Windows\Temp\testCart.csv"))
            {
                exists = true;
            }
            else
            {
                exists = false;
            }
            Assert.AreEqual(true, exists);
            Assert.AreEqual(5, Cart.Count);

        }

        [TestMethod()]
        public void SaveAndLoadCartTest()
        {
            bool exists;
            Dictionary<Product, int> Cart = new Dictionary<Product, int>();
            Product[] Products = MainWindow.LoadProducts("tests.csv");

            foreach (Product p in Products)
            {
                Cart.Add(p, 1);
            }

            MainWindow.SaveCart(Cart, @"C:\Windows\Temp\testCart.csv");

            if (File.Exists(@"C:\Windows\Temp\testCart.csv"))
            {
                Cart = MainWindow.LoadCart(Products, @"C:\Windows\Temp\testCart.csv");
                exists = true;
            }
            else
            {
                exists = false;
            }
            Assert.AreEqual(true, exists);
            Assert.AreEqual(5, Cart.Count);
        }
    }
}