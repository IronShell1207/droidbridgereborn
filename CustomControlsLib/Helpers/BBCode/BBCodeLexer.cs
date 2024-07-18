namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// The BBCode lexer.
    /// </summary>
    internal sealed class BBCodeLexer : Lexer
    {
        #region Public Fields

        /// <summary>
        /// Normal state
        /// </summary>
        public const int STATE_NORMAL = 0;

        /// <summary>
        /// Tag state
        /// </summary>
        public const int STATE_TAG = 1;

        /// <summary>
        /// Attribute
        /// </summary>
        public const int TOKEN_ATTRIBUTE = 2;

        /// <summary>
        /// End tag
        /// </summary>
        public const int TOKEN_END_TAG = 1;

        /// <summary>
        /// Line break
        /// </summary>
        public const int TOKEN_LINE_BREAK = 4;

        /// <summary>
        /// Start tag
        /// </summary>
        public const int TOKEN_START_TAG = 0;

        /// <summary>
        /// Text
        /// </summary>
        public const int TOKEN_TEXT = 3;

        #endregion Public Fields

        #region Private Fields

        private static readonly char[] _newlineChars = new char[] { '\r', '\n' };
        private static readonly char[] _quoteChars = new char[] { '\'', '"' };
        private static readonly char[] _whitespaceChars = new char[] { ' ', '\t' };

        #endregion Private Fields

        #region Protected Properties

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected override int DefaultState => STATE_NORMAL;

        #endregion Protected Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeLexer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BBCodeLexer(string value) : base(value)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Gets the next token.
        /// </summary>
        public override Token NextToken()
        {
            if (LA(1) == char.MaxValue)
            {
                return Token.End;
            }

            if (State == STATE_NORMAL)
            {
                if (LA(1) == '[')
                {
                    if (LA(2) == '/')
                    {
                        return CloseTag();
                    }
                    else
                    {
                        Token token = OpenTag();
                        PushState(STATE_TAG);

                        return token;
                    }
                }
                else if (IsInRange(_newlineChars))
                {
                    return Newline();
                }
                else
                {
                    return Text();
                }
            }
            else if (State == STATE_TAG)
            {
                if (LA(1) == ']')
                {
                    Consume();
                    PopState();

                    return NextToken();
                }

                return Attribute();
            }
            else
            {
                throw new ParseException("Invalid state");
            }
        }

        #endregion Public Methods

        #region Private Methods

        private Token Attribute()
        {
            Match('=');
            while (IsInRange(_whitespaceChars))
            {
                Consume();
            }

            Token token;

            if (IsInRange(_quoteChars))
            {
                Consume();
                Mark();
                while (!IsInRange(_quoteChars))
                {
                    Consume();
                }

                token = new Token(GetMark(), TOKEN_ATTRIBUTE);
                Consume();
            }
            else
            {
                Mark();
                while (!IsInRange(_whitespaceChars) && LA(1) != ']' && LA(1) != char.MaxValue)
                {
                    Consume();
                }

                token = new Token(GetMark(), TOKEN_ATTRIBUTE);
            }

            while (IsInRange(_whitespaceChars))
            {
                Consume();
            }

            return token;
        }

        private Token CloseTag()
        {
            Match('[');
            Match('/');
            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            Token token = new Token(GetMark(), TOKEN_END_TAG);
            Match(']');

            return token;
        }

        private bool IsTagNameChar() => IsInRange('A', 'Z') || IsInRange('a', 'z') || IsInRange(new char[] { '*' });

        private Token Newline()
        {
            Match('\r', 0, 1);
            Match('\n');

            return new Token(string.Empty, TOKEN_LINE_BREAK);
        }

        private Token OpenTag()
        {
            Match('[');
            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            return new Token(GetMark(), TOKEN_START_TAG);
        }

        private Token Text()
        {
            Mark();
            while (LA(1) != '[' && LA(1) != char.MaxValue && !IsInRange(_newlineChars))
            {
                Consume();
            }

            return new Token(GetMark(), TOKEN_TEXT);
        }

        #endregion Private Methods
    }
}