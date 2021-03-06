﻿#region FileInfo

// 
// File: LSLFormatTools.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Eric A. Blundell
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

#endregion

namespace LibLSLCC.Utility
{
    /// <summary>
    ///     Various utilities for formatting string that are useful for generating/dealing with
    ///     indented code and escaped strings.
    /// </summary>
    public static class LSLFormatTools
    {
        /// <summary>
        ///     Gets the number of spaces required to match the length of the whitespace leading up to the first non-whitespace
        ///     character in a string (new line is not considered whitespace here).
        /// </summary>
        /// <param name="str">The string to consider</param>
        /// <param name="tabSize">The size of a tab character in spaces</param>
        /// <returns>
        ///     The number of space characters required to match the length of all the whitespace characters at the end of the
        ///     string (except newlines)
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="str" /> is <c>null</c>.</exception>
        public static int GetStringSpacesIndented(this string str, int tabSize = 4)
        {
            if (str == null) throw new ArgumentNullException("str");

            var columns = 0;

            foreach (var character in str)
            {
                if (char.IsWhiteSpace(character))
                {
                    if (character == '\t')
                    {
                        columns += tabSize;
                    }
                    else if (character == ' ')
                    {
                        columns++;
                    }
                }
                else
                {
                    break;
                }
            }
            return columns;
        }

        private static string FormatFloatExponent(string exponent)
        {
            var expSign = exponent[0].ToString();

            expSign = (expSign == "-" || expSign == "+") ? expSign : "";

            var exp = exponent.Substring(expSign == string.Empty ? 0 : 1);


            var expLen = exp.TrimEnd('f').Length;

            if (exp[0] == '0' && expLen == 1)
            {
                return 'e' + expSign + exp;
            }

            return 'e' + expSign + exp.TrimStart('0');
        }

        /// <summary>
        ///     Format an LSL floating point number string of arbitrary precision from LSL float token format to an equivelant
        ///     string compatiblity with C# float token syntax.<br />
        ///     Specifiers such as 'f' and 'e' are guaranteed to be formatted as lower case characters.  A trailing 'f' specifier
        ///     will remain in the string if present.<br />
        ///     Type specifiers other than 'f' are not accepted.<br />
        ///     <para>
        ///         Example Output:<br />
        ///         "-.00" ->  "-0.0",<br /><br />
        ///         "0.0300" -> "0.03",<br /><br />
        ///         "-3." -> "-3.0",<br /><br />
        ///         ".4E04" -> "0.4e4",<br /><br />
        ///         "4E040" -> "4e40",<br /><br />
        ///         "56.F" -> "56.0f",<br /><br />
        ///         etc...
        ///     </para>
        /// </summary>
        /// <param name="floatStr">The float string to format.</param>
        /// <exception cref="FormatException">
        ///     If <paramref name="floatStr" /> is not a properly formated, LSL float formatting
        ///     rules apply.
        /// </exception>
        /// <returns>The normalized float string.</returns>
        public static string FormatFloatString(this string floatStr)
        {
            if (floatStr == null) throw new ArgumentNullException("floatStr");

            floatStr = floatStr.Trim();

            if (!LSLTokenTools.FloatRegexAnchored.IsMatch(floatStr))
            {
                throw new FormatException("\"" + floatStr + "\" is not a properly formated LSL float.");
            }

            floatStr = floatStr.ToLower();


            string[] exponentParts;

            var parts = floatStr.Split('.');

            if (parts.Length != 2)
            {
                exponentParts = floatStr.Split('e');

                if (exponentParts.Length > 1)
                {
                    return exponentParts[0] + FormatFloatExponent(exponentParts[1]);
                }

                return floatStr;
            }

            var front = parts[0];
            var end = parts[1];

            string negate = "";

            if (front.StartsWith("-"))
            {
                negate = "-";
                front = front.TrimStart('-');
            }

            front = front.TrimStart('0');

            exponentParts = end.Split('e');

            string exponent = "";

            if (exponentParts.Length > 1)
            {
                exponent = FormatFloatExponent(exponentParts[1]);

                end = exponentParts[0];
            }

            if (front.Length == 0)
                front = "0";


            if (end.Length == 0 || end == "0")
                return negate + front + ".0" + exponent;
            if (end == "f")
                return negate + front + ".0f" + exponent;


            string specifier = "";

            if (end.EndsWith("f"))
            {
                specifier = "f";
                end = end.TrimEnd('f');
            }

            return negate + front + "." + end[0] + end.Substring(1).TrimEnd('0') + exponent + specifier;
        }

