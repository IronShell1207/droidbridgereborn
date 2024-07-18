using System;
using System.Collections.Generic;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// Represents a token buffer.
    /// </summary>
    internal class TokenBuffer
    {
        #region Private Fields

        private int _position;
        private readonly List<Token> _tokens = new();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBuffer"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        public TokenBuffer(Lexer lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            Token token;
            do
            {
                token = lexer.NextToken();
                _tokens.Add(token);
            }
            while (token.TokenType != Lexer.TOKEN_END);
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        public void Consume() => _position++;

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of tokens to look ahead.</param>
        public Token LA(int count)
        {
            int index = _position + count - 1;
            if (index < _tokens.Count)
            {
                return _tokens[index];
            }

            return Token.End;
        }

        #endregion Public Methods
    }
}