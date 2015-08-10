namespace AutofacTest.Event
{
    public class MyClassWithMethod
    {
        public int Index { get; set; }

        public void Add(int value)
        {
            Index = Index + value;
        }
    }
}