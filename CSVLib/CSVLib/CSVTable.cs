using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace CSVLib
{
    public class CSVTable : IEnumerable<CSVTuple>
    {
        List<CSVTuple> _Tuples = new List<CSVTuple>();
        public ReadOnlyCollection<CSVTuple> Rows
        { 
            get { return _Tuples.AsReadOnly(); }
        }

        public int Width => _Tuples.IsEmpty() 
            ? 0 
            : _Tuples.Max(t => t.Length);

        public CSVTable()
        {

        }
        public CSVTable(CSVTuple firstTuple)
        {
            AddRow(firstTuple);
        }

        public void AddRow(CSVTuple tuple)
        {
            ThrowUtils.ThrowIf_NullArgument(tuple);

            _Tuples.Add(tuple);
        }

        public CSVTable Add(CSVTuple value)
        {
            return Add(new CSVTuple[] { value });
        }
        public CSVTable Add(IEnumerable<CSVTuple> values)
        {
            _Tuples.AddRange(values);

            return this;
        }

        public void Paste(CSVTable anotherTable, int rowIndex, int columnIndex)
        {
            expandIfSmaller();
            for (int i = 0; i < anotherTable.Rows.Count; i++)
            {
                var anotherTablesRow = anotherTable.Rows[i];
                for (int k = 0; k < anotherTablesRow.Length; k++)
                {
                    Rows[i + rowIndex][k + columnIndex] = anotherTablesRow[k];
                }
            }

            void expandIfSmaller()
            {
                var dRow = (anotherTable.Rows.Count - Rows.Count).NegativeToZero();
                for (int i = 0; i < dRow; i++)
                {
                    AddRow(CSVTuple.Empty);
                }
                for (int i = 0; i < anotherTable.Rows.Count; i++)
                {
                    var thisTablesRowI = i + rowIndex;
                    var dColumn = (columnIndex + anotherTable.Rows[i].Length - Rows[thisTablesRowI].Length).NegativeToZero();
                    if (dColumn != 0)
                    {
                        Rows[thisTablesRowI].ResizeTo(columnIndex + anotherTable.Rows[i].Length);
                    }
                }
            }
        }

        #region ##### CSV Support #####

        public void SaveAsCSV(string root, string fileName, char cellSeparator = ';')
        {
            SaveAsCSV(Path.Combine(root, fileName), cellSeparator);
        }
        public void SaveAsCSV(string path, char cellSeparator = ';')
        {
            try
            {
                ThrowUtils.ThrowIf_NullArgument(path);
                IOUtils.CreateDirectoryIfNotExist(Path.GetDirectoryName(path));

                if (!File.Exists(path))
                {
                    using (File.Create(path)) { }
                }
                File.WriteAllText(path, AsCSV(cellSeparator));
            }
            catch (Exception e)
            {
                throw new TableException(TableException.ErrorCodes.UNABLE_TO_SAVE_AS_CSV, path, e);
            }
        }
        public void SaveAsCSV(Stream stream, char cellSeparator = ';')
        {
            var writer = new StreamWriter(stream);
            writer.Write(AsCSV(cellSeparator));
            writer.Flush();
        }

        public string AsCSV(char cellSeparator = ';')
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Rows.Count; i++)
            {
                sb.AppendLine(Rows[i].AsCSV(cellSeparator));
            }
            return sb.ToString();
        }

        public static CSVTable FromCSVFile(string csvFilePath, char cellSeparator = ';')
        {
            List<CSVTuple> tuples = tryReadLines(csvFilePath)
                .Select(l => CSVTuple.FromCSV(l, cellSeparator))
                .ToList();
            CSVTuple header = tuples.Count > 0 ? tuples[0] : CSVTuple.Empty;
            CSVTable table = new CSVTable(header);
            for (int i = 1; i < tuples.Count; i++)
            {
                table.AddRow(tuples[i]);
            }

            return table;

            //////////////////////////////////////

            List<string> tryReadLines(string filePath)
            {
                List<string> lines;
                try
                {
                    lines = File.ReadAllLines(filePath).ToList() ?? new List<string>();
                }
                catch (Exception e)
                {
                    throw new TableException(TableException.ErrorCodes.UNABLE_TO_LOAD, e);
                }

                return lines;
            }
        }

        #endregion

        public IEnumerator<CSVTuple> GetEnumerator()
        {
            return _Tuples.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Tuples.GetEnumerator();
        }
    }
}
