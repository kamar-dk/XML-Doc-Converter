using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlDocConverter.Models;

namespace XmlDocConverter.Utilities.DocumentationParser
{
    /// <summary>
    /// Parser class for passing XML Documentation to HTML
    /// Inherits from <seealso cref="Parser"/>
    /// </summary>
    public class HtmlParser : Parser
    {
        /// <summary>
        /// Method for generating an HTML document with documentation
        /// </summary>
        /// <param name="classDocs">List of <seealso cref="ClassDocumentation"/> with the documentation</param>
        /// <returns>Returns string containing the HTML code for the document</returns>
        public static string GenerateHtml(List<ClassDocumentation> classDocs)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("<meta charset=\"UTF-8\">");
            html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("<title>Documentation</title>");
            html.AppendLine("<link rel=\"stylesheet\" href=\"styles.css\">");
            html.AppendLine("</head>");

            html.AppendLine("<body>");
            html.AppendLine("<div class=\"sidebar\">");
            html.AppendLine("<h2>Table of Contents</h2>");
            html.AppendLine("<ul>");

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

            // Generate nested list structure for namespaces
            GenerateNamespaceList(html, namespaces);

            html.AppendLine("</ul>");
            html.AppendLine("</div>");

            html.AppendLine("<div class=\"content\">");

            // Generate the documentation for each namespace and its classes
            foreach (var ns in namespaces.Keys.OrderBy(ns => ns))
            {
                html.AppendLine($"<h2 id=\"{GenerateAnchor(ns)}\">{ns}</h2>");
                html.AppendLine("<ul>");
                foreach (var classDoc in namespaces[ns])
                {
                    html.AppendLine($"<li><a href=\"#{GenerateAnchor(classDoc.ClassName)}\">{classDoc.ClassName}</a></li>");
                }
                html.AppendLine("</ul>");

                foreach (var classDoc in namespaces[ns])
                {
                    html.AppendLine($"<h3 id=\"{GenerateAnchor(classDoc.ClassName)}\">{classDoc.ClassName}</h3>");
                    html.AppendLine($"<p><strong>Namespace:</strong> {classDoc.Namespace}</p>");
                    if (!string.IsNullOrEmpty(classDoc.Summary))
                    {
                        html.AppendLine($"<p><strong>Summary:</strong> {classDoc.Summary}</p>");
                    }
                    if (!string.IsNullOrEmpty(classDoc.Remarks))
                    {
                        html.AppendLine($"<p><strong>Remarks:</strong> {classDoc.Remarks}</p>");
                    }

                    if (classDoc.Members.Count > 0)
                    {
                        html.AppendLine("<h4>Members</h4>");
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
                        html.AppendLine($"<h4 id=\"{memberAnchor}\">{memberNameWithoutPrefix}</h4>");
                        if (!string.IsNullOrEmpty(member.Summary))
                        {
                            html.AppendLine($"<p><strong>Summary:</strong> {member.Summary}</p>");
                        }
                        if (!string.IsNullOrEmpty(member.Remarks))
                        {
                            html.AppendLine($"<p><strong>Remarks:</strong> {member.Remarks}</p>");
                        }

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
            html.AppendLine("</div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        private static void GenerateNamespaceList(StringBuilder html, Dictionary<string, List<ClassDocumentation>> namespaces)
        {
            var nsHierarchy = BuildNamespaceHierarchy(namespaces.Keys);

            foreach (var nsNode in nsHierarchy)
            {
                GenerateNamespaceNode(html, nsNode);
            }
        }

        private static void GenerateNamespaceNode(StringBuilder html, NamespaceNode nsNode)
        {
            html.AppendLine($"<li><a href=\"#{GenerateAnchor(nsNode.FullName)}\">{nsNode.Name}</a></li>");

            if (nsNode.Children.Any())
            {
                html.AppendLine("<ul>");
                foreach (var child in nsNode.Children)
                {
                    GenerateNamespaceNode(html, child);
                }
                html.AppendLine("</ul>");
            }
        }

        private static List<NamespaceNode> BuildNamespaceHierarchy(IEnumerable<string> namespaces)
        {
            var rootNodes = new List<NamespaceNode>();

            foreach (var ns in namespaces.OrderBy(n => n))
            {
                var parts = ns.Split('.');
                NamespaceNode currentNode = null;

                foreach (var part in parts)
                {
                    var fullName = currentNode == null ? part : $"{currentNode.FullName}.{part}";
                    var existingNode = currentNode?.Children.FirstOrDefault(n => n.Name == part) ??
                                       rootNodes.FirstOrDefault(n => n.Name == part && n.FullName == fullName);

                    if (existingNode == null)
                    {
                        var newNode = new NamespaceNode { Name = part, FullName = fullName };
                        if (currentNode == null)
                        {
                            rootNodes.Add(newNode);
                        }
                        else
                        {
                            currentNode.Children.Add(newNode);
                        }
                        currentNode = newNode;
                    }
                    else
                    {
                        currentNode = existingNode;
                    }
                }
            }

            return rootNodes;
        }

        private class NamespaceNode
        {
            public string Name { get; set; }
            public string FullName { get; set; }
            public List<NamespaceNode> Children { get; } = new List<NamespaceNode>();
        }

        private static string GenerateAnchor(string name)
        {
            return name.Replace('.', '-');
        }
    }
}
