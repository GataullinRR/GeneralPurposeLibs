using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace INIUtils.Marshallers
{
    class StringMarshaller : TypeMarshaller<string>
    {
        const char WRAP_SYMBOL = '|';
        const string NULL = "NULL";

        public override bool TryPack(string value, out string result)
        {
            if (value == null)
            {
                result = NULL;
                return true;
            }

            StringBuilder packed = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (isAllowedChar(c))
                {
                    packed.Append(c);
                }
                else
                {
                    packed.AppendFormat("{0}{1}{0}", WRAP_SYMBOL, c.ToInt32());
                }
            }

            result = packed.ToString();
            return true;
        }

        static bool isAllowedChar(char c)
        {
            return (char.IsDigit(c) || char.IsLetter(c) || char.IsPunctuation(c) || c == ' ')
                && c != WRAP_SYMBOL;
        }

        public override bool TryUnpack(string packed, out string result)
        {
            if (packed == NULL || packed == null)
            {
                result = null;
                return true;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < packed.Length; i++)
            {
                char c = packed[i];
                if (c != WRAP_SYMBOL)
                {
                    sb.Append(c);
                }
                else
                {
                    int from = i + 1;
                    int to = -1;
                    for (int j = from; j < packed.Length; j++)
                    {
                        c = packed[j];
                        if (c == WRAP_SYMBOL)
                        {
                            to = j;
                            break;
                        }
                    }
                    bool ok = false;
                    int len = to - from;
                    int charValue = -1;
                    if (len > 0)
                    {
                        string dissalowedCharAsNumber = packed.Substring(from, len);
                        ok = int.TryParse(dissalowedCharAsNumber, out charValue);
                    }

                    if (ok)
                    {
                        sb.Append((char)charValue);
                        i += len + 1;
                    }
                    else
                    {
                        result = null;
                        return false;
                    }
                }
            }

            result = sb.ToString();
            return true;
        }
    }
}
