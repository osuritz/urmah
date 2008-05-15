using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Urmah
{
    public class PoweredBy : WebControl
    {
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            // Write out the assembly title, version number and copyright.

            About about = About.Instance;

            writer.Write("Powered by ");
            writer.AddAttribute("href", "http://urmah.googlecode.com/");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            HttpUtility.HtmlEncode(about.AssemblyProduct ?? "(product)", writer);
            writer.RenderEndTag();
            writer.Write(", version {0}", about.AssemblyVersion);

            writer.Write(". ");

            string copyright = about.AssemblyCopyright;
            if (copyright.Length > 0)
            {
                HttpUtility.HtmlEncode(copyright, writer);
                writer.Write(' ');
            }            
        }
    }
}
