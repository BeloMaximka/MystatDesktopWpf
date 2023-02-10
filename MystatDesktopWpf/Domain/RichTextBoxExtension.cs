using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;

namespace MystatDesktopWpf.Domain
{
    public static class RichTextBoxExtension
    {
        public static void LoadFromFile(this RichTextBox richTextBox, string fileName)
        {
            TextRange range = new(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using FileStream stream = new(fileName, FileMode.OpenOrCreate);
            range.Load(stream, DataFormats.Rtf);
        }
    }
}