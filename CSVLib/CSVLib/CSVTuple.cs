using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Utilities;

namespace CSVLib
{
    public class CSVTuple : IEnumerable<string>
    {
        public static CSVTuple Empty => new CSVTuple(null);

        public string this[int index]
        {
            get => _Elements[index];
            set => _Elements[index] = value;
        }

        public int Length => _Elements.Count;
        List<string> _Elements;
        public ReadOnlyCollection<string> Elements { get => _Elements.AsReadOnly(); }

        public CSVTuple(params object[] elements)
        {
            elements = elements ?? new object[] { };

            _Elements = elements
                .Select(e => e?.ToString() ?? "NULL")
                .ToList();
        }

        public CSVTuple Add(string value)
        {
            return Add(new string[] { value });
        }
        public CSVTuple Add<T>(IEnumerable<T> value)
        {
            _Elements.AddRange(value.Select(v => v?.ToString() ?? "NULL").ToArray());

            return this;
        }

        public void ResizeTo(int newLength)
        {
            var dE = newLength - Elements.Count;
            var elements = dE >= 0
                ? ArrayUtils.ConcatSequences(Elements, new string[dE])
                : Elements.Take(newLength);
            _Elements = elements.ToList();
        }

        public static CSVTuple FromCSV(string rowInCSVFormat, char cellSeparator = ';')
        {
            rowInCSVFormat = rowInCSVFormat ?? "";

            List<string> cells = rowInCSVFormat.Split(cellSeparator).ToList();
            return new CSVTuple(cells.ToArray());
        }

        public string AsCSV(char cellSeparator = ';')
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Length; i++)
            {
                sb.Append(Elements[i]);
                if (i != Length - 1)
                {
                    sb.Append(cellSeparator);
                }
            }
            return sb.ToString();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _Elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
