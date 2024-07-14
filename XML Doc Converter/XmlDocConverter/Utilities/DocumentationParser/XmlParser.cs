using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using XmlDocConverter.Models;

namespace XmlDocConverter.Utilities.DocumentationParser
{
    public static class XmlParser : Parser
    {
        /// <summary>
        /// Method for parsing the XML documentation file to C# classes
        /// </summary>
        /// <param name="xmlDoc">XML Document loaded in from the UI</param>
        /// <returns>Returns a list of <seealso cref="ClassDocumentation"/><</returns>
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

                var memberElements = members.Where(m => m.Attribute("name")?.Value.StartsWith($"M:{fullClassName}.") ?? false);

                foreach (var memberElement in memberElements)
                {
                    var memberDoc = new MemberDocumentation
                    {
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
        /// Parses the given <see cref="XElement"/> and extracts the text content,
        /// including handling of <c>paramref</c> elements by formatting them accordingly.
        /// </summary>
        /// <param name="element">The XML element to parse. Can be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="string"/> containing the concatenated text content of the XML element,
        /// with any <c>paramref</c> elements' names enclosed in backticks. If the input element is <c>null</c>,
        /// an empty string is returned.
        /// </returns>
        /// <example>
        /// Given the following XML element:
        /// <code>
        /// <example>
        /// <doc>
        ///   This is an <paramref name="example"/> of XML documentation.
        /// </doc>
        /// </example>
        /// </code>
        /// The method will return:
        /// <code>
        /// "This is an `example` of XML documentation."
        /// </code>
        /// </example>
        /// <remarks>
        /// This method processes the child nodes of the provided <see cref="XElement"/>.
        /// It handles text nodes and <c>paramref</c> elements specifically, while ignoring other types of nodes.
        /// </remarks>
        private static string ParseXmlDocumentation(XElement element)
        {
            if (element == null)
                return string.Empty;

            string text = string.Join("", element.Nodes().Select(node => {
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
