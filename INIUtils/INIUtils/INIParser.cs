using INIUtils.Marshallers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;

namespace INIUtils
{
    public enum ParserErrors
    {
        KVP_PATTERT_INVALID,
        MULTIPLE_KEYS,
        CANNOT_READ_FILE,
        UNSUCCESSFUL_UNPACKING
    }

    public class INIParserException : Exception
    {
        public ParserErrors Error { get; private set; }
        public object ErrorSource { get; private set; }

        public INIParserException(ParserErrors error)
            : this(error, null) { }

        public INIParserException(ParserErrors error, object errorSource)
        {
            Error = error;
            ErrorSource = errorSource;
        }

        public void RethrowIfNot(params ParserErrors[] errors)
        {
            if (!errors.Contains(Error))
            {
                throw this;
            }
        }
    }

    public class INIReader
    {
        const string KVP_SEPARATOR = " = ";

        Dictionary<string, string> _parameters = new Dictionary<string, string>();
        StringMarshaller _invalidCharsReplacer = new StringMarshaller();

        public INIReader()
        {
            fromLines(new string[0]);
        }
        public INIReader(Stream ini)
        {
            var reader = new StreamReader(ini);
            var line = reader.ReadLine();
            var lines = new List<string>();
            while (line != null)
            {
                lines.Add(line);
                line = reader.ReadLine();
            }

            fromLines(lines.ToArray());
        }
        public INIReader(string path)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(path);
            }
            catch
            {
                throw new INIParserException(ParserErrors.CANNOT_READ_FILE);
            }

            fromLines(lines);
        }
        public INIReader(string[] lines)
        {
            fromLines(lines);
        }
        void fromLines(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] kvp = line.Split(new[] { KVP_SEPARATOR }, StringSplitOptions.None);
                if (kvp.Length < 2) // Строки могут содержать паттерн " = "
                {
                    throw new INIParserException(ParserErrors.KVP_PATTERT_INVALID, line);
                }
                string key = kvp[0];
                string value = line.Substring(key.Length + KVP_SEPARATOR.Length);
                if (_parameters.ContainsKey(key))
                {
                    throw new INIParserException(ParserErrors.MULTIPLE_KEYS, key);
                }
                _parameters.Add(key, value);
            }
        }

        public bool TryGetBoolean(string key, out bool result)
            => TryGetParam(key, new BooleanMarshaller(), out result);
        public bool TryGetInt32(string key, out int result)
            => TryGetParam(key, new Int32Marshaller(), out result);
        public bool TryGetSingle(string key, out float result)
            => TryGetParam(key, new SingleMarshaller(), out result);
        public bool TryGetDouble(string key, out double result)
            => TryGetParam(key, new DoubleMarshaller(), out result);
        public bool TryGetString(string key, out string result)
            => TryGetParam(key, new StringMarshaller(), out result);
        public bool TryGetParam<T>(string key, TypeMarshaller<T> marshaller, out T result)
        {
            if (marshaller == null || key == null)
            {
                string errArgs =
                    marshaller == null ? nameof(marshaller) + "," : "" +
                    key == null ? nameof(key) : "";
                throw new ArgumentNullException(errArgs);
            }

            result = default(T);
            var paramExistsAndValid = IsParamExist(key);
            if (paramExistsAndValid)
            {
                paramExistsAndValid &= CommonUtils.TryOrDefault(() => GetParam(key, marshaller), out result);
            }
            return paramExistsAndValid;
        }

        public bool GetBoolean(string key)
            => GetParam(key, new BooleanMarshaller());
        public int GetInt32(string key)
            => GetParam(key, new Int32Marshaller());
        public float GetSingle(string key)
            => GetParam(key, new SingleMarshaller());
        public double GetDouble(string key)
            => GetParam(key, new DoubleMarshaller());
        public string GetString(string key)
            => GetParam(key, new StringMarshaller());
        public T GetParam<T>(string key, TypeMarshaller<T> marshaller)
        {
            if (marshaller == null ||  key == null)
            {
                string errArgs = 
                    marshaller == null ? nameof(marshaller) + "," : "" + 
                    key == null ? nameof(key) : "";
                throw new ArgumentNullException(errArgs);
            }

            var success = true;
            string raw = _parameters[key];
            if (_invalidCharsReplacer.GetType() != marshaller.GetType())
            {
                success = _invalidCharsReplacer.TryUnpack(_parameters[key], out raw);
            }
            success &= marshaller.TryUnpack(raw, out T result);
            return success ? result : throw new INIParserException(ParserErrors.UNSUCCESSFUL_UNPACKING, key);
        }

        public bool IsParamExist(string key)
        {
            return key == null ? false : _parameters.ContainsKey(key);
        }
    }
}
