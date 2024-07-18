using System;

namespace DrStrange.Controls.BBCode
{
    /// <summary>
    /// The exception for parse errors.
    /// </summary>
    public class ParseException : Exception
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        public ParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion Public Constructors
    }
}