using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CultureInfo = System.Globalization.CultureInfo;
using TextResource = Urmah.Resources.Resource;
using StringBuilder = System.Text.StringBuilder;

namespace Urmah
{
    internal class ListingTableBase : PageBase
    {
        protected virtual int DefaultPageSize { get { return 15; } }
        protected virtual int MaximumPageSize { get { return 100; } }

        
        protected int PageIndex { get; set; }
        protected int PageSize { get; set; }
        protected int TotalCount { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int pageSize;
            int.TryParse(this.Request.QueryString["size"], System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out pageSize);
            
            PageSize = Math.Min(MaximumPageSize, Math.Max(0, pageSize));

            if (PageSize == 0)
            {
                PageSize = DefaultPageSize;
            }

            int pageIndex;
            int.TryParse(this.Request.QueryString["page"], System.Globalization.NumberStyles.Integer, CultureInfo.InvariantCulture, out pageIndex);            
            PageIndex = Math.Max(1, pageIndex) - 1;
            
            base.OnInit(e);
        }
        
        protected void RenderPageNavigators(HtmlTextWriter writer)
        {
            // If not on the last page then render a link to the next page.
            writer.RenderBeginTag(HtmlTextWriterTag.P);

            int nextPageIndex = PageIndex + 1;
            bool morePages = nextPageIndex * PageSize < TotalCount;

            if (morePages)
            {
                RenderLinkToPage(writer, HtmlLinkType.Next, string.Format(TextResource.PagingNextFormatString, TextResource.Users), nextPageIndex);
            }

            // If not on the first page then render a link to the firs page.
            if (PageIndex > 0 && TotalCount > 0)
            {
                if (morePages)
                    writer.Write("; ");

                RenderLinkToPage(writer, HtmlLinkType.Start, "Back to first page", 0);
            }

            writer.RenderEndTag(); // </p>
            writer.WriteLine();
        }

        protected void RenderLinkToPage(HtmlTextWriter writer, string type, string text, int pageIndex)
        {
            RenderLinkToPage(writer, type, text, pageIndex, PageSize);
        }

        protected void RenderLinkToPage(HtmlTextWriter writer, string type, string text, int pageIndex, int pageSize)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (text == null)
            {
                throw new ArgumentNullException("text");
            }

            StringBuilder sbRemainingQueryString = new StringBuilder("&");
            foreach (string key in Request.QueryString.AllKeys)
            {
                if (key != "page" && key != "size")
                {
                    sbRemainingQueryString.AppendFormat("{0}={1}", key, Request.QueryString[key]);
                }
            }

            string href = string.Format("{0}?page={1}&size={2}{3}",
                this.Request.Path,
                (pageIndex + 1).ToString(CultureInfo.InvariantCulture),
                pageSize.ToString(CultureInfo.InvariantCulture),
                sbRemainingQueryString.Length == 1 ? string.Empty : sbRemainingQueryString.ToString());

            writer.AddAttribute(HtmlTextWriterAttribute.Href, href);

            if (type != null && type.Length > 0)
                writer.AddAttribute("rel", type);

            writer.RenderBeginTag(HtmlTextWriterTag.A);
            this.Server.HtmlEncode(text, writer);
            writer.RenderEndTag();
        }
    }
}
