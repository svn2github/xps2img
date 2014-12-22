using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandLine.Utils
{
    public static class ColumnFormatter
    {
        public const char Separator = '\x07';
        private static readonly char[] SeparatorArray = { Separator };

        private const int TabSize = 8;

        public static StringBuilder Format(IList<string> lines)
        {
            if (lines == null || !lines.Any())
            {
                return new StringBuilder(String.Empty);
            }

            var splitted = new List<string[]>(lines.Count()) { lines.First().Split(SeparatorArray) };

            // Max length for each column.
            var maxLength = new int[splitted[0].Length];

            // Initial lengthes.
            for (var i = 0; i < maxLength.Length; i++)
            {
                maxLength[i] = splitted[0][i].Length;
            }

            var resultLength = 0;

            // Data.
            for (var row = 1; row < lines.Count(); row++)
            {
                splitted.Add(lines.ElementAt(row).Split(Separator));
                var addedRow = splitted[row];
                // Search for max length.
                for (var col = 0; col < maxLength.Length && col < addedRow.Length; col++)
                {
                    var len = addedRow[col].Length;
                    if (maxLength[col] < len)
                    {
                        maxLength[col] = len;
                    }
                    resultLength += len;
                }
            }

            // Calculate number of tabs.
            for (var col = 0; col < maxLength.Length; col++)
            {
                maxLength[col] = (maxLength[col] / TabSize) + 1;
            }

            var stringBuilder = new StringBuilder((int)(resultLength * 1.2));

            // Format tabs.
            foreach (var row in splitted)
            {
                for (var col = 0; col < maxLength.Length && col < row.Length; col++)
                {
                    var tabs = (row[col].Length / TabSize);
                    stringBuilder.Append(row[col]);
                    if (col != maxLength.Length - 1)
                    {
                        stringBuilder.Append(new String('\t', maxLength[col] - tabs));
                    }
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder;
        }
    }
}
