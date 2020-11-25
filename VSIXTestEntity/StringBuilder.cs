using System;
using System.Text;

namespace VSIXTestEntity
{
    /// <summary>
    /// This example is from EfCore
    /// </summary>
    public class IndentedStringBuilder
    {
        private const byte IndentSize = 4;
        private byte _indent;
        private bool _indentPending = true;

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        ///     The current length of the built string.
        /// </summary>
        public virtual int Length
            => _stringBuilder.Length;

        /// <summary>
        ///     Appends the current indent and then the given string to the string being built.
        /// </summary>
        /// <param name="value"> The string to append. </param>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder Append(string value)
        {
            DoIndent();

            _stringBuilder.Append(value);

            return this;
        }

        /// <summary>
        ///     Appends a new line to the string being built.
        /// </summary>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder AppendLine()
        {
            AppendLine(string.Empty);

            return this;
        }

        /// <summary>
        ///     <para>
        ///         Appends the current indent, the given string, and a new line to the string being built.
        ///     </para>
        ///     <para>
        ///         If the given string itself contains a new line, the part of the string after that new line will not be indented.
        ///     </para>
        /// </summary>
        /// <param name="value"> The string to append. </param>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder AppendLine(string value)
        {
            if (value.Length != 0)
            {
                DoIndent();
            }

            _stringBuilder.AppendLine(value);

            _indentPending = true;

            return this;
        }

        /// <summary>
        ///     Resets this builder ready to build a new string.
        /// </summary>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder Clear()
        {
            _stringBuilder.Clear();
            _indent = 0;

            return this;
        }

        /// <summary>
        ///     Increments the indent.
        /// </summary>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder IncrementIndent()
        {
            _indent++;

            return this;
        }

        /// <summary>
        ///     Decrements the indent.
        /// </summary>
        /// <returns> This builder so that additional calls can be chained. </returns>
        public virtual IndentedStringBuilder DecrementIndent()
        {
            if (_indent > 0)
            {
                _indent--;
            }

            return this;
        }

        /// <summary>
        ///     Creates a scoped indenter that will increment the indent, then decrement it when disposed.
        /// </summary>
        /// <returns> An indenter. </returns>
        public virtual IDisposable Indent()
            => new Indenter(this);

        /// <summary>
        ///     Returns the built string.
        /// </summary>
        /// <returns> The built string. </returns>
        public override string ToString()
            => _stringBuilder.ToString();

        private void DoIndent()
        {
            if (_indentPending && _indent > 0)
            {
                _stringBuilder.Append(new string(' ', _indent * IndentSize));
            }

            _indentPending = false;
        }

        private sealed class Indenter : IDisposable
        {
            private readonly IndentedStringBuilder _stringBuilder;

            public Indenter(IndentedStringBuilder stringBuilder)
            {
                _stringBuilder = stringBuilder;

                _stringBuilder.IncrementIndent();
            }

            public void Dispose()
                => _stringBuilder.DecrementIndent();
        }
    }
}
