using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using XML_Doc_Converter_Start.Utilities;

namespace XMLDocConverter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "docTest.xml");
            XDocument xmlDoc = XDocument.Load(xmlPath);

            var classDocs = XmlHandler.ParseDocumentation(xmlDoc);
            var markdownContent = MarkdownHandler.GenerateMarkdown(classDocs);

            string markdownPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "documentation.md");
            File.WriteAllText(markdownPath, markdownContent);
            Console.WriteLine($"Markdown file created at {markdownPath}");
        }
    }
}

/* This works

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XMLDocConverter
{
    public class Program
    {
        public class MemberDocumentation
        {
            public string MemberName { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public List<string> SeeAlso { get; set; } = new List<string>();
            public List<string> Examples { get; set; } = new List<string>();
            public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        }

        public class ClassDocumentation
        {
            public string ClassName { get; set; }
            public string Namespace { get; set; }
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public List<MemberDocumentation> Members { get; set; } = new List<MemberDocumentation>();
        }

        public static void Main(string[] args)
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "docTest.xml");
            XDocument xmlDoc = XDocument.Load(xmlPath);

            var classDocs = ParseDocumentation(xmlDoc);

            foreach (var classDoc in classDocs)
            {
                Console.WriteLine($"Class: {classDoc.ClassName}");
                Console.WriteLine($"Namespace: {classDoc.Namespace}");
                Console.WriteLine($"Summary: {classDoc.Summary}");
                Console.WriteLine($"Remarks: {classDoc.Remarks}");
                Console.WriteLine("Members:");
                foreach (var member in classDoc.Members)
                {
                    Console.WriteLine($"\tMember: {member.MemberName}");
                    Console.WriteLine($"\tSummary: {member.Summary}");
                    Console.WriteLine($"\tRemarks: {member.Remarks}");
                    if (member.Parameters.Count > 0)
                    {
                        Console.WriteLine("\tParameters:");
                        foreach (var param in member.Parameters)
                        {
                            Console.WriteLine($"\t\t{param.Key}: {param.Value}");
                        }
                    }
                    if (member.SeeAlso.Count > 0)
                    {
                        Console.WriteLine("\tSee Also:");
                        foreach (var seeAlso in member.SeeAlso)
                        {
                            Console.WriteLine($"\t\t{seeAlso}");
                        }
                    }
                    if (member.Examples.Count > 0)
                    {
                        Console.WriteLine("\tExamples:");
                        foreach (var example in member.Examples)
                        {
                            Console.WriteLine($"\t\t{example}");
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        public static List<ClassDocumentation> ParseDocumentation(XDocument xmlDoc)
        {
            var classDocs = new List<ClassDocumentation>();

            var members = xmlDoc.Descendants("member");
            var classes = members.Where(m => m.Attribute("name").Value.StartsWith("T:"));

            foreach (var classElement in classes)
            {
                var fullClassName = classElement.Attribute("name").Value.Substring(2);
                var classDoc = new ClassDocumentation
                {
                    ClassName = fullClassName.Split('.').Last(),
                    Namespace = string.Join(".", fullClassName.Split('.').SkipLast(1)),
                    Summary = classElement.Element("summary")?.Value.Trim(),
                    Remarks = classElement.Element("remarks")?.Value.Trim()
                };

                var memberElements = members.Where(m => m.Attribute("name").Value.StartsWith($"M:{fullClassName}."));

                foreach (var memberElement in memberElements)
                {
                    var memberDoc = new MemberDocumentation
                    {
                        MemberName = memberElement.Attribute("name").Value,
                        Summary = memberElement.Element("summary")?.Value.Trim(),
                        Remarks = memberElement.Element("remarks")?.Value.Trim()
                    };

                    var paramsElements = memberElement.Elements("param");
                    foreach (var paramElement in paramsElements)
                    {
                        memberDoc.Parameters[paramElement.Attribute("name").Value] = paramElement.Value.Trim();
                    }

                    var seeAlsoElements = memberElement.Elements("seealso");
                    foreach (var seeAlsoElement in seeAlsoElements)
                    {
                        memberDoc.SeeAlso.Add(seeAlsoElement.Attribute("cref").Value);
                    }

                    var exampleElements = memberElement.Elements("example");
                    foreach (var exampleElement in exampleElements)
                    {
                        memberDoc.Examples.Add(exampleElement.Value.Trim());
                    }

                    classDoc.Members.Add(memberDoc);
                }

                classDocs.Add(classDoc);
            }

            return classDocs;
        }
    }
}*/




/*using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;


public class Program
{
    public static void Main(string[] args)
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "docTest.xml");

        if (!File.Exists(filePath))
        {
            Console.WriteLine("The XML file was not found at " + filePath);
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(Documentation));
        using (StreamReader reader = new StreamReader(filePath))
        {
            Documentation doc = (Documentation)serializer.Deserialize(reader);

            // Dictionary to store class information
            Dictionary<string, ClassInfo> classInfoDict = new Dictionary<string, ClassInfo>();

            // Organize members into the appropriate classes
            foreach (var member in doc.Members)
            {
                // Extract the class name from the member name
                string className = ExtractClassName(member.Name);

                if (!classInfoDict.ContainsKey(className))
                {
                    classInfoDict[className] = new ClassInfo { Name = className };
                }

                classInfoDict[className].Members.Add(member);
            }

            // Output the results
            Console.WriteLine($"Assembly Name: {doc.Assembly.Name}");

            foreach (var classInfo in classInfoDict.Values)
            {
                PrintClassInfo(classInfo);
            }
        }
    }

    private static string ExtractClassName(string memberName)
    {
        // Extracts the class name from the member name
        // e.g., "T:TaggedLibrary.Math1" or "M:TaggedLibrary.Math1.Add(System.Int32,System.Int32)"
        int colonIndex = memberName.IndexOf(':');
        int dotIndex = memberName.IndexOf('.', colonIndex + 1);

        if (dotIndex == -1)
        {
            // If there's no dot, it's a type name
            return memberName.Substring(colonIndex + 1).Split('.')[1]; // Extract class name, ignoring namespace
        }
        else
        {
            // Otherwise, it's a member name, so extract the class part
            int classEndIndex = memberName.LastIndexOf('.', memberName.IndexOf('(') - 1);
            if (classEndIndex == -1) classEndIndex = memberName.LastIndexOf('.');
            return memberName.Substring(colonIndex + 1, classEndIndex - colonIndex - 1).Split('.')[1]; // Extract class name, ignoring namespace
        }
    }

    private static void PrintClassInfo(ClassInfo classInfo)
    {
        Console.WriteLine($"\nClass Name: {classInfo.Name}");
        foreach (var member in classInfo.Members)
        {
            Console.WriteLine($"Member Name: {member.Name}");
            if (member.Summary != null)
            {
                Console.WriteLine($"Summary: {member.Summary.Text}");
                if (member.Summary.List != null)
                {
                    foreach (var item in member.Summary.List.Items)
                    {
                        Console.WriteLine($"Term: {item.Term}, Description: {item.Description}");
                    }
                }
            }
            if (member.Remarks != null)
            {
                foreach (var para in member.Remarks.Paras)
                {
                    Console.WriteLine($"Remarks: {para}");
                }
            }
            // Add more print statements for other fields if needed
        }
    }
}
*/