using System;

namespace J.H_D.Data
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    class ParameterAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ParameterType Type { get; private set; }

        public ParameterAttribute(string Name, string Description, ParameterType Type)
        {
            this.Name = Name;
            this.Description = Description;
            this.Type = Type;
        }
    }
}
