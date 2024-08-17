using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XmlDocConverterLibary.Models;

namespace XmlDocConverterLibary.Utilities.DocumentationParser
{
    /// <summary>
    /// Markdown Parser that generates Markdown where the anchor tags are compatible with BitBucket
    /// </summary>
    public class BitBucketMarkdownParser : MarkdownParser
    {
        /// <summary>
        /// Overridden method for generating a header with an anchor tag
        /// </summary>
        /// <param name="header">The header text</param>
        /// <param name="level">The level of the header</param>
        /// <returns>Returns a string with the anchor tag and the header</returns>
        public override string GenerateHeader(string header, int level)
        {
            var anchor = GenerateAnchor(header);
            return $"<a name=\"{anchor}\"></a>\n{new string('#', level)} {header}";
        }
    }
}