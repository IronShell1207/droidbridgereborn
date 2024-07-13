using System;
using System.Globalization;

using Windows.UI;
using Windows.UI.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// Represents the BBCode parser.
    /// </summary>
    internal sealed class BBCodeParser : Parser<Span>
    {
        #region Private Fields

        private const string TAG_BOLD = "b";
        private const string TAG_COLOR = "color";
        private const string TAG_ITALIC = "i";
        private const string TAG_SIZE = "size";
        private const string TAG_STRIKETHROUGH = "s";
        private const string TAG_UNDERLINE = "u";

        private readonly FrameworkElement _source;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeParser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The framework source element this parser operates in.</param>
        public BBCodeParser(string value, FrameworkElement source) : base(new BBCodeLexer(value))
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            _source = source;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Parses the text and returns a Span containing the parsed result.
        /// </summary>
        public override Span Parse()
        {
            Span span = new();
            Parse(span);

            return span;
        }

        #endregion Public Methods

        #region Private Methods

        private void Parse(Span span)
        {
            ParseContext context = new(span);

            while (true)
            {
                Token token = LA(1);
                Consume();

                if (token.TokenType == BBCodeLexer.TOKEN_START_TAG)
                {
                    ParseTag(token.Value, true, context);
                }
                else if (token.TokenType == BBCodeLexer.TOKEN_END_TAG)
                {
                    ParseTag(token.Value, false, context);
                }
                else if (token.TokenType == BBCodeLexer.TOKEN_TEXT)
                {
                    Run run = context.CreateRun(token.Value);
                    span.Inlines.Add(run);
                }
                else if (token.TokenType == BBCodeLexer.TOKEN_LINE_BREAK)
                {
                    span.Inlines.Add(new LineBreak());
                }
                else if (token.TokenType == BBCodeLexer.TOKEN_ATTRIBUTE)
                {
                    throw new ParseException("Unexpected token");
                }
                else if (token.TokenType == BBCodeLexer.TOKEN_END)
                {
                    break;
                }
                else
                {
                    throw new ParseException("Unknown token type");
                }
            }
        }

        private void ParseTag(string tag, bool start, ParseContext context)
        {
            if (tag == TAG_BOLD)
            {
                context.FontWeight = null;
                if (start)
                {
                    context.FontWeight = FontWeights.Bold;
                }
            }
            else if (tag == TAG_COLOR)
            {
                if (start)
                {
                    Token token = LA(1);
                    if (token.TokenType == BBCodeLexer.TOKEN_ATTRIBUTE)
                    {
                        int rgb = Convert.ToInt32(token.Value.Substring(1), 16);
                        Color color = Color.FromArgb(255, (byte)((rgb >> 16) & 0xff), (byte)((rgb >> 8) & 0xff), (byte)((rgb >> 0) & 0xff));
                        context.Foreground = new SolidColorBrush(color);
                        Consume();
                    }
                }
                else
                {
                    context.Foreground = null;
                }
            }
            else if (tag == TAG_ITALIC)
            {
                context.FontStyle = start ? FontStyle.Italic : FontStyle.Normal;
            }
            else if (tag == TAG_SIZE)
            {
                if (start)
                {
                    Token token = LA(1);
                    if (token.TokenType == BBCodeLexer.TOKEN_ATTRIBUTE)
                    {
                        context.FontSize = Convert.ToDouble(token.Value, CultureInfo.InvariantCulture);
                        Consume();
                    }
                }
                else
                {
                    context.FontSize = null;
                }
            }
            else if (tag == TAG_STRIKETHROUGH)
            {
                context.TextDecorations = start ? TextDecorations.Strikethrough : TextDecorations.None;
            }
            else if (tag == TAG_UNDERLINE)
            {
                context.TextDecorations = start ? TextDecorations.Underline : TextDecorations.None;
            }
        }

        #endregion Private Methods

        #region Private Classes

        private class ParseContext
        {
            #region Public Properties

            public double? FontSize { get; set; }

            public FontStyle? FontStyle { get; set; }

            public FontWeight? FontWeight { get; set; }

            public Brush Foreground { get; set; }

            public Span Parent { get; }

            public TextDecorations TextDecorations { get; set; }

            #endregion Public Properties

            #region Public Constructors

            public ParseContext(Span parent) => Parent = parent;

            #endregion Public Constructors

            #region Public Methods

            /// <summary>
            /// Creates a run reflecting the current context settings.
            /// </summary>
            public Run CreateRun(string text)
            {
                Run run = new()
                {
                    Text = text,
                    TextDecorations = TextDecorations,
                };

                if (FontSize.HasValue)
                {
                    run.FontSize = FontSize.Value;
                }
                if (FontWeight.HasValue)
                {
                    run.FontWeight = FontWeight.Value;
                }
                if (FontStyle.HasValue)
                {
                    run.FontStyle = FontStyle.Value;
                }
                if (Foreground is not null)
                {
                    run.Foreground = Foreground;
                }

                return run;
            }

            #endregion Public Methods
        }

        #endregion Private Classes
    }
}