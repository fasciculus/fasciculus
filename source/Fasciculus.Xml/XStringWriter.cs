using System.IO;
using System.Text;

namespace Fasciculus.Xml
{
    /// <summary>
    /// A <see cref="StringWriter"/> to use with <see cref="System.Xml.Linq.XDocument"/>.
    /// </summary>
    public class XStringWriter : StringWriter
    {
        private readonly Encoding encoding;

        /// <summary>
        /// Gets the <see cref="Encoding"/> in which the output is written. This controls the
        /// xml specification at the top of the document.
        /// </summary>
        public override Encoding Encoding => encoding;

        /// <summary>
        /// Initializes a string writer with the specified <paramref name="encoding"/>.
        /// </summary>
        public XStringWriter(Encoding encoding)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// Initializes a string writer with <see cref="Encoding.UTF8"/>.
        /// </summary>
        public XStringWriter()
            : this(Encoding.UTF8) { }

        /// <summary>
        /// Initializes a string writer that writes to the given <see cref="StringBuilder"/> with the specified <paramref name="encoding"/>.
        /// </summary>
        public XStringWriter(StringBuilder sb, Encoding encoding)
            : base(sb)
        {
            this.encoding = encoding;
        }

        /// <summary>
        /// Initializes a string writer that writes to the given <see cref="StringBuilder"/> with <see cref="Encoding.UTF8"/>.
        /// </summary>
        public XStringWriter(StringBuilder sb)
            : this(sb, Encoding.UTF8) { }
    }
}
