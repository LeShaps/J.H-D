using System;
using System.Collections.Generic;
using System.Text;

namespace J.H_D.Data
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class HelpAttribute : Attribute
    {
        public string Category { get; private set; }
        public string Description { get; private set; }
        public Warnings Warnings { get; private set; }

        public HelpAttribute(string Category, string Description)
        {
            this.Category = Category;
            this.Description = Description;
            Warnings = Warnings.None;
        }

        public HelpAttribute(string Category, string Description, Warnings WarningsFlags)
        {
            this.Category = Category;
            this.Description = Description;
            this.Warnings = WarningsFlags;
        }
    }
}
