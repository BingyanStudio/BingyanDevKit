using System;

namespace Bingyan
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CSVAttribute : Attribute
    {
        public string Header => header;
        private readonly string header;

        public CSVAttribute(string header)
        {
            this.header = header;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class CSVIgnoreAttribute : Attribute { }
}