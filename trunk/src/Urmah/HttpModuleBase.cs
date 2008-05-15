using System;
using System.Web;

namespace Urmah
{
    public class HttpModuleBase : IHttpModule
    {
        #region IHttpModule Members

        public void Init(HttpApplication context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            
            OnInit(context);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Initializes the module and prepares it to handle requests.
        /// </summary>
        protected virtual void OnInit(HttpApplication application) { }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module.
        /// </summary>
        protected virtual void OnDispose() { }
    }
}
