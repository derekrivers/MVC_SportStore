using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {



        [TestMethod]
        public void Can_Delete_Valid_Product()
        {
            //Arrange

            Product prod = new Product { ProductId = 2, Name = "Test" };

            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 3, Name = "P3"},
            });

            //Arrange - create a controller
            AdminController target = new AdminController(mock.Object);

            //Act

            target.Delete(prod.ProductId);

            //Assert

            mock.Verify(m=> m.DeleteProduct(prod.ProductId));


        }

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {

            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Arrange - Create the controller
            AdminController target = new AdminController(mock.Object);

            //Arrange - create a product
            Product product = new Product { Name = "Test" };

            //Act - try to save the product
            ActionResult result = target.Edit(product);

            //Assert - check the method result type

            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
            Assert.IsInstanceOfType(result, typeof(ActionResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            //Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            // Arrange - Create the controller
            AdminController target = new AdminController(mock.Object);

            //Arrange - create a product
            Product product = new Product { Name = "Test" };

            //Arrange

            target.ModelState.AddModelError("error", "error");

            //Act - try to save the product

            ActionResult result = target.Edit(product);

            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never);

            //Assert

            Assert.IsInstanceOfType(result, typeof(ViewResult));


        }


        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
            });

            // Arrange - Create the controller
            AdminController target = new AdminController(mock.Object);


            //act
            Product result = (Product)target.Edit(4).ViewData.Model;

            //assert
            Assert.IsNull(result);

        }


        [TestMethod]
        public void Can_Edit_Product()
        {
            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
            });

            // Arrange - Create the controller
            AdminController target = new AdminController(mock.Object);


            //act
            Product p1 = (Product)target.Edit(1).ViewData.Model;
            Product p2 = (Product)target.Edit(2).ViewData.Model;
            Product p3 = (Product)target.Edit(3).ViewData.Model;

            //Assert
            Assert.AreEqual(1, p1.ProductId);
            Assert.AreEqual(2, p2.ProductId);
            Assert.AreEqual(3, p3.ProductId);



        }

        [TestMethod]
        public void Index_Contains_All_Products()
        {

            //Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();

            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductId = 1, Name = "P1"},
                new Product {ProductId = 2, Name = "P2"},
                new Product {ProductId = 3, Name = "P3"},
            });

            // Arrange - Create the controller
            AdminController target = new AdminController(mock.Object);

            // Act
            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();


            //Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual(result[0].ProductId, 1);
            Assert.AreEqual(result[1].ProductId, 2);
            Assert.AreEqual(result[2].ProductId, 3);

        }
    }
}
