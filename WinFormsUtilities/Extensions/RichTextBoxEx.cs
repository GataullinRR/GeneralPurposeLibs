using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Utilities.Extensions;

namespace WinFormsUtilities.Extensions
{
    public static class RichTextBoxEx
    {
        private const int EM_SETTABSTOPS = 0x00CB;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr h, int msg, int wParam, int[] lParam);

        public static void SetTabWidth(this RichTextBox rtb, int widthInSpaces)
        {
            Graphics graphics = rtb.CreateGraphics();
            var spaceWidth = (int)graphics.MeasureString(" ", rtb.Font).Width;
            SendMessage(rtb.Handle, EM_SETTABSTOPS, 1,
            new int[] { widthInSpaces * spaceWidth });
        }

        public static void AppendLine(this RichTextBox rtb, string format, params object[] objs)
        {
            rtb.AppendText(string.Format(format, objs) + Environment.NewLine);
        }

        public static void AppendLine(this RichTextBox rtb, string line)
        {
            rtb.AppendText(line + Environment.NewLine);
        }

        public static void Highlight(this RichTextBox rtb, string regexp, string wordToHighlight, Color highlightColor)
        {
            int oldSelectionStart = rtb.SelectionStart;
            int oldSelectionLength = rtb.SelectionLength;

            try
            {
                Regex rex = new Regex(regexp, RegexOptions.Multiline);
                MatchCollection mc = rex.Matches(rtb.Text);
                foreach (Match m in mc)
                {
                    int startIndex = m.Index + m.Value.Find(wordToHighlight).Index;
                    int stopIndex = wordToHighlight.Length;
                    rtb.Select(startIndex, stopIndex);
                    rtb.SelectionColor = highlightColor;
                }
            }
            catch { }

            rtb.SelectionStart = oldSelectionStart;
            rtb.SelectionLength = oldSelectionLength;
        }

        public static void Highlight(this RichTextBox rtb, string regexp, Color highlightColor)
        {
            int oldSelectionStart = rtb.SelectionStart;
            int oldSelectionLength = rtb.SelectionLength;

            try
            {
                Regex rex = new Regex(regexp, RegexOptions.Multiline);
                MatchCollection mc = rex.Matches(rtb.Text);
                foreach (Match m in mc)
                {
                    int startIndex = m.Index;
                    int stopIndex = m.Length;
                    rtb.Select(startIndex, stopIndex);
                    rtb.SelectionColor = highlightColor;
                }
            }
            catch { }

            rtb.SelectionStart = oldSelectionStart;
            rtb.SelectionLength = oldSelectionLength;
        }

        public static void ResetHighlighting(this RichTextBox rtb, Color defaultColor)
        {
            int oldSelectionStart = rtb.SelectionStart;
            int oldSelectionLength = rtb.SelectionLength;

            rtb.SelectionStart = 0;
            rtb.SelectionLength = rtb.Text.Length;
            rtb.SelectionColor = defaultColor;

            rtb.SelectionStart = oldSelectionStart;
            rtb.SelectionLength = oldSelectionLength;
        }

        public static List<Point> FindHighlightAreas(string text, string regexp, string wordToHighlight)
        {
            List<Point> areas = new List<Point>();
            try
            {
                Regex rex = new Regex(regexp, RegexOptions.Multiline);
                MatchCollection mc = rex.Matches(text);
                foreach (Match m in mc)
                {
                    int startIndex = m.Index + m.Value.Find(wordToHighlight).Index;
                    int stopIndex = wordToHighlight.Length + startIndex;
                    areas.Add(new Point(startIndex, stopIndex));
                }
            }
            catch { }
            return areas;
        }

        public static void HighlightAreas(this RichTextBox rtb, List<Point> areas, Color color)
        {
            int oldSelectionStart = rtb.SelectionStart;
            int oldSelectionLength = rtb.SelectionLength;

            for (int i = 0; i < areas.Count; i++)
            {
                Point area = areas[i];
                rtb.Select(area.X, area.Y - area.X);
                rtb.SelectionColor = color;
            }

            rtb.SelectionStart = oldSelectionStart;
            rtb.SelectionLength = oldSelectionLength;
        }
    }
}
