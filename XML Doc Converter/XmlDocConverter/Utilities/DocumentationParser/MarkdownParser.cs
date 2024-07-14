using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlDocConverter.Models;

namespace XmlDocConverter.Utilities.DocumentationParser
{
    /// <summary>
    /// Parser class for passing XML Documentation to Markdown
    /// Inherits from <seealso cref="Parser"/>
    /// </summary>
    public class MarkdownParser : Parser
    {
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
    }
}
