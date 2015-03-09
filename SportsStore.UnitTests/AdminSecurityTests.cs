using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminSecurityTests
    {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials()
        {

            //Arrange - create mock authrntification

            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("admin", "password")).Returns(true);

            //Arrange - create teh view model

            LoginViewModel model = new LoginViewModel
            {
                Username = "admin",
                Password = "password"
            };

            //Arrange - Create the controller

            AccountController target = new AccountController(mock.Object);

            //Act - authenticate using valid credentials
            ActionResult result = target.Login(model, "/MyUrl");

            //Assert

            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyUrl", ((RedirectResult)result).Url);


        }

        [TestMethod]
        public void Cannot_Login_With_InValid_Credentials()
        {

            //Arrange - create mock authrntification

            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("hack", "scripted")).Returns(false);

            //Arrange - create teh view model

            LoginViewModel model = new LoginViewModel
            {
                Username = "hack",
                Password = "scripted"
            };

            //Arrange - Create the controller

            AccountController target = new AccountController(mock.Object);

            //Act - authenticate using valid credentials
            ActionResult result = target.Login(model, "/MyUrl");

            //Assert

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);


        }
    }

}
