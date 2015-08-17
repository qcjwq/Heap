using Autofac;
using Autofac.Util;
using AutofacTest.RegistType.Implement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutofacUnitTest
{
    /// <summary>
    /// AutoFac定义了三种生命周期：
    /// Per Dependency
    /// Single Instance
    /// Per Lifetime Scope
    /// </summary>
    [TestClass]
    public class LifeTimeUnitTest
    {
        /// <summary>
        /// Per Dependency为默认的生命周期，也被称为’transient’或’factory’，其实就是每次请求都创建一个新的对象
        /// </summary>
        [TestMethod]
        public void Pre_Denpendency()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>().InstancePerDependency();
            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            var actual2 = container.Resolve<MyClass>();
            Assert.AreNotEqual(actual, actual2);
        }

        /// <summary>
        /// Single Instance也很好理解，就是每次都用同一个对象
        /// </summary>
        [TestMethod]
        public void Single_Instance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>().SingleInstance();
            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            var actual2 = container.Resolve<MyClass>();
            Assert.AreEqual(actual, actual2);
        }

        /// <summary>
        /// Per Lifetime Scope，同一个Lifetime生成的对象是同一个实例
        /// </summary>
        [TestMethod]
        public void Pre_Lifetime_Scope()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>().InstancePerLifetimeScope();
            IContainer container = builder.Build();
            var actual = container.Resolve<MyClass>();
            var actual2 = container.Resolve<MyClass>();

            ILifetimeScope inner = container.BeginLifetimeScope();
            var actual3 = inner.Resolve<MyClass>();
            var actual4 = inner.Resolve<MyClass>();

            Assert.AreEqual(actual, actual2);
            Assert.AreNotEqual(actual2, actual3);
            Assert.AreEqual(actual3, actual4);
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Life_Time_And_Dispose()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Disposable>();

            using (IContainer container = builder.Build())
            {
                var outInstance = container.Resolve<Disposable>(new NamedParameter("name", "out"));

                using (var inner = container.BeginLifetimeScope())
                {
                    var inInstance = container.Resolve<Disposable>(new NamedParameter("name", "in"));
                }//inInstance dispose here
            }//out dispose here
        }
    }
}