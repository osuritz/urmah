﻿using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Security;

using CultureInfo = System.Globalization.CultureInfo;
using TextResource = Urmah.Resources.Resource;
using StringBuilder = System.Text.StringBuilder;

namespace Urmah
{
    internal sealed class UserDetailPage : PageBase
    {
        private string Username
        {
            get { return Request.QueryString["id"]; }
        }
        
        private MembershipUser _user = null;
        private MembershipUser RequestedUser
        {
            get
            {
                if (_user == null)
                {
                    _user = Membership.GetUser(Username);
                }
                return _user;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (RequestedUser != null)
            {
                PageTitle = string.Format(TextResource.UserDetailPageTitleFormatString, RequestedUser.UserName);
            }
            
            base.OnLoad(e);
        }

        private static void RenderNoUser(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.Write(TextResource.UserNotFoundInDataStore);
            writer.RenderEndTag(); // </p>
            writer.WriteLine();
        }
        
        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");                        

            if (RequestedUser == null)
            {
                RenderNoUser(writer);
            }
            else
            {
                RenderUser(writer);
            }
            
            base.RenderContents(writer);
        }

        private void RenderTitle(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "PageTitle");
            writer.RenderBeginTag(HtmlTextWriterTag.H1);


            writer.Write(TextResource.UserDetailTitleFormatString, RequestedUser.UserName, RequestedUser.Email);

            //writer.AddAttribute(HtmlTextWriterAttribute.Title, this.Server.HtmlEncode(this.ApplicationName));

            writer.RenderEndTag(); // </h1>
            writer.WriteLine();
        }

        private void RenderUser(HtmlTextWriter writer)
        {
            // Write out the page title containing user.
            
            RenderTitle(writer);

            SpeedBar.Render(writer,
                SpeedBar.Home.Format(BasePageName),
                SpeedBar.Users.Format(BasePageName),
                Roles.Enabled ? SpeedBar.Roles.Format(BasePageName) : null,
                SpeedBar.Help,
                SpeedBar.About.Format(BasePageName));

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "DetailsView");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            RenderAccountInformation(writer);

            if (Roles.Enabled)
            {
                RenderRoles(writer);
            }
            
            writer.RenderEndTag(); // </div>
        }

        private void RenderAccountInformation(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.H2);
            writer.Write(TextResource.UserDetailAccountInformationTitle);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "box account-info");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            // First Block of Information
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "column");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            Table accountTable = new Table();
            TableRow row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailUsernameCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), RequestedUser.UserName, "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailEmailCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), RequestedUser.Email, "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailLastActivityCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), GetTimeAndDateString(RequestedUser.LastActivityDate), "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailLastLoginCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), GetTimeAndDateString(RequestedUser.LastLoginDate), "field-value"));
            accountTable.Rows.Add(row);
            accountTable.RenderControl(writer);
            writer.RenderEndTag(); //   </div>

            // Second Block of Information
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "column");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            accountTable = new Table();
            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailCreationDateCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), GetTimeAndDateString(RequestedUser.CreationDate), "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailAccountIsApprovedCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), CapitalizeFirstLetter(RequestedUser.IsApproved ? TextResource.True : TextResource.False), "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailAccountIsLockedOutCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), CapitalizeFirstLetter(RequestedUser.IsLockedOut ? TextResource.True : TextResource.False), "field-value"));
            accountTable.Rows.Add(row);

            row = new TableRow();
            row.Cells.Add(FormatCell(new TableCell(), TextResource.UserDetailIsOnlineCaption, "field-caption"));
            row.Cells.Add(FormatCell(new TableCell(), CapitalizeFirstLetter(RequestedUser.IsOnline ? TextResource.Online : TextResource.Offline), "field-value"));
            accountTable.Rows.Add(row);
            accountTable.RenderControl(writer);
            writer.RenderEndTag(); //   </div>


            writer.RenderEndTag(); //   </div>
        }

        private void RenderRoles(HtmlTextWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.H2);
            writer.Write(TextResource.UserDetailRolesTitle);
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "box roles");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            string[] roles = Roles.GetRolesForUser(Username);

            Table rolesTable = new Table();

            for (int i = 0; i < roles.Length; i++)
            {
                string roleName = roles[i];
                TableRow row = new TableRow();

                row.Cells.Add(FormatCell(new TableCell(), roleName, ""));

                TableCell cell = new TableCell();
                HyperLink removeRoleLink = new HyperLink() { Text = TextResource.UserInRoleDelete };
                removeRoleLink.NavigateUrl = string.Format("deleteRole?id={0}&role={1}", Username, roleName);
                cell.Controls.Add(removeRoleLink);
                row.Cells.Add(cell);

                rolesTable.Rows.Add(row);
            }                        
            rolesTable.RenderControl(writer);

            // Write out a Dropdown list with the list of roles.
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "action-bar");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute("method", "GET");
            writer.AddAttribute("action", "addRole");
            writer.RenderBeginTag(HtmlTextWriterTag.Form);
            
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "id");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, Username);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            
            writer.Write(TextResource.UserInRoleAddRoleCaption);

            DropDownList ddlRoles = new DropDownList() { ID = "role" };
            string[] allRoles = Roles.GetAllRoles();
            foreach (string roleName in allRoles)
            {
                if (!Roles.IsUserInRole(Username, roleName))
                {
                    ddlRoles.Items.Add(roleName);
                }
            }
            if (ddlRoles.Items.Count == 0)
            {
                ddlRoles.Enabled = false;
            }
            ddlRoles.RenderControl(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, TextResource.UserInRoleAddRoleOkButtonCaption);
            if (ddlRoles.Items.Count == 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            writer.RenderEndTag(); //   </form>
            writer.RenderEndTag(); // </div>

            writer.RenderEndTag(); //   </div>
        }
    }
}
