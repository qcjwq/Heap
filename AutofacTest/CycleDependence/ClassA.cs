namespace AutofacTest.Cycledependence
{
    public class ClassA
    {
        private readonly ClassB b;

        public ClassA(ClassB b)
        {
            this.b = b;
        }
    }
}