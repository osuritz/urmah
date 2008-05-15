using System;

namespace Urmah
{
    public sealed class HtmlLinkType
    {
        /// <summary>
        /// Refers to the first document in a collection of documents. This
        /// link  type tells search engines which document is considered by
        /// the author to be the starting point of the collection.
        /// </summary>
        public const string Start = "start";

        /// <summary>
        /// Refers  to the next document in a linear sequence of documents.
        /// User  agents  may  choose  to  preload  the "next" document, to
        /// reduce the perceived load time.
        /// </summary>
        public const string Next = "next";

    }
}
