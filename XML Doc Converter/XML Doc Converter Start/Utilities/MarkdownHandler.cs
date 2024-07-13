using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XML_Doc_Converter_Start.Utilities
{
    public static class MarkdownHandler
    {
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
                    var memberNameWithoutPrefix = member.MemberName.StartsWith("M:") ? member.MemberName.Substring(2) : member.MemberName;
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

                    if (member.SeeAlso.Count > 0)
                    {
                        markdown.AppendLine($"**See Also:**");
                        foreach (var seeAlso in member.SeeAlso)
                        {
                            var seeAlsoWithoutPrefix = seeAlso.StartsWith("M:") ? seeAlso.Substring(2) : seeAlso;
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
