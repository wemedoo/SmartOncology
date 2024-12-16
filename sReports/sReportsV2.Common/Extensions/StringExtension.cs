using sReportsV2.Common.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace sReportsV2.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceNonAlphaCharactersWithDash(this string text)
        {
            char[] arr = text.Select(c => (char.IsLetterOrDigit(c) || c == '-') ? c : '-').ToArray();
            return new string(arr);
        }
        public static string RemoveDiacritics(this string s)
        {
            s = Ensure.IsNotNull(s, nameof(s));

            String normalizedString = s.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                Char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        public static string CapitalizeFirstLetter(this string text)
        {
            string retText = string.Empty;
            if (!string.IsNullOrEmpty(text) && text.Length > 0)
            {
                if (text.Length == 1)
                {
                    retText = char.ToUpper(text[0]).ToString();
                }
                else
                {
                    retText = char.ToUpper(text[0]) + text.Substring(1).ToLower();
                }

            }
            return retText;
        }

        public static string CapitalizeOnlyFirstLetter(this string text)
        {
            string retText = string.Empty;
            if (!string.IsNullOrEmpty(text) && text.Length > 0)
            {
                string firstLetterUp = char.ToUpper(text[0]).ToString();
                retText = firstLetterUp + text.Substring(1);
            }
            return retText;
        }

        public static string TrimInput(this string inputText)
        {
            return string.IsNullOrEmpty(inputText) ? string.Empty : inputText.Trim();
        }

        public static string GetFileNameFromUri(this string uri, bool excludeGUIDPartFromName = true)
        {
            string fileName = "";
            if (!string.IsNullOrWhiteSpace(uri))
            {
                string resourceNameRaw = GetResourceNameFromUri(uri);
                fileName = excludeGUIDPartFromName ? string.Join("", resourceNameRaw.Split('_').Skip(1)) : resourceNameRaw; // The first is always the GUID
            }
            return fileName;
        }

        public static string GetResourceNameFromUri(this string uri)
        {
            char resourceSeparator = '/';
            return !string.IsNullOrWhiteSpace(uri) ? WebUtility.UrlDecode(uri.Split(resourceSeparator).LastOrDefault()) : "";
        }

        public static string PrepareForMongoStrictTextSearch(this string s)
        {
            return s.EncloseStringInDoubleQuotes();
        }

        public static string EncloseStringInDoubleQuotes(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
                s = "\"" + s + "\"";

            return s;
        }

        public static string EscapeSqlString(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            } 
            else
            {
                return text.Replace("'", "''");
            }
        }

        public static string SanitizeForCsvExport(this string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                // Csv specifications suggests of replacing quotes with double quotes
                if (s.Contains("\""))
                {
                    s = s.Replace("\"", "\"\"");
                }

                // Putting csv value between double quotes
                s = String.Format("\"{0}\"", s);
            }
            return s;
        }

        public static string FormatCsvHeaderCell(this string s, bool fieldSetInstanceRepetition=true)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                string[] splits = s.Split('_');

                if (splits.Length > 1)
                {
                    List<string> strings = splits.Skip(1).ToList();

                    if(strings.Count == 3)
                    {
                        string fieldSetInstanceRepetitionNumber = fieldSetInstanceRepetition && int.TryParse(strings[0], out int s0) ? s0+1 + "." : "";  // FieldSet Number starting from 1
                        string label = strings[1];
                        string FieldInstanceRepetitionNumber = fieldSetInstanceRepetition || strings[2] != "0" ? strings[2] : "";

                        s = label + " " + fieldSetInstanceRepetitionNumber + FieldInstanceRepetitionNumber;
                    }
                }
            }
            return s;
        }

        /// <summary>
        /// Splits the string with splitSeparator and returns only the desiredSplitNumber tokens, joint with joinSeparator
        /// </summary>
        /// <param name="s"></param>
        /// <param name="splitSeparator"></param>
        /// <param name="joinSeparator"></param>
        /// <param name="desiredSplitsNumber"></param>
        /// <returns></returns>
        public static string SplitAndTake(this string s, char splitSeparator, string joinSeparator ,int desiredSplitsNumber)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                string[] splits = s.Split(splitSeparator);

                if (splits.Length >= desiredSplitsNumber)
                {
                    s = String.Join(joinSeparator, splits.Take(desiredSplitsNumber));
                }
            }
            return s;
        }

        public static string FormatComplexColumnFromView(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] complexColumnDelimiters = { $"<{Delimiters.ComplexColumnDelimiter}>", $"</{Delimiters.ComplexColumnDelimiter}>" };
                return string.Join(", ", s.Split(complexColumnDelimiters, StringSplitOptions.RemoveEmptyEntries));
            }
            return s;
        }

        public static string ToJsBoolean(this bool value)
        {
            return value.ToString().ToLower();
        }

        /// <summary>
        /// Sets the first char of each word in Upper Case (e.g. "my sentence" -> "My Sentence")
        /// </summary>
        public static string ToTitleCase(this string s)
        {
            return string.Join(" ", s.Split(' ').Where(word => word.Length > 0).Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        }

        /// <summary>
        /// Handles commas inside the CSV row.
        /// Example: "0,1000,\"Waadt, Lausanne\"" -> ["0", "1000", "Waadt, Lausanne"]
        /// </summary>
        public static string[] SplitCsvRow(this string row)
        {
            string pattern = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            string[] values = Regex.Split(row, pattern);

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i].Trim('"');
            }

            return values;
        }

        public static string CombineFilePath(this string directory, string sanitizedFileName)
        {
            string outputPath = Path.Combine(directory, sanitizedFileName + ".zip");

            return outputPath;
        }

        public static string SanitizeFileName(this string fileName)
        {
            return ReplaceInvalidCharacters(fileName, "_");
        }

        private static string ReplaceInvalidCharacters(this string input, string replacement)
        {
            input = input.Replace("/", replacement);
            return input;
        }

        private static readonly List<string> FieldSelectableTypes = new List<string>()
        {
            FieldTypes.Radio,
            FieldTypes.Select,
            FieldTypes.Checkbox
        };

        public static bool IsSelectableField(this string fieldType)
        {
            return FieldSelectableTypes.Contains(fieldType);
        }

        public static byte[] Base64Encode(this string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string base64String = Convert.ToBase64String(plainTextBytes);
            return Convert.FromBase64String(base64String);
        }

        public static int? ExtractIntFromString(this string stringWithNumbers)
        {
            int? result = null;
            if (!string.IsNullOrWhiteSpace(stringWithNumbers))
            {
                Match match = Regex.Match(stringWithNumbers, @"\d+");  // e.g. "#a#1234" -> 1234
                if (match.Success && int.TryParse(match.Value, out int value))
                    result = value;
            }
            return result;
        }

        public static string ExtractTextFromHtml(this string html)
        {
            string decodedHtml = HttpUtility.HtmlDecode(html);
            string extractedText = Regex.Replace(decodedHtml, "<.*?>", String.Empty);  // Remove HTML tags
            extractedText = extractedText.Replace("\u00A0", " ");  // Handle non-breaking spaces
            return extractedText;
        }
    }
}