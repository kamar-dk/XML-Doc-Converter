using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using XmlDocConverter.Models;

namespace XmlDocConverter.Utilities
{
    /// <summary>
    /// Class for parsing XML documentation from XML to C# classes
    /// Got method to convert create Markdown document with the documentation
    /// </summary>
    public class DocumentationParser
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
        /// Method for generation a Markdown document
        /// </summary>
        /// <param name="classDocs">List of <seealso cref="ClassDocumentation"/> with the documentation</param>
        /// <returns>Returns a string with the Markdown</returns>
        public static string GenerateMarkdown(List<ClassDocumentation> classDocs)
        {
            var markdown = new StringBuilder();

            // Create a dictionary to store namespaces and their respective classes
            var namespaces = new Dictionary<string, List<ClassDocumentation>>();
            foreach (var classDoc in classDocs)
            {
                if (!namespaces.ContainsKey(classDoc.Namespace))
                {
                    namespaces[classDoc.Namespace] = new List<ClassDocumentation>();
                }
                namespaces[classDoc.Namespace].Add(classDoc);
            }

            // Generate the table of contents
            markdown.AppendLine("# Table of Contents");
            markdown.AppendLine();
            foreach (var ns in namespaces)
            {
                markdown.AppendLine($"- [{ns.Key}](#{GenerateAnchor(ns.Key)})");
            }
            markdown.AppendLine();

            // Generate the documentation for each namespace and its classes
            foreach (var ns in namespaces)
            {
                markdown.AppendLine($"# {ns.Key}");
                markdown.AppendLine();

                // Generate the table of contents for the classes in this namespace
                markdown.AppendLine($"## Table of Contents");
                markdown.AppendLine();
                foreach (var classDoc in ns.Value)
                {
                    markdown.AppendLine($"- [{classDoc.ClassName}](#{GenerateAnchor(classDoc.ClassName)})");
                }
                markdown.AppendLine();

                foreach (var classDoc in ns.Value)
                {
                    markdown.AppendLine($"## {classDoc.ClassName}");
                    markdown.AppendLine();
                    markdown.AppendLine($"**Namespace:** {classDoc.Namespace}");
                    markdown.AppendLine();
                    markdown.AppendLine($"**Summary:**");
                    markdown.AppendLine($"{classDoc.Summary}");
                    markdown.AppendLine();
                    markdown.AppendLine($"**Remarks:**");
                    markdown.AppendLine($"{classDoc.Remarks}");
                    markdown.AppendLine();

                    if (classDoc.Members.Count > 0)
                    {
                        markdown.AppendLine($"### Members");
                        markdown.AppendLine();
                        markdown.AppendLine($"| Name | Summary |");
                        markdown.AppendLine($"| --- | --- |");

                        foreach (var member in classDoc.Members)
                        {
                            var memberNameWithoutPrefix = member.MemberName?.StartsWith("M:") ?? false ? member.MemberName.Substring(2) : member.MemberName;
                            var memberAnchor = GenerateAnchor(memberNameWithoutPrefix);
                            markdown.AppendLine($"| [{memberNameWithoutPrefix}](#{memberAnchor}) | {member.Summary} |");
                        }

                        markdown.AppendLine();
                    }

                    foreach (var member in classDoc.Members)
                    {
                        var memberNameWithoutPrefix = member.MemberName?.StartsWith("M:") ?? false ? member.MemberName.Substring(2) : member.MemberName;
                        var memberAnchor = GenerateAnchor(memberNameWithoutPrefix);
                        markdown.AppendLine($"### {memberNameWithoutPrefix}");
                        markdown.AppendLine();
                        markdown.AppendLine($"**Summary:**");
                        markdown.AppendLine($"{member.Summary}");
                        markdown.AppendLine();
                        markdown.AppendLine($"**Remarks:**");
                        markdown.AppendLine($"{member.Remarks}");
                        markdown.AppendLine();

                        if (member.Parameters.Count > 0)
                        {
                            markdown.AppendLine($"**Parameters:**");
                            foreach (var param in member.Parameters)
                            {
                                markdown.AppendLine($"- **{param.Key}:** {param.Value}");
                            }
                            markdown.AppendLine();
                        }

                        if (member.TypeParameters.Count > 0)
                        {
                            markdown.AppendLine($"**Type Parameters:**");
                            foreach (var typeParam in member.TypeParameters)
                            {
                                markdown.AppendLine($"- **{typeParam.Key}:** {typeParam.Value}");
                            }
                            markdown.AppendLine();
                        }

                        if (member.Exceptions.Count > 0)
                        {
                            markdown.AppendLine($"**Exceptions:**");
                            foreach (var exception in member.Exceptions)
                            {
                                markdown.AppendLine($"- **{exception.Key}:** {exception.Value}");
                            }
                            markdown.AppendLine();
                        }

                        if (!string.IsNullOrEmpty(member.Returns))
                        {
                            markdown.AppendLine($"**Returns:**");
                            markdown.AppendLine($"{member.Returns}");
                            markdown.AppendLine();
                        }

                        if (member.SeeAlso.Count > 0)
                        {
                            markdown.AppendLine($"**See Also:**");
                            foreach (var seeAlso in member.SeeAlso)
                            {
                                var seeAlsoWithoutPrefix = seeAlso?.StartsWith("M:") ?? false ? seeAlso.Substring(2) : seeAlso;
                                markdown.AppendLine($"- {seeAlsoWithoutPrefix}");
                            }
                            markdown.AppendLine();
                        }

                        if (member.Examples.Count > 0)
                        {
                            markdown.AppendLine($"**Examples:**");
                            foreach (var example in member.Examples)
                            {
                                markdown.AppendLine($"```");
                                markdown.AppendLine($"{example}");
                                markdown.AppendLine($"```");
                            }
                            markdown.AppendLine();
                        }
                    }
                    markdown.AppendLine("---");
                    markdown.AppendLine();
                }
            }

            return markdown.ToString();
        }

        /// <summary>
        /// Method for cleaning whitespace from a string
        /// </summary>
        /// <param name="input">String that needs to be cleaned</param>
        /// <returns>Returns a cleaned string</returns>
        private static string CleanWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(input, @"\s+", " ").Trim();
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

        private static string GenerateAnchor(string memberName)
        {
            // Convert to lowercase, remove invalid characters, and remove spaces
            return Regex.Replace(memberName.ToLower(), @"[^\w]+", "").Trim();
        }
    }
}
