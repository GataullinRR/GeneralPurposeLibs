using INIUtils.Marshallers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;

namespace INIUtils
{
    public class INIBuilderException : Exception
    {

    }

    public class INIWriter
    {
        const string NULL = "NULL";
        const string KVP_PATTERN = "{0} = {1}";

        StringBuilder _stringBuilder = new StringBuilder();
        StringMarshaller _invalidCharsReplacer = new StringMarshaller();

        public void AppendKVP(string key, bool value)
            => AppendKVP(key, value, new BooleanMarshaller());
        public void AppendKVP(string key, int value)
            => AppendKVP(key, value, new Int32Marshaller());
        public void AppendKVP(string key, float value)
            => AppendKVP(key, value, new SingleMarshaller());
        public void AppendKVP(string key, double value)
            => AppendKVP(key, value, new DoubleMarshaller());
        public void AppendKVP(string key, string value)
            => AppendKVP(key, value, new StringMarshaller());
        public void AppendKVP<T>(string key, T value, TypeMarshaller<T> marshaller)
        {
            checkArgs();

            _stringBuilder.AppendFormatLine(KVP_PATTERN, key, pack());

            return;

            void checkArgs()
            {
                ThrowUtils.ThrowIf_NullArgument(key, marshaller);
                throwIfKeyInvalid();

                return;

                void throwIfKeyInvalid()
                {
                    bool haveInvalidChars = key.Any(c => !char.IsLetterOrDigit(c) && c != '_');
                    if (haveInvalidChars)
                    {
                        throw new INIBuilderException();
                    }
                }
            }

            string pack()
            {
                var success = marshaller.TryPack(value, out string packed);
                if (!success)
                {
                    throw new INIBuilderException();
                }
                else if (_invalidCharsReplacer.GetType() != marshaller.GetType())
                {
                    success = _invalidCharsReplacer.TryPack(packed, out packed);
                    if (!success)
                    {
                        throw new INIBuilderException();
                    }
                }

                return packed;
            }
        }

        public bool Save(Stream ini)
        {
            var sw = new StreamWriter(ini);
            int cnt = Environment.NewLine.Length;
            var tmp = _stringBuilder.ToString();
            sw.Write(tmp.Substring(0, tmp.Length - cnt));
            sw.Flush();

            return true;
        }
        public bool Save(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                string dir = Path.GetDirectoryName(path);
                Directory.CreateDirectory(dir);

                using (StreamWriter sw = File.CreateText(path))
                {
                    int cnt = Environment.NewLine.Length;
                    var tmp = _stringBuilder.ToString();
                    sw.Write(tmp.Substring(0, tmp.Length - cnt));
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
