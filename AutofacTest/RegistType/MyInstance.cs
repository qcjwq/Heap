﻿namespace AutofacTest.RegistType
{
    public class MyInstance
    {
        private static MyInstance instance = new MyInstance();

        private MyInstance()
        {

        }

        public static MyInstance Instance()
        {
            return instance;
        }
    }
}