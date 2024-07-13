using System;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// Represents a character buffer.
    /// </summary>
    internal class CharBuffer
    {
        #region Private Fields

        private int _mark;
        private int _position;
        private readonly string _value;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CharBuffer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CharBuffer(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _value = value;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        public void Consume() => _position++;

        /// <summary>
        /// Gets the mark.
        /// </summary>
        public string GetMark()
        {
            if (_mark < _position)
            {
                return _value.Substring(_mark, _position - _mark);
            }

            return string.Empty;
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of character to look ahead.</param>
        public char LA(int count)
        {
            int index = _position + count - 1;
            if (index < _value.Length)
            {
                return _value[index];
            }

            return char.MaxValue;
        }

        /// <summary>
        /// Marks the current position.
        /// </summary>
        public void Mark() => _mark = _position;

        #endregion Public Methods
    }
}