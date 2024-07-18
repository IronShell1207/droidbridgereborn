using System.Globalization;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// Represents a single token.
    /// </summary>
    internal class Token
    {
        #region Public Fields

        /// <summary>
        /// Represents the token that marks the end of the input.
        /// </summary>
        public static readonly Token End = new Token(string.Empty, Lexer.TOKEN_END);

        #endregion Public Fields

        #region Public Properties

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type.</value>
        public int TokenType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="tokenType">Type of the token.</param>
        public Token(string value, int tokenType)
        {
            Value = value;
            TokenType = tokenType;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="object"/>.
        /// </returns>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0}: {1}", TokenType, Value);

        #endregion Public Methods
    }
}