using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chapters
{
    public static class StringExtensions
    {
        public static int GetNumberOfUpperChar(this string text)
        {
            Char[] array = text.ToCharArray();
            int upperCounter = 0;
            foreach (Char ch in array)
            {
                upperCounter += char.IsUpper(ch) ? 1 : 0;
            }

            return upperCounter;
        }
        public static List<string> SplitInParts(this string s, int partLength)
        {
            List<string> listOfValues = new List<string>();

            for (var i = 0; i < s.Length; i += partLength)
                listOfValues.Add(s.Substring(i, Math.Min(partLength, s.Length - i)));

            return listOfValues;
        }

        public static List<string> GetRows(this string value, int rowLength)
        {
            List<string> rows = new List<string>();
            string[] words = value.Split(' ');
            StringBuilder textBuilder = new StringBuilder();
            int upperCounter = 0;

            for (int i = 0; i < words.Length; i++)
            {
                if ((textBuilder.Length + words[i].Length) < rowLength - upperCounter / 3)
                {
                    upperCounter += words[i].GetNumberOfUpperChar();
                    textBuilder.Append(words[i]).Append(" ");
                }
                else
                {
                    rows.Add(textBuilder.ToString().Trim());
                    textBuilder.Clear();
                    upperCounter = 0;
                    textBuilder.Append(words[i]).Append(" ");
                }

                if (i == words.Length - 1)
                {
                    rows.Add(textBuilder.ToString().Trim());
                }
            }

            return rows;
        }

        public static string ReplaceAllBr(this string value) 
        {
            return value.Replace("<br>","");
        }
    }
}
