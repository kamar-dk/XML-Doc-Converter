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

            var sortedNamespaces = namespaces.Keys.OrderBy(ns => ns).ToList();

            // Track already added namespaces to avoid duplicates
            var addedNamespaces = new HashSet<string>();

            foreach (var ns in sortedNamespaces)
            {
                var levels = ns.Split('.');

                for (int i = 0; i < levels.Length; i++)
                {
                    var subNamespace = string.Join(".", levels.Take(i + 1));

                    if (!addedNamespaces.Contains(subNamespace))
                    {
                        var indent = new string(' ', i * 2);
                        markdown.AppendLine($"{indent}- [{subNamespace}](#{GenerateAnchor(subNamespace)})");
                        addedNamespaces.Add(subNamespace);
                    }
                }
            }
            markdown.AppendLine();

            // Generate the documentation for each namespace and its classes
            foreach (var ns in sortedNamespaces)
            {
                markdown.AppendLine($"# {ns}");
                markdown.AppendLine();

                // Generate the table of contents for the classes in this namespace
                markdown.AppendLine($"## Table of Contents");
                markdown.AppendLine();
                foreach (var classDoc in namespaces[ns])
                {
                    markdown.AppendLine($"- [{classDoc.ClassName}](#{GenerateAnchor(classDoc.ClassName)})");
                }
                markdown.AppendLine();

                foreach (var classDoc in namespaces[ns])
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

        public static string GenerateHtml(List<ClassDocumentation> classDocs)
        {
            var html = new StringBuilder();

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
            html.AppendLine("<h1>Table of Contents</h1>");
            html.AppendLine(GenerateNestedToc(namespaces));

            // Generate the documentation for each namespace and its classes
            foreach (var ns in namespaces.Keys.OrderBy(ns => ns))
            {
                html.AppendLine($"<h1 id=\"{GenerateAnchor(ns)}\">{ns}</h1>");

                // Generate the table of contents for the classes in this namespace
                html.AppendLine("<h2>Table of Contents</h2>");
                html.AppendLine("<ul>");
                foreach (var classDoc in namespaces[ns])
                {
                    html.AppendLine($"<li><a href=\"#{GenerateAnchor(classDoc.ClassName)}\">{classDoc.ClassName}</a></li>");
                }
                html.AppendLine("</ul>");

                foreach (var classDoc in namespaces[ns])
                {
                    html.AppendLine($"<h2 id=\"{GenerateAnchor(classDoc.ClassName)}\">{classDoc.ClassName}</h2>");
                    html.AppendLine($"<p><strong>Namespace:</strong> {classDoc.Namespace}</p>");
                    html.AppendLine($"<p><strong>Summary:</strong> {classDoc.Summary}</p>");
                    html.AppendLine($"<p><strong>Remarks:</strong> {classDoc.Remarks}</p>");

                    if (classDoc.Members.Count > 0)
                    {
                        html.AppendLine("<h3>Members</h3>");
                        html.AppendLine("<table>");
                        html.AppendLine("<tr><th>Name</th><th>Summary</th></tr>");

                        foreach (var member in classDoc.Members)
                        {
                            var memberNameWithoutPrefix = member.MemberName?.StartsWith("M:") ?? false ? member.MemberName.Substring(2) : member.MemberName;
                            var memberAnchor = GenerateAnchor(memberNameWithoutPrefix);
                            html.AppendLine($"<tr><td><a href=\"#{memberAnchor}\">{memberNameWithoutPrefix}</a></td><td>{member.Summary}</td></tr>");
                        }

                        html.AppendLine("</table>");
                    }

                    foreach (var member in classDoc.Members)
                    {
                        var memberNameWithoutPrefix = member.MemberName?.StartsWith("M:") ?? false ? member.MemberName.Substring(2) : member.MemberName;
                        var memberAnchor = GenerateAnchor(memberNameWithoutPrefix);
                        html.AppendLine($"<h3 id=\"{memberAnchor}\">{memberNameWithoutPrefix}</h3>");
                        html.AppendLine($"<p><strong>Summary:</strong> {member.Summary}</p>");
                        html.AppendLine($"<p><strong>Remarks:</strong> {member.Remarks}</p>");

                        if (member.Parameters.Count > 0)
                        {
                            html.AppendLine("<p><strong>Parameters:</strong></p><ul>");
                            foreach (var param in member.Parameters)
                            {
                                html.AppendLine($"<li><strong>{param.Key}:</strong> {param.Value}</li>");
                            }
                            html.AppendLine("</ul>");
                        }

                        if (member.TypeParameters.Count > 0)
                        {
                            html.AppendLine("<p><strong>Type Parameters:</strong></p><ul>");
                            foreach (var typeParam in member.TypeParameters)
                            {
                                html.AppendLine($"<li><strong>{typeParam.Key}:</strong> {typeParam.Value}</li>");
                            }
                            html.AppendLine("</ul>");
                        }

                        if (member.Exceptions.Count > 0)
                        {
                            html.AppendLine("<p><strong>Exceptions:</strong></p><ul>");
                            foreach (var exception in member.Exceptions)
                            {
                                html.AppendLine($"<li><strong>{exception.Key}:</strong> {exception.Value}</li>");
                            }
                            html.AppendLine("</ul>");
                        }

                        if (!string.IsNullOrEmpty(member.Returns))
                        {
                            html.AppendLine($"<p><strong>Returns:</strong> {member.Returns}</p>");
                        }

                        if (member.SeeAlso.Count > 0)
                        {
                            html.AppendLine("<p><strong>See Also:</strong></p><ul>");
                            foreach (var seeAlso in member.SeeAlso)
                            {
                                var seeAlsoWithoutPrefix = seeAlso?.StartsWith("M:") ?? false ? seeAlso.Substring(2) : seeAlso;
                                html.AppendLine($"<li>{seeAlsoWithoutPrefix}</li>");
                            }
                            html.AppendLine("</ul>");
                        }

                        if (member.Examples.Count > 0)
                        {
                            html.AppendLine("<p><strong>Examples:</strong></p>");
                            foreach (var example in member.Examples)
                            {
                                html.AppendLine("<pre><code>");
                                html.AppendLine($"{example}");
                                html.AppendLine("</code></pre>");
                            }
                        }
                    }
                    html.AppendLine("<hr>");
                }
            }

            return html.ToString();
        }

        private static string GenerateNestedToc(Dictionary<string, List<ClassDocumentation>> namespaces)
        {
            var sortedNamespaces = namespaces.Keys.OrderBy(ns => ns).ToList();
            return GenerateNestedTocRecursive(sortedNamespaces, 0);
        }

        private static string GenerateNestedTocRecursive(List<string> namespaces, int level)
        {
            var html = new StringBuilder();
            html.AppendLine("<ul>");
            for (int i = 0; i < namespaces.Count; i++)
            {
                var currentNamespace = namespaces[i];
                var nextNamespace = i + 1 < namespaces.Count ? namespaces[i + 1] : null;

                var indent = new string(' ', level * 2);
                html.AppendLine($"{indent}<li><a href=\"#{GenerateAnchor(currentNamespace)}\">{currentNamespace}</a></li>");

                if (nextNamespace != null && nextNamespace.StartsWith(currentNamespace + "."))
                {
                    var nestedNamespaces = namespaces.Skip(i + 1).TakeWhile(ns => ns.StartsWith(currentNamespace + ".")).ToList();
                    html.AppendLine(GenerateNestedTocRecursive(nestedNamespaces, level + 1));
                    i += nestedNamespaces.Count;
                }
            }
            html.AppendLine("</ul>");
            return html.ToString();
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
