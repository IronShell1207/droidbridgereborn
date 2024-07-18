using DrStrange.Controls.BBCode;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace DrStrange.Controls
{
    public static class TextBlockHelper
    {
        #region Public Fields

        public static readonly DependencyProperty BBCodeProperty = DependencyProperty.RegisterAttached(
            "BBCode",
            typeof(bool),
            typeof(TextBlock),
            new PropertyMetadata(null, new PropertyChangedCallback(OnBBCodeChanged)));

        #endregion Public Fields

        #region Public Methods

        public static bool GetBBCode(DependencyObject textBlock) => (bool)textBlock.GetValue(BBCodeProperty);

        public static void SetBBCode(DependencyObject textBlock, bool value) => textBlock.SetValue(BBCodeProperty, value);

        #endregion Public Methods

        #region Private Methods

        private static void OnBBCodeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;

            textBlock.RegisterPropertyChangedCallback(TextBlock.TextProperty, (s, e) =>
            {
                string bbcode = textBlock.Text;
                textBlock.Inlines.Clear();

                if (!string.IsNullOrWhiteSpace(bbcode))
                {
                    Inline inline;
                    try
                    {
                        BBCodeParser parser = new BBCodeParser(bbcode, textBlock);
                        inline = parser.Parse();
                    }
                    catch
                    {
                        inline = new Run { Text = bbcode };
                    }
                    textBlock.Inlines.Add(inline);
                }
            });
        }

        #endregion Private Methods
    }
}