using System.Security.AccessControl;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using AutofacTest.Cycledependence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutofacUnitTest
{
    [TestClass]
    public class CycleDependenceUnitTest
    {
        /// <summary>
        /// ClassA和ClassB的生命周期不能都是InstancePerDependency
        /// </summary>
        [TestMethod]
        public void Circular_Dependencies_Exception()
        {
            var builder = new ContainerBuilder();
            builder.Register(a => new ClassB()
            {
                A = a.Resolve<ClassA>()
            });
            builder.Register(a => new ClassA(a.Resolve<ClassB>()));
            IContainer container = builder.Build();

            //会抛出异常
            //Assert.IsNotNull(container.Resolve<ClassA>());
        }

        /// <summary>
        /// 允许循环依赖，但是要设置SingleInstance
        /// </summary>
        [TestMethod]
        public void Circular_Dependencies_Ok()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ClassB>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies).SingleInstance();
            builder.Register(a => new ClassA(a.Resolve<ClassB>()));
            IContainer container = builder.Build();

            Assert.IsNotNull(container.Resolve<ClassA>());
            Assert.IsNotNull(container.Resolve<ClassB>());
            Assert.IsNotNull(container.Resolve<ClassB>().A);
        }
    }
}