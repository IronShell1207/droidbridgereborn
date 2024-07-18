using System;
using System.Collections.Generic;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// Provides basic lexer functionality.
    /// </summary>
    internal abstract class Lexer
    {
        #region Public Fields

        /// <summary>
        /// Defines the end-of-file token type.
        /// </summary>
        public const int TOKEN_END = int.MaxValue;

        #endregion Public Fields

        #region Private Fields

        private readonly CharBuffer _buffer;
        private readonly Stack<int> _states;

        #endregion Private Fields

        #region Protected Properties

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected abstract int DefaultState { get; }

        /// <summary>
        /// Gets the current state of the lexer.
        /// </summary>
        /// <value>The state.</value>
        protected int State => _states.Count > 0 ? _states.Peek() : DefaultState;

        #endregion Protected Properties

        #region Protected Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected Lexer(string value)
        {
            _buffer = new CharBuffer(value);
            _states = new Stack<int>();
        }

        #endregion Protected Constructors

        #region Public Methods

        /// <summary>
        /// Gets the next token.
        /// </summary>
        public abstract Token NextToken();

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        protected void Consume() => _buffer.Consume();

        /// <summary>
        /// Gets the mark.
        /// </summary>
        protected string GetMark() => _buffer.GetMark();

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <returns>
        /// <see langword="true"/> if the current character is in given range; otherwise, <see langword="false"/>.
        /// </returns>
        protected bool IsInRange(char first, char last)
        {
            char la = LA(1);
            return la >= first && la <= last;
        }

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///	<see langword="true"/> if the current character is in given range; otherwise, <see langword="false"/>.
        /// </returns>
        protected bool IsInRange(params char[] value)
        {
            if (value == null)
            {
                return false;
            }

            char la = LA(1);
            for (int i = 0; i < value.Length; i++)
            {
                if (la == value[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of characters to look ahead.</param>
        protected char LA(int count) => _buffer.LA(count);

        /// <summary>
        /// Marks the current position.
        /// </summary>
        protected void Mark() => _buffer.Mark();

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void Match(char value)
        {
            if (LA(1) == value)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void Match(char value, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (LA(1) == value)
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the specified string.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void Match(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (LA(1) == value[i])
                {
                    Consume();
                }
                else
                {
                    throw new ParseException("String mismatch");
                }
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="value">The value.</param>
        protected void MatchRange(char[] value)
        {
            if (IsInRange(value))
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char[] value, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (IsInRange(value))
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        protected void MatchRange(char first, char last)
        {
            if (IsInRange(first, last))
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char first, char last, int minOccurs, int maxOccurs)
        {
            int i = 0;
            while (IsInRange(first, last))
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Pops the state.
        /// </summary>
        protected int PopState() => _states.Pop();

        /// <summary>
        /// Pushes a new state on the stac.
        /// </summary>
        /// <param name="state">The state.</param>
        protected void PushState(int state) => _states.Push(state);

        #endregion Protected Methods

        #region Private Methods

        private static void ValidateOccurence(int count, int minOccurs, int maxOccurs)
        {
            if (count < minOccurs || count > maxOccurs)
            {
                throw new ParseException("Invalid number of characters");
            }
        }

        #endregion Private Methods
    }
}