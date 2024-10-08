﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XmlDocConverterLibary.Utilities.DocumentationParser
{
    /// <summary>
    /// Class with Utility Methods for the differet Parser classes
    /// <seealso cref="HtmlParser"/>
    /// <seealso cref="XmlParser"/>
    /// <seealso cref="MarkdownParser"/>
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// Method for generating an anchor tag from a member name
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        public static string GenerateAnchor(string memberName)
        {
            // Convert to lowercase, remove invalid characters, and remove spaces
            return Regex.Replace(memberName.ToLower(), @"[^\w]+", "").Trim();
        }

        /// <summary>
        /// Method for cleaning whitespace from a string
        /// </summary>
        /// <param name="input">String that needs to be cleaned</param>
        /// <returns>Returns a cleaned string</returns>
        public static string CleanWhitespace(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return Regex.Replace(input, @"\s+", " ").Trim();
        }
    }
}
