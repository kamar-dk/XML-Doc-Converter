using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using XmlDocConverter.Models;

namespace XmlDocConverter.Utilities
{
    public class DocumentationParser
    {
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
                    Summary = classElement.Element("summary")?.Value.Trim(),
                    Remarks = classElement.Element("remarks")?.Value.Trim()
                };

                var memberElements = members.Where(m => m.Attribute("name")?.Value.StartsWith($"M:{fullClassName}.") ?? false);

                foreach (var memberElement in memberElements)
                {
                    var memberDoc = new MemberDocumentation
                    {
                        MemberName = memberElement.Attribute("name")?.Value,
                        Summary = memberElement.Element("summary")?.Value.Trim(),
                        Remarks = memberElement.Element("remarks")?.Value.Trim(),
                        Returns = memberElement.Element("returns")?.Value.Trim()
                    };

                    foreach (var paramElement in memberElement.Elements("param"))
                    {
                        memberDoc.Parameters[paramElement.Attribute("name")?.Value] = paramElement.Value.Trim();
                    }

                    foreach (var typeParamElement in memberElement.Elements("typeparam"))
                    {
                        memberDoc.TypeParameters[typeParamElement.Attribute("name")?.Value] = typeParamElement.Value.Trim();
                    }

                    foreach (var exceptionElement in memberElement.Elements("exception"))
                    {
                        memberDoc.Exceptions[exceptionElement.Attribute("cref")?.Value] = exceptionElement.Value.Trim();
                    }

                    foreach (var seeAlsoElement in memberElement.Elements("seealso"))
                    {
                        memberDoc.SeeAlso.Add(seeAlsoElement.Attribute("cref")?.Value);
                    }

                    foreach (var exampleElement in memberElement.Elements("example"))
                    {
                        memberDoc.Examples.Add(exampleElement.Value.Trim());
                    }

                    classDoc.Members.Add(memberDoc);
                }

                classDocs.Add(classDoc);
            }

            return classDocs;
        }

        public static string GenerateMarkdown(List<ClassDocumentation> classDocs)
        {
            var markdown = new System.Text.StringBuilder();

            foreach (var classDoc in classDocs)
            {
                markdown.AppendLine($"# {classDoc.ClassName}");
                markdown.AppendLine();
                markdown.AppendLine($"**Namespace:** {classDoc.Namespace}");
                markdown.AppendLine();
                markdown.AppendLine($"**Summary:**");
                markdown.AppendLine($"{classDoc.Summary}");
                markdown.AppendLine();
                markdown.AppendLine($"**Remarks:**");
                markdown.AppendLine($"{classDoc.Remarks}");
                markdown.AppendLine();

                foreach (var member in classDoc.Members)
                {
                    var memberNameWithoutPrefix = member.MemberName?.StartsWith("M:") ?? false ? member.MemberName.Substring(2) : member.MemberName;
                    markdown.AppendLine($"## {memberNameWithoutPrefix}");
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

            return markdown.ToString();
        }
    }
}
