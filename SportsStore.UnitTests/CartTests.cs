using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class CartTests
    {

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            // Arrange - create a mock order processor

            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create a cart with an item

            ShoppingCart cart = new ShoppingCart();

            cart.AddItem(new Product(), 1);

            //Arrange = create an instance of the controller

            CartController target = new CartController(null, mock.Object);

            //Act - try to check out.

            ViewResult result = target.Checkout(cart, new ShippingDetails());

            //Assert - check that the order has been passed to the processor.

            mock.Verify(m => m.ProcessOrder(It.IsAny<ShoppingCart>(), It.IsAny<ShippingDetails>()), Times.Once);

            // Assert - check that the method is returning the completed view

            Assert.AreEqual("Completed", result.ViewName);

            //Assert - check that i am passing a valid model to the view

            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);

        }


        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            // Arrange - create a mock order processor

            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arrange - create a cart with an item

            ShoppingCart cart = new ShoppingCart();
            
            cart.AddItem(new Product(), 1);

            //Arrange = create an instance of the controller

            CartController target = new CartController(null, mock.Object);

            //Arrange - add an error to the model
            target.ModelState.AddModelError("error", "error");

            //Act - try to check out.

            ViewResult result = target.Checkout(cart, new ShippingDetails());


            //Assert - check that the order wasnt passed to the processor.

            mock.Verify(m=>m.ProcessOrder(It.IsAny<ShoppingCart>(), It.IsAny<ShippingDetails>()), Times.Never);

            // Assert - check that the method is passing the default view

            Assert.AreEqual("",result.ViewName);

            //Assert - check that i am passing an invalid model to the view

            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);



        }


        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //arrange - create a mock order processor

            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //arrange - create an empty cart

            ShoppingCart cart = new ShoppingCart();

            //arrange - create shipping details

            ShippingDetails shippingDetails = new ShippingDetails();

            //arrange - create an instance of the controller

            CartController target = new CartController(null, mock.Object);


            //Act


            ViewResult result = target.Checkout(cart, shippingDetails);

            //Assert - check that the order has'nt been passed on to teh order processor.

            mock.Verify(m=>m.ProcessOrder(It.IsAny<ShoppingCart>(), It.IsAny<ShippingDetails>()), Times.Never());

            //Assert - check that the method is returning the default view

            Assert.AreEqual("", result.ViewName);

            //Assert - check that im passing an invalid model to the view

            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);



        }


        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //arrange

            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());

            //arrange - create a cart

            ShoppingCart cart = new ShoppingCart();

            //arrange = create the controller

            CartController target = new CartController(mock.Object, null);

            //act - add a product to the cart
            target.AddToCart(cart, 1, null);

            //assert

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductId, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //arrange

            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1", Category = "Apples"}
            }.AsQueryable());

            //arrange - create a cart

            ShoppingCart cart = new ShoppingCart();

            //arrange = create the controller

            CartController target = new CartController(mock.Object, null);

            //act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");

        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //arrange - create a cart

            ShoppingCart cart = new ShoppingCart();

            //arrange = create the controller

            CartController target = new CartController(null, null);

            //Act

            CartIndexViewModel result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            // Assert

            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");
        }


        [TestMethod]
        public void Can_Clear_Contents()
        {
            //arrange - create some test products

            Product p1 = new Product { ProductId = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            //Arrange - create a new cart

            ShoppingCart target = new ShoppingCart();

            //Arrange - add some items

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            //act - reset the cart

            target.Clear();


            //assert

            Assert.AreEqual(target.Lines.Count(), 0);
        }


        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //Arrange - create some test products

            Product p1 = new Product {ProductId = 1, Name = "P1", Price = 100M};
            Product p2 = new Product { ProductId = 2, Name = "P2", Price = 50M };

            //Arrange - create a new cart

            ShoppingCart target = new ShoppingCart();

            //act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 3);

            decimal result = target.ComputeTotalValue();

            //Assert

            Assert.AreEqual(result, 450M);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //arrange - create some test products

            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };
            Product p3 = new Product { ProductId = 3, Name = "P3" };

            //arrange - crate a new cart.

            ShoppingCart target = new ShoppingCart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //Act

            target.RemoveItem(p2);

            //Assert

            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);


        }


        [TestMethod]
        public void Can_add_Quantity_For_Existing_lines()
        {
            //arrange - create some test products

            Product p1 = new Product { ProductId = 1, Name = "P1" };
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            //arrange - crate a new cart.

            ShoppingCart target = new ShoppingCart();

            //act
            target.AddItem(p1,1);
            target.AddItem(p2,1);
            target.AddItem(p1,10);

            CartLine[] results = target.Lines.ToArray();

            //assert

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //arrange - create some test products

            Product p1 = new Product {ProductId = 1, Name = "P1"};
            Product p2 = new Product { ProductId = 2, Name = "P2" };

            //arrange - crate a new cart.

            ShoppingCart target = new ShoppingCart();

            //act

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();

            //Assert

            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);

        }
        
        
    }
}
