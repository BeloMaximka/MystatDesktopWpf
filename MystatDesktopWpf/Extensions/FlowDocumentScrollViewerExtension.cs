using System.Windows.Controls;
using System.Windows.Media;

namespace MystatDesktopWpf.Extensions
{
    public static class FlowDocumentScrollViewerExtension
    {
        // Adjusts width according to its text because this control can't do it itself
        public static void AdjustWidthToText(this FlowDocumentScrollViewer documentViever)
        {
            documentViever.Width = documentViever.Document.GetFormattedText(VisualTreeHelper.GetDpi(documentViever).PixelsPerDip).
                                    WidthIncludingTrailingWhitespace + 20;
            if (documentViever.Width > documentViever.MaxWidth) documentViever.Width = documentViever.MaxWidth;
        }
    }
}
