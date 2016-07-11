using System.Drawing;
using System.Windows.Forms;

namespace SamirBoulema.TGIT
{
    public static class RichTextBoxExtension
    {
        public static void AppendText(this RichTextBox box, string text, Color color, bool bold)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.SelectionFont = new Font(box.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }

        public static void AppendText(this RichTextBox box, string text, bool bold)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = box.ForeColor;
            box.SelectionFont = new Font(box.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
