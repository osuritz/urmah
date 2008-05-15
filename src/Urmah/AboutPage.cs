using System.Diagnostics;
using System.Reflection;
using System.Web.UI;
using TextResource = Urmah.Resources.Resource;

namespace Urmah
{
    internal class AboutPage : PageBase
    {        
        public AboutPage()
        {
            PageTitle = string.Format(TextResource.AboutTitleFormatString, About.Instance.AssemblyTitle);
        }

        protected override void RenderContents(System.Web.UI.HtmlTextWriter writer)
        {
            // Title
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "PageTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.H1);
            writer.Write(PageTitle);
            writer.RenderEndTag(); // </h1>
            writer.WriteLine();

            // Speed Bar
            SpeedBar.Render(writer,
                SpeedBar.Home.Format(BasePageName),
                SpeedBar.Help,
                SpeedBar.About.Format(BasePageName));

            About about = About.Instance;
            
            writer.RenderBeginTag(HtmlTextWriterTag.P);

            writer.RenderBeginTag(HtmlTextWriterTag.H2);
            writer.Write(about.AssemblyTitle);
            writer.RenderEndTag(); // </h2>

            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("version {0}", about.AssemblyVersion);
            writer.RenderEndTag(); // </div>

            writer.RenderEndTag(); // </p>

            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(Server.HtmlEncode(about.AssemblyCopyright));
            writer.RenderEndTag(); // </p>

            base.RenderContents(writer);
        }
    }
}
