using System;
using System.Web;

namespace Urmah
{
    public class UserAndRolePageFactory : IHttpHandlerFactory
    {
        private static IHttpHandler FindHandler(string name)
        {
            string controler = name;
            string args = string.Empty;
            
            int index = name.IndexOf("/");
            if (index > -1)
            {
                controler = name.Substring(0, index);
                args = name.Substring(index);
                if (args.Length > 1)
                {
                    args = args.Substring(1);
                }
                else
                {
                    args = string.Empty;
                }
            }
            
            switch (controler)
            {
                case "stylesheet":
                    return new ManifestResourceHandler("Urmah.Resources.UserAndRole.css", "text/css");

                case "cleardot":
                    return new ManifestResourceHandler("Urmah.Resources.cleardot.gif", "image/gif");

                case "genericicons":
                    return new ManifestResourceHandler("Urmah.Resources.GenericIcons.png", "image/png");
                
                case "users":
                    return UserPageFactory.GetHandler(args);                

                case "roles":
                    return RolePageFactory.GetHandler(args);

                case "about":
                    return new AboutPage();

                default:
                    return name.Length == 0 ? new UserAndRolePage() : null;
            }
        }
        
        #region IHttpHandlerFactory Members

        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            string resource = context.Request.PathInfo.Length == 0 ? string.Empty :
                context.Request.PathInfo.Substring(1).ToLowerInvariant();

            IHttpHandler handler = FindHandler(resource);

            if (handler == null)
            {
                context.Response.StatusCode = 404; // Page Not Found
            }

            return handler;
        }

        /// <summary>
        /// Enables the factory to reuse an existing handler instance.
        /// </summary
        public void ReleaseHandler(IHttpHandler handler)
        {            
        }

        #endregion

    }
}
