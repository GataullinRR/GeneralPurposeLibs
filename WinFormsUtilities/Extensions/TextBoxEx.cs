using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace WinFormsUtilities.Extensions
{
    public static class TextBoxEx
    {
        public static void AppendLine(this TextBox tb, string format, params object[] objs)
        {
            tb.AppendText(string.Format(format, objs) + Environment.NewLine);
        }

        public static void AppendLine(this TextBox tb, string line)
        {
            tb.AppendText(line + Environment.NewLine);
        }
    }
}
