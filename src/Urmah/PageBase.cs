using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CultureInfo = System.Globalization.CultureInfo;
using StringBuilder = System.Text.StringBuilder;

namespace Urmah
{
    internal abstract class PageBase : Page
    {
        private string _title;
        protected virtual string PageTitle
        {
            get { return _title ?? string.Empty; }
            set { _title = value; }
        }
        
        protected string BasePageName
        {
            get { return this.Request.ServerVariables["URL"]; }
        }

        protected string ApplicationName
        {
            get
            {
                return InferApplicationName(Context);
            }
        }
        
        protected virtual void RenderDocumentStart(HtmlTextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");

            writer.AddAttribute("xmlns", "http://www.w3.org/1999/xhtml");
            writer.RenderBeginTag(HtmlTextWriterTag.Html);  // <html>

            writer.RenderBeginTag(HtmlTextWriterTag.Head);  // <head>
            RenderHead(writer);
            writer.RenderEndTag();                          // </head>
            writer.WriteLine();

            writer.RenderBeginTag(HtmlTextWriterTag.Body);  // <body>
        }

        protected virtual void RenderHead(HtmlTextWriter writer)
        {
            //
            // Write the document title.
            //

            writer.RenderBeginTag(HtmlTextWriterTag.Title);
            Server.HtmlEncode(this.PageTitle, writer);
            writer.RenderEndTag();
            writer.WriteLine();

            //
            // Write a <link> tag to relate the style sheet.
            //

            writer.AddAttribute("rel", "stylesheet");
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text/css");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, this.BasePageName + "/stylesheet");
            writer.RenderBeginTag(HtmlTextWriterTag.Link);
            writer.RenderEndTag();
            writer.WriteLine();
        }
        
        protected virtual void RenderContents(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        protected virtual void RenderDocumentEnd(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "Footer");
            writer.RenderBeginTag(HtmlTextWriterTag.P); // <p>

            // Write the powered-by signature, that includes version information.
            PoweredBy poweredBy = new PoweredBy();
            poweredBy.RenderControl(writer);

            // Write out server date, time and time zone details.

            DateTime now = DateTime.Now;

            writer.Write("Server date is ");
            this.Server.HtmlEncode(now.ToString("D", CultureInfo.InvariantCulture), writer);

            writer.Write(". Server time is ");
            this.Server.HtmlEncode(now.ToString("T", CultureInfo.InvariantCulture), writer);

            writer.Write(". All dates and times displayed are in the ");
            writer.Write(TimeZone.CurrentTimeZone.IsDaylightSavingTime(now) ?
                TimeZone.CurrentTimeZone.DaylightName : TimeZone.CurrentTimeZone.StandardName);
            writer.Write(" zone. ");


            writer.RenderEndTag(); // </p>

            writer.RenderEndTag(); // </body>
            writer.WriteLine();

            writer.RenderEndTag(); // </html>
            writer.WriteLine();
        }
        
        protected override void Render(HtmlTextWriter writer)
        {
            RenderDocumentStart(writer);
            RenderContents(writer);
            RenderDocumentEnd(writer);
        }


        #region Utility Methods
        protected TableCell FormatCell(TableCell cell, string contents, string cssClassName)
        {
            return FormatCell(cell, contents, cssClassName, string.Empty);
        }

        protected TableCell FormatCell(TableCell cell, string contents, string cssClassName, string toolTip)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell");
            }

            cell.Wrap = false;
            cell.CssClass = cssClassName;

            if (contents.Length == 0)
            {
                cell.Text = "&nbsp;";
            }
            else
            {
                string encodedContents = this.Server.HtmlEncode(contents);

                if (toolTip.Length == 0)
                {
                    cell.Text = encodedContents;
                }
                else
                {
                    Label label = new Label();
                    label.ToolTip = toolTip;
                    label.Text = encodedContents;
                    cell.Controls.Add(label);
                }
            }

            return cell;
        }

        protected string GetUrl(string path)
        {
            return string.Format("{0}{1}", BasePageName, path);
        }

        protected static string GetTimeAndDateString(DateTime dateTime)
        {
            return string.Format("{0} @ {1}", dateTime.ToShortDateString(), dateTime.ToShortTimeString());
        }

        protected static string CapitalizeFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            
            StringBuilder sb = new StringBuilder(s);

            if (char.IsLower(sb[0]))
            {
                sb[0] = char.ToUpperInvariant(sb[0]);
            }

            return sb.ToString();
        }

        private static string InferApplicationName(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

#if NET_1_1 || NET_1_0
            return HttpRuntime.AppDomainAppId;
#else
            //
            // Setup the application name (ASP.NET 2.0 or later).
            //

            string appName = null;

            if (context.Request != null)
            {
                //
                // ASP.NET 2.0 returns a different and more cryptic value
                // for HttpRuntime.AppDomainAppId comared to previous 
                // versions. Also HttpRuntime.AppDomainAppId is not available
                // in partial trust environments. However, the APPL_MD_PATH
                // server variable yields the same value as 
                // HttpRuntime.AppDomainAppId did previously so we try to
                // get to it over here for compatibility reasons (otherwise
                // folks upgrading to this version of ELMAH could find their
                // error log empty due to change in application name.
                //

                appName = context.Request.ServerVariables["APPL_MD_PATH"];
            }

            if (string.IsNullOrEmpty(appName))
            {
                //
                // Still no luck? Try HttpRuntime.AppDomainAppVirtualPath,
                // which is available even under partial trust.
                //

                appName = HttpRuntime.AppDomainAppVirtualPath;
            }

            return (string.IsNullOrEmpty(appName) ? "/" : appName);
#endif
        }

        #endregion
    }
}
