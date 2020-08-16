using System;
namespace J.H_D.Data
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class EmbedableAttribute : Attribute
    {
        public string Name { get; private set; }
        public bool Fieldable { get; private set; }
        public bool Link { get; private set; }

        public EmbedableAttribute(string FieldName)
        {
            Name = FieldName;
            Fieldable = true;
            Link = false;
        }

        public EmbedableAttribute(string FieldName, bool Fieldable = true, bool Link = false)
        {
            Name = FieldName;
            this.Fieldable = Fieldable;
            this.Link = Link;
        }
    }
}
