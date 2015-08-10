using System;
using Autofac;
using AutofacTest.Event;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutofacUnitTest
{
    [TestClass]
    public class EventUnitTest
    {
        [TestMethod]
        public void Event_Test()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyEvent>()
                .OnRegistered(e => Console.WriteLine("OnRegistered"))
                .OnPreparing(e => Console.WriteLine("OnPreparing"))
                .OnActivating(e => e.ReplaceInstance(new MyEvent("Input")))
                .OnActivating(e => Console.WriteLine("OnActivating"))
                .OnActivated(e => Console.WriteLine("OnActivated"))
                .OnRelease(e => Console.WriteLine("OnRelease"));

            using (IContainer container = builder.Build())
            {
                using (var actual = container.Resolve<MyEvent>())
                {
                    Assert.IsNotNull(actual);
                }
            }

            /*输出：
            OnRegistered
            OnPreparing
            Init
            Input
            OnActivating
            OnActivated
            Dispose
            OnRelease
            */
        }

        /// <summary>
        /// 利用事件可以在构造对象之后调用对象的方法
        /// </summary>
        [TestMethod]
        public void Call_Method_When_Init()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MyClassWithMethod>().OnActivating(e => e.Instance.Add(5));
            IContainer container = builder.Build();

            Assert.AreEqual(5, container.Resolve<MyClassWithMethod>().Index);
        }
    }
}