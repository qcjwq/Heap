using System;

namespace AutofacTest.Event
{
    public class MyEvent : IDisposable
    {
        public MyEvent(string input)
        {
            Console.WriteLine(input);
        }

        public MyEvent()
        {
            Console.WriteLine("Init");
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
        }
    }
}