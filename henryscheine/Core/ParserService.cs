using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class ParserService<T>: IParserService<T>
    {
        public IEnumerable<IEnumerable<T>> ParseText(string inputText)
        {
            var result = new List<List<T>>();

            if (inputText == null)
            {
                throw new ArgumentException("Input text can't be null");
            }

            var inputTextArray = inputText.Split(Environment.NewLine);

            if (inputText.Trim() == "")
            {
                throw new ArgumentException("Input text can't be empty");
            }

            var columnsCount = 0;

            foreach (var line in inputTextArray)
            {
                var cells = line.Substring(1, line.Length - 2).Split("\",\"");
                var rowItems = new List<T>();
                foreach(var cell in cells)
                {
                    var filteredCellText = Array.FindAll<char>(cell.ToCharArray(), (c => (!char.IsControl(c))));
                    rowItems.Add(GetValue<T>(new string(filteredCellText)));
                }

                if (columnsCount == 0)
                    columnsCount = rowItems.Count;
                else
                    FillOutEmptyColumns(columnsCount, ref rowItems);

                result.Add(rowItems);
            }
            return result;
        }

        private static T GetValue<T>(String value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        private void FillOutEmptyColumns(int columnsCount, ref List<T> rowItems)
        {
            if(rowItems.Count > columnsCount)
            {
                rowItems = rowItems.Take(columnsCount).ToList();
            } else
            {
                while (rowItems.Count < columnsCount)
                {
                    rowItems.Add(GetValue<T>(""));
                }
            }
        }
    }
}
