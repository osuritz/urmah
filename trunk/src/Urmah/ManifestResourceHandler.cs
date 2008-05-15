using System;
using System.Web;

using Stream = System.IO.Stream;
using Encoding = System.Text.Encoding;
using Assembly = System.Reflection.Assembly;

namespace Urmah
{
    /// <summary>
    /// Reads a resource from the assembly manifest and returns its contents
    /// as the response entity.
    /// </summary>
    public class ManifestResourceHandler : IHttpHandler
    {
        private readonly Assembly _assembly;
        private readonly string _resourceName;
        private readonly string _contentType;
        private readonly Encoding _responseEncoding;

        #region Constructors
        
        public ManifestResourceHandler(Assembly assembly, string resourceName, string contentType, Encoding responseEncoding)
        {
            if (resourceName == null)
            {
                throw new ArgumentNullException("resourceName");
            }
            if (contentType == null)
            {
                throw new ArgumentNullException("contentType");
            }

            _assembly = assembly ?? Assembly.GetExecutingAssembly();

            _resourceName = resourceName;
            _contentType = contentType;
            _responseEncoding = responseEncoding;
        }

        public ManifestResourceHandler(string resourceName, string contentType, Encoding responseEncoding) :
            this(Assembly.GetExecutingAssembly(), resourceName, contentType, responseEncoding) { }

        public ManifestResourceHandler(Assembly assembly, string resourceName, string contentType) :
            this(assembly, resourceName, contentType, null) { }

        public ManifestResourceHandler(string resourceName, string contentType) :
            this(Assembly.GetExecutingAssembly(), resourceName, contentType, null) { }

        public ManifestResourceHandler(string resourceName, Encoding responseEncoding) :
            this(Assembly.GetExecutingAssembly(), resourceName, "text/plain", responseEncoding) { }

        public ManifestResourceHandler(string resourceName) :
            this(Assembly.GetExecutingAssembly(), resourceName, "text/plain", null) { }
        
        #endregion

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            using (Stream stream = _assembly.GetManifestResourceStream(_resourceName))
            {
                // Allocate a buffer for reading the stream. The maximum size of this buffer is fixed to 4 KB.
                byte[] buffer = new byte[Math.Min(stream.Length, 4096)];

                // Set the response headers for indicating the content type and encoding (if specified).
                HttpResponse response = context.Response;
                response.ContentType = _contentType;

                if (_responseEncoding != null)
                    response.ContentEncoding = _responseEncoding;

                // Finally, write out the bytes!
                int readLength = stream.Read(buffer, 0, buffer.Length);

                while (readLength > 0)
                {
                    response.OutputStream.Write(buffer, 0, readLength);
                    readLength = stream.Read(buffer, 0, buffer.Length);
                }
            }
        }

        #endregion
    }
}
