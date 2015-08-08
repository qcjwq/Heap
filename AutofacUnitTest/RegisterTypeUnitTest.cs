using System;
using System.Runtime.Remoting;
using Autofac;
using Autofac.Core.Registration;
using AutofacTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Core;

namespace AutofacUnitTest
{
    /// <summary>
    /// 注册部分
    /// </summary>
    [TestClass]
    public class RegisterTypeUnitTest
    {
        /// <summary>
        /// 使用RegisterType进行注册
        /// </summary>
        [TestMethod]
        public void Can_Resolve_MyClass()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClass>();

            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// 注册为接口
        /// </summary>
        [TestMethod]
        public void Register_As_Interface()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new MyClass()).As<MyInterface>();

            IContainer container = builder.Build();

            Assert.IsNotNull(container.Resolve<MyInterface>());
        }

        /// <summary>
        /// 注册为Lamdba
        /// </summary>
        [TestMethod]
        public void Can_Register_With_Lambda()
        {
            var builder = new ContainerBuilder();
            builder.Register(a => new MyClass());

            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// 带构造参数的注册
        /// </summary>
        [TestMethod]
        public void Register_With_Parameter()
        {
            var builder = new ContainerBuilder();
            builder.Register(a => new MyParameter());
            builder.Register(a => new MyClass(a.Resolve<MyParameter>()));

            IContainer container = builder.Build();

            Assert.IsNotNull(container.Resolve<MyClass>());
        }

        /// <summary>
        /// 带属性赋值的注册
        /// </summary>
        [TestMethod]
        public void Register_With_Property()
        {
            var builder = new ContainerBuilder();
            builder.Register(a => new MyProperty());
            builder.Register(a => new MyClass()
            {
                Property = a.Resolve<MyProperty>()
            });

            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Property);
        }

        /// <summary>
        /// Autofac分离了类的创建和使用,
        /// 这样可以根据输入参数（NamedParameter）动态的选择实现类
        /// </summary>
        [TestMethod]
        public void Select_An_Implementer_Based_On_Parameter_Value()
        {
            var builder = new ContainerBuilder();
            builder.Register<IRepository>((c, p) =>
            {
                var type = p.Named<string>("type");
                if (type == "db")
                {
                    return new DbRepository();
                }
                else
                {
                    return new TxtRepository();
                }
            }).As<IRepository>();

            IContainer container = builder.Build();

            var actual = container.Resolve<IRepository>(new NamedParameter("type", "db"));
            Assert.AreEqual(typeof(DbRepository), actual.GetType());
        }

        /// <summary>
        /// 单例模式下注册
        /// </summary>
        [TestMethod]
        public void Register_With_Instance()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(MyInstance.Instance()).ExternallyOwned();

            IContainer container = builder.Build();

            var actual = container.Resolve<MyInstance>();
            var actual2 = container.Resolve<MyInstance>();
            Assert.AreEqual(actual, actual2);
        }
    }
}