        /// <summary>
        ///     Gets the number of spaces required to exactly match the length of a given string up to the first new line
        /// </summary>
        /// <param name="str">Input string to get the length in spaces of</param>
        /// <param name="tabSize">Tab size in spaces, defaults to 4</param>
        /// <returns>Number of spaces required to match the length of the string</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str" /> is <c>null</c>.</exception>
        public static int GetStringSpacesEquivalent(this string str, int tabSize = 4)
        {
            if (str == null) throw new ArgumentNullException("str");

            if (str.Length == 0) return 0;

            var columns = 0;

            for (var index = 0; index < str.Length; index++)
            {
                var character = str[index];

                if (char.IsWhiteSpace(character))
                {
                    if (character == '\t')
                    {
                        columns += tabSize;
                    }
                    else if (character == ' ')
                    {
                        columns++;
                    }
                }
                else if (char.IsDigit(character) || char.IsLetter(character) || char.IsSymbol(character) ||
                         char.IsPunctuation(character))
                {
                    columns += 1;
                }
                else if (index + 1 < str.Length && char.IsHighSurrogate(character) &&
                         char.IsLowSurrogate(str[index + 1]))
                {
                    columns += 1;
                    index++;
                }
                else if (character == '\n')
                {
                    break;
                }
            }
            return columns;
        }

        /// <summary>
        ///     Creates a spacer string using tabs up until spaces are required for alignment.
        ///     Strings less than tabSize end up being only spaces.
        /// </summary>
        /// <param name="spaces">The number of spaces the spacer string should be equivalent to</param>
        /// <param name="tabSize">The size of a tab character in spaces, default value is 4</param>
        /// <returns>
        ///     A string consisting of leading tabs and possibly trailing spaces that is equivalent in length
        ///     to the number of spaces provided in the spaces parameter
        /// </returns>
        public static string CreateTabCorrectSpaceString(int spaces, int tabSize = 4)
        {
            var space = "";
            var actual = 0;

            for (var i = 0; i < (spaces/tabSize); i++)
            {
                space += '\t';
                actual += tabSize;
            }

            while (actual < spaces)
            {
                space += ' ';
                actual++;
            }


            return space;
        }

        /// <summary>
        ///     If a string has control codes in it, this will return a string with those control codes
        ///     replaced with their symbolic representation, IE: &#92;n &#92;t ect..
        ///     Supports every escape code supported by C# itself
        /// </summary>
        /// <param name="str">String to replace control codes in.</param>
        /// <returns>String with control codes replaced with symbolic representation.</returns>
        public static string ShowControlCodeEscapes(string str)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(str), writer,
                        new CodeGeneratorOptions {IndentString = "\t"});
                    var literal = writer.ToString();
                    literal = literal.Replace(string.Format("\" +{0}\t\"", Environment.NewLine), "");
                    return literal.Trim('"');
                }
            }
        }

        /// <summary>
        ///     Create a repeating string by repeating the content string a number of times.
        /// </summary>
        /// <param name="repeats">The number of times the 'content' string should repeat.</param>
        /// <param name="content">The content string to repeat.</param>
        /// <returns><paramref name="content" /> concatenated <paramref name="repeats" /> number of times.</returns>
        public static string CreateRepeatingString(int repeats, string content)
        {
            var result = "";
            for (var i = 0; i < repeats; i++) result += content;
            return result;
        }

        /// <summary>
        ///     Generate a string with N number of spaces in it
        /// </summary>
        /// <param name="spaces">Number of spaces</param>
        /// <returns>A string containing 'spaces' number of spaces</returns>
        public static string CreateSpacesString(int spaces)
        {
            return CreateRepeatingString(spaces, " ");
        }

        /// <summary>
        ///     Generate a string with N number of tabs in it
        /// </summary>
        /// <param name="tabs">Number of tabs</param>
        /// <returns>A string containing 'tabs' number of tabs</returns>
        public static string CreateTabsString(int tabs)
        {
            return CreateRepeatingString(tabs, "\t");
        }

        /// <summary>
        ///     Generate a string with N number of newlines in it.
        ///     The newlines are explicitly '\n' characters, not <see cref="Environment.NewLine" />
        /// </summary>
        /// <param name="newLines">Number of newlines</param>
        /// <returns>A string containing 'newLines' number of newlines</returns>
        public static string CreateNewLinesString(int newLines)
        {
            return CreateRepeatingString(newLines, "\n");
        }
    }
}