using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlDocConverterLibary.Models;

namespace XmlDocConverterLibary.Utilities.DocumentationParser
{
    /// <summary>
    /// Class for parsing the XML Documentation to C# instance of classes
    /// </summary>
    public class XmlParser : Parser
    {
        /// <summary>
        /// Method for parsing the XML documentation file to C# classes
        /// </summary>
        /// <param name="xmlDoc">XML Document loaded in from the UI</param>
        /// <returns>Returns a list of <seealso cref="ClassDocumentation"/></returns>
        public static List<ClassDocumentation> ParseDocumentation(XDocument xmlDoc)
        {
            var classDocs = new List<ClassDocumentation>();

            var members = xmlDoc.Descendants("member");
            var classes = members.Where(m => m.Attribute("name")?.Value.StartsWith("T:") ?? false);

            foreach (var classElement in classes)
            {
                var fullClassName = classElement.Attribute("name")?.Value.Substring(2);
                if (fullClassName == null) continue;

                var classDoc = new ClassDocumentation
                {
                    ClassName = fullClassName.Split('.').Last(),
                    Namespace = string.Join(".", fullClassName.Split('.').SkipLast(1)),
                    Summary = CleanWhitespace(ParseXmlDocumentation(classElement.Element("summary"))),
                    Remarks = CleanWhitespace(ParseXmlDocumentation(classElement.Element("remarks")))
                };

                // Include properties, fields, and methods
                var memberElements = members.Where(m =>
                    m.Attribute("name")?.Value.StartsWith($"M:{fullClassName}.") == true ||
                    m.Attribute("name")?.Value.StartsWith($"P:{fullClassName}.") == true ||
                    m.Attribute("name")?.Value.StartsWith($"F:{fullClassName}.") == true);

                foreach (var memberElement in memberElements)
                {
                    var memberType = memberElement.Attribute("name")?.Value[0] switch
                    {
                        'M' => "Method",
                        'P' => "Property",
                        'F' => "Field",
                        _ => "Unknown"
                    };

                    var memberDoc = new MemberDocumentation
                    {
                        MemberType = memberType,
                        MemberName = memberElement.Attribute("name")?.Value,
                        Summary = CleanWhitespace(ParseXmlDocumentation(memberElement.Element("summary"))),
                        Remarks = CleanWhitespace(ParseXmlDocumentation(memberElement.Element("remarks"))),
                        Returns = CleanWhitespace(ParseXmlDocumentation(memberElement.Element("returns")))
                    };

                    foreach (var paramElement in memberElement.Elements("param"))
                    {
                        memberDoc.Parameters[paramElement.Attribute("name")?.Value] = CleanWhitespace(paramElement.Value.Trim());
                    }

                    foreach (var typeParamElement in memberElement.Elements("typeparam"))
                    {
                        memberDoc.TypeParameters[typeParamElement.Attribute("name")?.Value] = CleanWhitespace(typeParamElement.Value.Trim());
                    }

                    foreach (var exceptionElement in memberElement.Elements("exception"))
                    {
                        memberDoc.Exceptions[exceptionElement.Attribute("cref")?.Value] = CleanWhitespace(exceptionElement.Value.Trim());
                    }

                    foreach (var seeAlsoElement in memberElement.Elements("seealso"))
                    {
                        memberDoc.SeeAlso.Add(seeAlsoElement.Attribute("cref")?.Value);
                    }

                    foreach (var exampleElement in memberElement.Elements("example"))
                    {
                        memberDoc.Examples.Add(CleanWhitespace(exampleElement.Value.Trim()));
                    }

                    classDoc.Members.Add(memberDoc);
                }

                classDocs.Add(classDoc);
            }

            return classDocs;
        }

        /// <summary>
        ///  Method for parsing the XML documentation to a string
        /// </summary>
        /// <param name="element">Element from the XML documentation</param>
        /// <returns>Returns the text of the XML Documentation</returns>
        private static string ParseXmlDocumentation(XElement element)
        {
            if (element == null)
                return string.Empty;

            string text = string.Join("", element.Nodes().Select(node =>
            {
                if (node is XText textNode)
                {
                    return textNode.Value;
                }
                else if (node is XElement el && el.Name == "paramref")
                {
                    return $"`{el.Attribute("name")?.Value}`";
                }
                return "";
            }));

            return text;
        }
    }
}
