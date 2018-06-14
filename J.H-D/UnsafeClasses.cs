using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    public enum TagType
    {
        General,
        Artist,
        Copyright,
        Character,
        Unknown
    }

    enum Imagetype
    {
        Image_s,
        Konachan,
        Danbooru,
        Sankaku
    }

    public partial class Tag
    {
        public TagType _type { private set; get; }
        public string _name { private set; get; }
        public uint _id { private set; get; }

        public Tag(TagType type, string name, uint id)
        {
            _type = type;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _id = id;
        }

        public Tag(TagType type, string name)
        {
            _type = type;
            _name = name;
            _id = 0;
        }

        public Tag(string source)
        {
            _name = Program.getInfos("\"name\":\"", source, '"');
            _id = Convert.ToUInt32(Program.getInfos("\"id\":", source, '}'));
            ushort type = Convert.ToUInt16(Program.getInfos("\"type\":", source, ','));
            switch (type)
            {
                case 0:
                    _type = TagType.General;
                    break;
                case 1:
                    _type = TagType.Artist;
                    break;
                case 2:
                    _type = TagType.Copyright;
                    break;
                case 3:
                    _type = TagType.Character;
                    break;
                default:
                    _type = TagType.Unknown;
                    break;
            }
        }
    }
}
