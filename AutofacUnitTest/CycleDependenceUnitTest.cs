using Autofac;
using AutofacTest.Cycledependence;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutofacUnitTest
{
    [TestClass]
    public class CycleDependenceUnitTest
    {
        [TestMethod]
        public void Circular_Dependencies_Exception()
        {
            var builder = new ContainerBuilder();
            builder.Register(a => new ClassB() { A = a.Resolve<ClassA>() });
            builder.Register(a => new ClassA(a.Resolve<ClassB>()));

            IContainer container = builder.Build();

            Assert.IsNotNull(container.Resolve<ClassA>());
        }
    }
}