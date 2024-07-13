using System.Collections.Generic;

namespace XmlDocConverter.Models
{
    public class MemberDocumentation
    {
        public string MemberName { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public string Returns { get; set; }
        public List<string> SeeAlso { get; set; } = new List<string>();
        public List<string> Examples { get; set; } = new List<string>();
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> TypeParameters { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Exceptions { get; set; } = new Dictionary<string, string>();
    }

    public class ClassDocumentation
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public List<MemberDocumentation> Members { get; set; } = new List<MemberDocumentation>();
    }
}

