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
        protected override void OnLoad(EventArgs e)
        {
            PageTitle = TitleString;

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
                int onlineCount = Membership.GetNumberOfUsersOnline();
                
                RenderUsersContent(writer, userCount, onlineCount);
            }
            catch (Exception ex)
            {
                RenderUsersException(writer, ex);
            }

            writer.RenderEndTag(); // </div>
        }

        private void RenderUsersContent(HtmlTextWriter writer, int userCount, int onlineCount)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);
                        
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "field-caption");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(TextResource.ExistingUsersOnlineTotalRation, userCount, onlineCount);
            writer.RenderEndTag(); // </span>
            
            writer.RenderEndTag(); // </p>

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format("{0}/users/create", BasePageName));
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(TextResource.CreateUser);
            writer.RenderEndTag(); // </a>
            writer.WriteBreak(); // <br />

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format(SpeedBar.Users.Href, BasePageName));
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

            writer.AddAttribute(HtmlTextWriterAttribute.Href, string.Format(SpeedBar.Roles.Href, BasePageName));
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
