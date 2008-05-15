using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

using CultureInfo = System.Globalization.CultureInfo;
using TextResource = Urmah.Resources.Resource;

namespace Urmah
{
    internal sealed class UserAndRolePage : PageBase
    {
        //private const int _defaultPageSize = 15;
        
        //private int _pageIndex;
        //private int _pageSize;
        //private int _totalCount;
        //private ArrayList _errorEntryList;

        
        //private const int _maximumPageSize = 100;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            RenderTitle(writer);

            SpeedBar.Render(writer,
                SpeedBar.Users.Format(BasePageName),
                Roles.Enabled ? SpeedBar.Roles.Format(BasePageName) : null,
                SpeedBar.Help,
                SpeedBar.About.Format(BasePageName));

            RenderControlPanel(writer);
            
            
            base.RenderContents(writer);
        }

        private void RenderTitle(HtmlTextWriter writer)
        {
            //
            // If the application name matches the APPL_MD_PATH then its
            // of the form /LM/W3SVC/.../<name>. In this case, use only the 
            // <name> part to reduce the noise. The full application name is 
            // still made available through a tooltip.
            //

            string simpleName = this.ApplicationName;

            if (string.Compare(simpleName, this.Request.ServerVariables["APPL_MD_PATH"],
                true, CultureInfo.InvariantCulture) == 0)
            {
                int lastSlashIndex = simpleName.LastIndexOf('/');

                if (lastSlashIndex > 0)
                {
                    simpleName = simpleName.Substring(lastSlashIndex + 1);
                }
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "PageTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.H1);
                        
            writer.Write(TextResource.TitleFormatString, Server.HtmlEncode(simpleName), Server.HtmlEncode(Environment.MachineName));
            
            //writer.AddAttribute(HtmlTextWriterAttribute.Title, this.Server.HtmlEncode(this.ApplicationName));

            writer.RenderEndTag(); // </h1>
            writer.WriteLine();
        }

        private void RenderControlPanel(HtmlTextWriter writer)
        {
            RenderUsersBlock(writer);
            RenderRolesBlock(writer);
        }
        
        private void RenderUsersBlock(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "control-panel-block");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.RenderBeginTag(HtmlTextWriterTag.H2);
            writer.Write(TextResource.UsersTitle);
            writer.RenderEndTag(); // </h2>

            try
            {
                int userCount;
                Membership.GetAllUsers(0, 1, out userCount);
                RenderUsersContent(writer, userCount);
            }
            catch (Exception ex)
            {
                RenderUsersException(writer, ex);
            }

            writer.RenderEndTag(); // </div>
        }

        private void RenderUsersContent(HtmlTextWriter writer, int userCount)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);
                        
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "field-caption");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(TextResource.ExistingUsersFormatString, userCount);
            writer.RenderEndTag(); // </span>
            
            writer.RenderEndTag(); // </p>

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("{0}/users/create", BasePageName));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(TextResource.CreateUser);
            writer.RenderEndTag(); // </a>
            writer.WriteBreak(); // <br />

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("{0}/users", BasePageName));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(TextResource.ManageUsers);
            writer.RenderEndTag(); // </a>
        }

        private void RenderUsersException(HtmlTextWriter writer, Exception ex)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Em);
            writer.Write(TextResource.MembershipExceptionMessage, ex.Message);
            writer.RenderEndTag(); // </em>
        }

        private void RenderRolesBlock(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "control-panel-block");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            
            writer.RenderBeginTag(HtmlTextWriterTag.H2);
            writer.Write(TextResource.RolesTitle);
            writer.RenderEndTag(); // </h2>

            if (Roles.Enabled)
            {
                RenderRolesContent(writer);
            }
            else
            {
                RenderRolesDisabled(writer);
            }

            writer.RenderEndTag(); // </div>
        }

        private void RenderRolesContent(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "field-caption");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(TextResource.ExistingRolesFormatString, Roles.GetAllRoles().Length);
            writer.RenderEndTag(); // </span>
            
            writer.RenderEndTag(); // </p>

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("{0}/roles/create", BasePageName));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(TextResource.CreateRole);
            writer.RenderEndTag(); // </a>
            writer.WriteBreak(); // <br />

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("{0}/roles/manage", BasePageName));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(TextResource.ManageRoles);
            writer.RenderEndTag(); // </a>
        }

        private void RenderRolesDisabled(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Em);
            writer.Write(TextResource.RoleManagementDisabled);
            writer.RenderEndTag(); // </em>
        }
    }
}
