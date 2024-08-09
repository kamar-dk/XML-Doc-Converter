using System.Collections.Generic;

namespace XmlDocConverterLibary.Models
{
    /// <summary>
    /// Class for Member documentation from the XML documentation
    /// </summary>
    public class MemberDocumentation
    {
        /// <summary>
        /// Type of the Member (Method, Property, Field, etc.)
        /// </summary>
        public string MemberType { get; set; }
        /// <summary>
        /// Name of the Member
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// Summary of the Member
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// Remarks of the Member
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// Returns of the Member
        /// </summary>
        public string Returns { get; set; }
        /// <summary>
        /// Value of the Member
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Permission of the Member
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// Indicates if the documentation is inherited
        /// </summary>
        public bool InheritDoc { get; set; }
        /// <summary>
        /// Completion list for the Member
        /// </summary>
        public string CompletionList { get; set; }
        /// <summary>
        /// List of SeeAlso references
        /// </summary>
        public List<string> SeeAlso { get; set; } = new List<string>();
        /// <summary>
        /// List of Examples
        /// </summary>
        public List<string> Examples { get; set; } = new List<string>();
        /// <summary>
        /// List of Overloads
        /// </summary>
        public string Overloads { get; set; }
        /// <summary>
        /// Lists associated with the member
        /// </summary>
        public List<string> Lists { get; set; } = new List<string>();
        /// <summary>
        /// List of Parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// List of Type Parameters
        /// </summary>
        public Dictionary<string, string> TypeParameters { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// Dictionary of Exceptions
        /// </summary>
        public Dictionary<string, string> Exceptions { get; set; } = new Dictionary<string, string>();
    }

    /// <summary>
    /// Class for Class Documentation from the XML documentation
    /// </summary>
    public class ClassDocumentation
    {
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string Summary { get; set; }
        public string Remarks { get; set; }
        public List<MemberDocumentation> Members { get; set; } = new List<MemberDocumentation>();
    }
}
