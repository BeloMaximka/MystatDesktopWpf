using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MystatDesktopWpf.Extensions
{
    public static class InlineCollectionExtension
    {
        // Detects hyperlinks in text (https://... or http://...) and sets a new collection of Runs and Hyperlinks
        public static void SetInlinesWithHyperlinksFromText(this InlineCollection inlines, string text, RequestNavigateEventHandler? linkCallback = null)
        {
            var regex = new Regex(@"(https?:\/\/[^\s]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = regex.Matches(text).Cast<Match>().Select(m => m.Value).ToList();

            inlines.Clear();
            foreach (var segment in regex.Split(text))
            {
                if (matches.Contains(segment))
                {
                    var hyperlink = new Hyperlink(new Run(segment))
                    {
                        NavigateUri = new Uri(segment),
                    };
                    if (linkCallback == null)
                    {
                        hyperlink.RequestNavigate += (sender, args) => Process.Start(new ProcessStartInfo
                        {
                            FileName = segment,
                            UseShellExecute = true
                        });
                    }

                    inlines.Add(hyperlink);
                }
                else
                {
                    inlines.Add(new Run(segment));
                }
            }
        }
    }
}