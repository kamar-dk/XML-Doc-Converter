using System.Text;
using XmlDocConverterLibary.Models;

namespace XmlDocConverterLibary.Parsers
{
    /// <summary>
    /// Class for parsing documentation models into BitBucket Markdown format.
    /// </summary>
    public class BitBucketMarkdownParser
    {
        /// <summary>
        /// Converts the ClassDocumentation object to BitBucket compatible markdown.
        /// </summary>
        /// <param name="classDoc">The ClassDocumentation object.</param>
        /// <returns>A string representing the markdown content.</returns>
        public string ParseClassDocumentation(ClassDocumentation classDoc)
        {
            StringBuilder markdown = new StringBuilder();

            // Class header
            markdown.AppendLine($"# {classDoc.ClassName}");
            markdown.AppendLine();
            markdown.AppendLine($"**Namespace:** {classDoc.Namespace}");
            markdown.AppendLine();
            markdown.AppendLine($"**Summary:** {classDoc.Summary}");
            markdown.AppendLine();

            if (!string.IsNullOrEmpty(classDoc.Remarks))
            {
                markdown.AppendLine($"**Remarks:** {classDoc.Remarks}");
                markdown.AppendLine();
            }

            // Process each member
            foreach (var member in classDoc.Members)
            {
                markdown.Append(ParseMemberDocumentation(member));
            }

            return markdown.ToString();
        }

        /// <summary>
        /// Converts the MemberDocumentation object to BitBucket compatible markdown.
        /// </summary>
        /// <param name="memberDoc">The MemberDocumentation object.</param>
        /// <returns>A string representing the markdown content.</returns>
        public string ParseMemberDocumentation(MemberDocumentation memberDoc)
        {
            StringBuilder markdown = new StringBuilder();

            markdown.AppendLine($"## {memberDoc.MemberName} ({memberDoc.MemberType})");
            markdown.AppendLine();
            markdown.AppendLine($"**Summary:** {memberDoc.Summary}");
            markdown.AppendLine();

            if (!string.IsNullOrEmpty(memberDoc.Remarks))
            {
                markdown.AppendLine($"**Remarks:** {memberDoc.Remarks}");
                markdown.AppendLine();
            }

            if (!string.IsNullOrEmpty(memberDoc.Returns))
            {
                markdown.AppendLine($"**Returns:** {memberDoc.Returns}");
                markdown.AppendLine();
            }

            if (!string.IsNullOrEmpty(memberDoc.Value))
            {
                markdown.AppendLine($"**Value:** {memberDoc.Value}");
                markdown.AppendLine();
            }

            if (!string.IsNullOrEmpty(memberDoc.Permission))
            {
                markdown.AppendLine($"**Permission:** {memberDoc.Permission}");
                markdown.AppendLine();
            }

            if (memberDoc.InheritDoc)
            {
                markdown.AppendLine($"**Inherited Documentation**");
                markdown.AppendLine();
            }

            if (!string.IsNullOrEmpty(memberDoc.CompletionList))
            {
                markdown.AppendLine($"**Completion List:** {memberDoc.CompletionList}");
                markdown.AppendLine();
            }

            if (memberDoc.SeeAlso != null && memberDoc.SeeAlso.Count > 0)
            {
                markdown.AppendLine("**See Also:**");
                foreach (var seeAlso in memberDoc.SeeAlso)
                {
                    markdown.AppendLine($"- {seeAlso}");
                }
                markdown.AppendLine();
            }

            if (memberDoc.Examples != null && memberDoc.Examples.Count > 0)
            {
                markdown.AppendLine("**Examples:**");
                foreach (var example in memberDoc.Examples)
                {
                    markdown.AppendLine($"- {example}");
                }
                markdown.AppendLine();
            }

            if (!string.IsNullOrEmpty(memberDoc.Overloads))
            {
                markdown.AppendLine($"**Overloads:** {memberDoc.Overloads}");
                markdown.AppendLine();
            }

            if (memberDoc.Lists != null && memberDoc.Lists.Count > 0)
            {
                markdown.AppendLine("**Lists:**");
                foreach (var list in memberDoc.Lists)
                {
                    markdown.AppendLine($"- {list}");
                }
                markdown.AppendLine();
            }

            if (memberDoc.Parameters != null && memberDoc.Parameters.Count > 0)
            {
                markdown.AppendLine("**Parameters:**");
                foreach (var param in memberDoc.Parameters)
                {
                    markdown.AppendLine($"- **{param.Key}:** {param.Value}");
                }
                markdown.AppendLine();
            }

            if (memberDoc.TypeParameters != null && memberDoc.TypeParameters.Count > 0)
            {
                markdown.AppendLine("**Type Parameters:**");
                foreach (var typeParam in memberDoc.TypeParameters)
                {
                    markdown.AppendLine($"- **{typeParam.Key}:** {typeParam.Value}");
                }
                markdown.AppendLine();
            }

            if (memberDoc.Exceptions != null && memberDoc.Exceptions.Count > 0)
            {
                markdown.AppendLine("**Exceptions:**");
                foreach (var exception in memberDoc.Exceptions)
                {
                    markdown.AppendLine($"- **{exception.Key}:** {exception.Value}");
                }
                markdown.AppendLine();
            }

            return markdown.ToString();
        }
    }
}
