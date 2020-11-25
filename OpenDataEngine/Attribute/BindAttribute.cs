using System;

namespace OpenDataEngine.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BindAttribute : System.Attribute
    {
        public String Local { get; }
        public String Foreign { get; }

        public BindAttribute(String local, String foreign)
        {
            Local = local;
            Foreign = foreign;
        }
    }
}
