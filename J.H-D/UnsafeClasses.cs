using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    enum TagType
    {
        General,
        Artist,
        Copyright,
        Character,
        Unknown
    }

    public class Image_s
    {
        public string _file_url { protected set; get; }
        public string _sample_url { protected set; get; }
        public string _preview_url { protected set; get; }
        public char _rating { protected set; get; }
        public bool _isLoli { protected set; get; }
        public string _source { protected set; get; }
        public string _name { protected set; get; }
    }

    class Tag
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

    class kon_image : Image_s
    {
        public string _jpegurl { private set; get; }
        public string _author { private set; get; }
        public List<Tag> _tags { private set; get; }
        public int _filesize { private set; get; }

        public kon_image() { }

        public kon_image(string source)
        {
            _file_url = Program.getInfos("file_url=\"", source, '"');
            _sample_url = Program.getInfos("sample_url=\"", source, '"');
            _preview_url = Program.getInfos("preview_url=\"", source, '"');
            if (Program.getInfos("rating\"", source, '"') != null)
                _rating = Program.getInfos("rating\"", source, '"')[0];
            else
                _rating = 'e';
            if (Program.getInfos("has_children\"", source, '"') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("source=\"", source, '"');
            make_name(_file_url);
            _author = Program.getInfos("author=\"", source, '"');
            _jpegurl = Program.getInfos("jpeg_url=\"", source, '"');
            _filesize = Convert.ToInt32(Program.getInfos("file_size=\"", source, '"'));
            make_tags(source);
        }

        private void make_name(string fileurl)
        {
            string[] bypoint = fileurl.Split('.');
            string extention = bypoint[bypoint.Length - 1];
            string[] byslash = fileurl.Split('/');
            string name = byslash[byslash.Length - 2];

            _name = name + "." + extention;
        }

        void make_tags(string source)
        {
            List<Tag> taglist = new List<Tag>();
            string unsplited = Program.getInfos("tags=\"", source, '"');

            if (unsplited == null)
                return;
            string[] tags = unsplited.Split(' ');
            foreach(string s in tags)
            {
                Tag newtag = new Tag(TagType.Unknown, s);
                taglist.Add(newtag);
            }
            _tags = taglist;
        }

        public string make_tagnamelist()
        {
            string[] tagnames = new string[_tags.Count];
            int i = 0;

            for (; i < _tags.Count - 2; i++)
            {
                tagnames[i] = _tags[i]._name + ",";
            }
            tagnames[i] = _tags[i]._name;
            return (Program.makeArgs(tagnames));
        }

        public override string ToString()
        {
            string tofile = "{" + _file_url + "|" + _sample_url + "|" + _preview_url + "|" + _jpegurl + "|" +  _rating + "|" + _isLoli.ToString() + "|" + _source + "|" +
                _name + "|" + _filesize + "|" + _author + "},{";

            string TagsInfos = null;
            foreach (Tag t in _tags)
            {
                string newtag = "[" + t._name + ":" + t._id + ":" + t._type.ToString() + "]";
                TagsInfos += newtag;
            }
            tofile += TagsInfos + "}" + Environment.NewLine;
            return (tofile);
        }
    }

    class dan_image : Image_s
    {
        public uint _file_size { private set; get; }
        public bool _is_banned { private set; get; }
        public string _author { private set; get; }
        public List<Tag> _tags { private set; get; }

        public dan_image(string source)
        {
            _file_url = Program.getInfos("<large-file-url>", source, '<');
            _sample_url = Program.getInfos("<file-url>", source, '<');
            _preview_url = Program.getInfos("<preview-file-url>", source, '<');
            _rating = Program.getInfos("<rating>", source, '<')[0];
            if (Program.getInfos("<has-children type=\"boolean\">", source, '<') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("<source>", source, '<');
            _file_size = Convert.ToUInt32(Program.getInfos("<file-size type=\"integrer\">", source, '<'));
            if (Program.getInfos("<is-banned type=\"boolean\">", source, '<') == "false")
                _is_banned = false;
            else
                _is_banned = true;
            _author = Program.getInfos("<uploader-name>", source, '<');
            make_name(_file_url);
            make_tags(source);
        }

        private void make_name(string ext)
        {
            string[] fparsed = ext.Split('.');
            string extension = fparsed[fparsed.Length - 1];
            string[] parsed_fname = fparsed[fparsed.Length - 2].Split('/');
            string fileurl = parsed_fname[parsed_fname.Length - 1] + "." + extension;
            string name = fileurl.Split('?')[0];

            _name = name;
        }

        public string make_tagnamelist()
        {
            string[] tagnames = new string[_tags.Count];
            int i = 0;

            for (; i < _tags.Count - 2; i++)
            {
                tagnames[i] = _tags[i]._name + ",";
            }
            tagnames[i] = _tags[i]._name;
            return (Program.makeArgs(tagnames));
        }

        private void make_tags(string source)
        {
            List<Tag> tags = new List<Tag>();
            string generaltags = Program.getInfos("<tag-string-general>", source, '<');
            string charactertags = Program.getInfos("<tag-string-character>", source, '<');
            string artisttags = Program.getInfos("<tag-string-artist>", source, '<');
            string metatags = Program.getInfos("<tag-string-meta>", source, '<');

            if (generaltags != null)
            {
                foreach(string s in generaltags.Split(' '))
                {
                    Tag newtag = new Tag(TagType.General, s);
                    tags.Add(newtag);
                }
            }
            if (charactertags != null)
            {
                foreach(string s in charactertags.Split(' '))
                {
                    Tag newtag = new Tag(TagType.Character, s);
                    tags.Add(newtag);
                }
            }
            if (artisttags != null)
            {
                foreach(string s in artisttags.Split(' '))
                {
                    Tag newtag = new Tag(TagType.Artist, s);
                    tags.Add(newtag);
                }
            }
            if (metatags != null)
            {
                foreach(string s in metatags.Split(' '))
                {
                    Tag newtag = new Tag(TagType.Unknown, s);
                    tags.Add(newtag);
                }
            }
            _tags = tags;
        }

        public override string ToString()
        {
            string tofile = "{" + _file_url + "|" + _sample_url + "|" + _preview_url + "|" + _rating + "|" + _isLoli.ToString() + "|" + _source + "|" +
                _name + "|" + _file_size + "|" + _is_banned.ToString() + "|" + _author + "},{";

            string TagsInfos = null;
            foreach (Tag t in _tags)
            {
                string newtag = "[" + t._name + ":" + t._id + ":" + t._type.ToString() + "]";
                TagsInfos += newtag;
            }
            tofile += TagsInfos + "}" + Environment.NewLine;
            return (tofile);
        }
    }

    class sk_image : Image_s
    {
        public string _author { private set; get; }
        public string _file_furl { private set; get; }
        public List<Tag> _tags { private set; get; }

        public sk_image() { }

        public sk_image(string source)
        {
            _file_url = Program.getInfos("\"file_url\":\"//", source, '"');
            _sample_url = Program.getInfos("\"sample_url\":\"//", source, '"');
            _preview_url = Program.getInfos("\"preview_url\":\"//", source, '"');
            _rating = Program.getInfos("\"rating\":\"", source, '"')[0];
            if (Program.getInfos("\"has_children\":", source, ',') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("\"source\":\"", source, '"');
            _author = Program.getInfos("\"author\":\"", source, '"');
            make_taglist(Program.getInfos("\"tags\":[", source, ']'));
            make_name(_file_url);
        }

        private void make_name(string url)
        {
            string[] fparsed = url.Split('.');
            string extension = fparsed[fparsed.Length - 1];
            string[] parsed_fname = fparsed[fparsed.Length - 2].Split('/');
            string fileurl = parsed_fname[parsed_fname.Length - 1] + "." + extension;
            string name = fileurl.Split('?')[0];

            _file_furl = "https://" + url;
            _name = name;
        }

        public string make_tagnamelist()
        {
            string[] tagnames = new string[_tags.Count];
            int i = 0;

            for (; i < _tags.Count - 2; i++)
            {
                tagnames[i] = _tags[i]._name + ",";
            }
            tagnames[i] = _tags[i]._name;
            return (Program.makeArgs(tagnames));
        }

        private void make_taglist(string ttp)
        {
            List<Tag> tags = new List<Tag>();
            string[] separatedTags = ttp.Split(new string[] { "},{" }, StringSplitOptions.None);

            foreach (string s in separatedTags)
            {
                Tag addTag = new Tag(s);
                tags.Add(addTag);
            }
            _tags = tags;
        }

        public override string ToString()
        {
            string tofile = "{" + _file_url + "|" + _sample_url + "|" + _preview_url + "|" + _rating + "|" + _isLoli.ToString() + "|" + _source + "|" +
                _name + "|" + _author + "},{";

            string TagsInfos = null;
            foreach (Tag t in _tags)
            {
                string newtag = "[" + t._name + ":" + t._id + ":" + t._type.ToString() + "]";
                TagsInfos += newtag;
            }
            tofile += TagsInfos + "}" + Environment.NewLine;
            return (tofile);
        }
    }

    class r34_image : Image_s
    {
        public string[] _tags { private set; get; }

        public r34_image() { }

        public r34_image(string source)
        {
            _file_url = Program.getInfos("file_url=\"", source, '"');
            _sample_url = Program.getInfos("sample_url=\"", source, '"');
            _preview_url = Program.getInfos("preview_url=\"", source, '"');
            _rating = Program.getInfos("rating=\"", source, '"')[0];
            if (Program.getInfos("has_children=\"", source, '"') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("source=\"", source, '"');
            _tags = Program.getInfos("tags=\"", source, '"').Split(' ');
        }
    }

}
