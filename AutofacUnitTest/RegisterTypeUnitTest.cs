using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using Autofac;
using Autofac.Builder;
using Autofac.Core.Registration;
using AutofacTest;
using AutofacTest.RegistType;
using AutofacTest.RegistType.Implement;
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

        /// <summary>
        /// 注册open generic类型
        /// </summary>
        [TestMethod]
        public void Regitster_Open_Generic()
        {
            var builder = new ContainerBuilder();
            builder.RegisterGeneric(typeof(MyList<>));
            IContainer container = builder.Build();

            var actual = container.Resolve<MyList<int>>();
            Assert.IsNotNull(actual);

            var actual2 = container.Resolve<MyList<string>>();
            Assert.IsNotNull(actual2);
        }

        /// <summary>
        /// 对于同一个接口，后面注册的实现会覆盖之前的实现
        /// </summary>
        [TestMethod]
        public void Register_Order()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DbRepository>().As<IRepository>();
            builder.RegisterType<TxtRepository>().As<IRepository>();
            IContainer container = builder.Build();

            var actual = container.Resolve<IRepository>();
            Assert.AreEqual(typeof(TxtRepository), actual.GetType());
        }

        /// <summary>
        /// 如果不想覆盖的话，可以用PreserveExistingDefaults，这样会保留原来注册的实现
        /// </summary>
        [TestMethod]
        public void Register_Order_Defaults()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DbRepository>().As<IRepository>();
            builder.RegisterType<TxtRepository>().As<IRepository>().PreserveExistingDefaults();
            IContainer container = builder.Build();

            var actual = container.Resolve<IRepository>();
            Assert.AreEqual(typeof(DbRepository), actual.GetType());
        }

        /// <summary>
        /// 可以用Name来区分不同的实现，代替As方法
        /// </summary>
        [TestMethod]
        public void Register_With_Name()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DbRepository>().Named<IRepository>("DB");
            builder.RegisterType<TxtRepository>().Named<IRepository>("TXT");
            IContainer container = builder.Build();

            var actualDb = container.ResolveNamed<IRepository>("DB");
            var actualTxt = container.ResolveNamed<IRepository>("TXT");
            Assert.AreEqual(typeof(DbRepository), actualDb.GetType());
            Assert.AreEqual(typeof(TxtRepository), actualTxt.GetType());
        }

        /// <summary>
        /// 如果一个类有多个构造函数的话，可以在注册时候选择不同的构造函数
        /// </summary>
        [TestMethod]
        public void Chose_Construtors()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyParameter>();
            builder.RegisterType<MyClass>().UsingConstructor(typeof(MyParameter));
            IContainer container = builder.Build();

            var actual = container.Resolve<MyClass>();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// 可以注册一个Assemble下所有的类，当然，也可以根据类型进行筛选
        /// </summary>
        [TestMethod]
        public void Register_Assembly()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies())
                .Where(a => a.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
            IContainer container = builder.Build();

            var actual = container.Resolve<IRepository>();
            Assert.IsNotNull(actual);
        }
    }
}