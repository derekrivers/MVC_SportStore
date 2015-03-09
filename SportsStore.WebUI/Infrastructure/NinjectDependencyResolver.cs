using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Moq;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Infrastructure.Abstract;
using SportsStore.WebUI.Infrastructure.Concrete;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {

        private IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        public void AddBindings()
        {
            //Add bindings here.

            //Mock<IProductRepository> mock = new Mock<IProductRepository>();

            //mock.Setup(m => m.Products).Returns(new List<Product>
            //{
            //    new Product {Name = "Football", Price = 25},
            //    new Product {Name = "Surf Board", Price = 179},
            //    new Product {Name = "Running Shoes", Price = 95},
            //});

            _kernel.Bind<IProductRepository>().To<EFProductRepository>();


            EmailSetings emailSetings = new EmailSetings()
            {
                WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false")
            };

            _kernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("emailSettings", emailSetings);

            _kernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
        }

       
    }
}