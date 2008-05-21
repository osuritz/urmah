using System;
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
    internal sealed class UsersBrowserPage : ListingTableBase
    {
        private MembershipUserCollection _userList;

        private string SearchName
        {
            get
            {
                return Request.QueryString["username"];
            }
        }

        protected override string ContentTitleFormatString
        {
            get { return TextResource.UserBrowserTitleFormatString; }
        }

        protected override void OnLoad(EventArgs e)
        {
            PageTitle = TitleString;
            
            int totalCount;
            
            // Read the user records.
            if (string.IsNullOrEmpty(SearchName))
            {
                _userList = Membership.GetAllUsers(PageIndex, PageSize, out totalCount);
            }
            else
            {
                _userList = Membership.FindUsersByName(SearchName, PageIndex, PageSize, out totalCount);
            }
            TotalCount = totalCount;
                                    
            base.OnLoad(e);
        }               

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            RenderTitle(writer);

            SpeedBar.Render(writer,
                SpeedBar.Home.Format(BasePageName),
                Roles.Enabled ? SpeedBar.Roles.Format(BasePageName) : null,
                SpeedBar.Help,
                SpeedBar.About.Format(BasePageName));

            RenderSearchForm(writer);
            
            if (_userList.Count > 0)
            {
                // Write user number range displayed on this page and the total available
                // in the data store, followed by stock page sizes.

                writer.RenderBeginTag(HtmlTextWriterTag.P);

                RenderStats(writer);
                //RenderStockPageSizes(writer);

                writer.RenderEndTag(); // </p>
                writer.WriteLine();

                // Write out the main table to display the users.
                RenderUsers(writer);

                // Write out page navigation links.
                RenderPageNavigators(writer);

            }
            else
            {
                // No users found in the data store, so display a corresponding message.
                RenderNoUsers(writer);
            }

            RenderControls(writer);
                       
            base.RenderContents(writer);
        }


        private void RenderStats(HtmlTextWriter writer)
        {
            int firstNumber = PageIndex * PageSize + 1;
            int lastNumber = firstNumber + _userList.Count - 1;
            int totalPages = (int)Math.Ceiling((double)TotalCount / PageSize);

            writer.Write(TextResource.UserPagingStats,
                         firstNumber.ToString("N0"),
                         lastNumber.ToString("N0"),
                         TotalCount.ToString("N0"),
                         (PageIndex + 1).ToString("N0"),
                         totalPages.ToString("N0"));
        }


        private void RenderSearchForm(HtmlTextWriter writer)
        {
            writer.AddAttribute("method", "GET");
            writer.RenderBeginTag(HtmlTextWriterTag.Form);
            
            writer.Write(TextResource.UserSearchFormUsername);

            TextBox searchBox = new TextBox() { ID = "username" };
            searchBox.Text = SearchName;
            searchBox.RenderControl(writer);

            Button submitButton = new Button() { Text = TextResource.UserSearchFormSubmitText };
            submitButton.RenderControl(writer);

            writer.RenderEndTag(); // </form>
        }
        
        private void RenderUsers(HtmlTextWriter writer)
        {
            // Create a table to display user  in each row.
            Table table = new Table();
            table.ID = "UserList";
            table.CellSpacing = 0;

            // Create the table row for headings.

            TableRow headRow = new TableRow();

            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "User Name", "name-col"));
            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "Email", "email-col"));
            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "Comment", "comm-col"));
            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "Account Status", "lockout-col"));
            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "Last Login", "lastlogin-col"));
            headRow.Cells.Add(FormatCell(new TableHeaderCell(), "", "action-col"));

            table.Rows.Add(headRow);

            // Generate a table body row for each user.
            int userIndex = 0;
            foreach (MembershipUser user in _userList)
            {
                TableRow bodyRow = new TableRow();

                bodyRow.CssClass = userIndex % 2 == 0 ? "even-row" : "odd-row";

                // Format online status cell
                TableCell cell;
                Image img;

                cell = new TableCell() { Wrap = false, CssClass = "lockout-col" };
                img = new Image() { ImageUrl = GetUrl("/cleardot"), CssClass = (user.IsOnline ? "online" : "offline") };
                img.ToolTip = (user.IsOnline ? TextResource.UserIsOnline : TextResource.UserIsOffline);
                cell.Controls.Add(img);
                cell.Controls.Add(new LiteralControl(" "));
                cell.Controls.Add(new LiteralControl(user.UserName));
                
                cell.Wrap = false;
                cell.CssClass = "name-col" ;
                bodyRow.Cells.Add(cell);
                
                //bodyRow.Cells.Add(FormatCell(new TableCell(), user.UserName, "name-col"));
                bodyRow.Cells.Add(FormatCell(new TableCell(), user.Email, "email-col"));
                bodyRow.Cells.Add(FormatCell(new TableCell(), user.Comment ?? string.Empty, "comm-col"));
                                
                // Format lockout status cell
                cell = new TableCell() { Wrap = false, CssClass = "lockout-col" };
                img = new Image() { ImageUrl = GetUrl("/cleardot"), CssClass = (user.IsLockedOut ? "locked-out" : "active") };
                img.ToolTip = (user.IsLockedOut ? TextResource.UserAccountLockedOut : TextResource.UserAccountNotLockedOut);
                cell.Controls.Add(img);

                bodyRow.Cells.Add(cell);
                //bodyRow.Cells.Add(FormatCell(new TableCell(), user.IsLockedOut ? "Locked OUT" : "" , "lockout-col"));

                bodyRow.Cells.Add(FormatCell(new TableCell(), user.LastLoginDate.ToShortDateString() + " @ " + user.LastLoginDate.ToShortTimeString(), "lastlogin-col"));

                // Action cell
                cell = new TableCell();
                HyperLink detailsLink = new HyperLink() { Text = TextResource.UsersActionDetail };
                detailsLink.NavigateUrl = string.Format("{0}/detail?id={1}", this.Request.Path, user.UserName);
                cell.Controls.Add(detailsLink);
                
                bodyRow.Cells.Add(cell);


                table.Rows.Add(bodyRow);
                userIndex++;
            }
            
            //for (int userIndex = 0; userIndex < _userList.Count; userIndex++)
            //{
            //    ErrorLogEntry errorEntry = (ErrorLogEntry)_errorEntryList[userIndex];
            //    Error error = errorEntry.Error;

            //    TableRow bodyRow = new TableRow();
            //    bodyRow.CssClass = userIndex % 2 == 0 ? "even-row" : "odd-row";

            //    //
            //    // Format host and status code cells.
            //    //

            //    bodyRow.Cells.Add(FormatCell(new TableCell(), error.HostName, "host-col"));
            //    bodyRow.Cells.Add(FormatCell(new TableCell(), error.StatusCode.ToString(), "code-col", Mask.NullString(HttpWorkerRequest.GetStatusDescription(error.StatusCode))));
            //    bodyRow.Cells.Add(FormatCell(new TableCell(), GetSimpleErrorType(error), "type-col", error.Type));

            //    //
            //    // Format the message cell, which contains the message 
            //    // text and a details link pointing to the page where
            //    // all error details can be viewed.
            //    //

            //    TableCell messageCell = new TableCell();
            //    messageCell.CssClass = "error-col";

            //    Label messageLabel = new Label();
            //    messageLabel.Text = this.Server.HtmlEncode(error.Message);

            //    HyperLink detailsLink = new HyperLink();
            //    detailsLink.NavigateUrl = this.Request.Path + "/detail?id=" + errorEntry.Id;
            //    detailsLink.Text = "Details&hellip;";

            //    messageCell.Controls.Add(messageLabel);
            //    messageCell.Controls.Add(new LiteralControl(" "));
            //    messageCell.Controls.Add(detailsLink);

            //    bodyRow.Cells.Add(messageCell);

            //    //
            //    // Format the user, date and time cells.
            //    //

            //    bodyRow.Cells.Add(FormatCell(new TableCell(), error.User, "user-col"));
            //    bodyRow.Cells.Add(FormatCell(new TableCell(), error.Time.ToShortDateString(), "date-col",
            //        error.Time.ToLongDateString()));
            //    bodyRow.Cells.Add(FormatCell(new TableCell(), error.Time.ToLongTimeString(), "time-col"));

            //    //
            //    // Finally, add the row to the table.
            //    //

            //    table.Rows.Add(bodyRow);
            //}

            table.RenderControl(writer);
        }

        private void RenderNoUsers(HtmlTextWriter writer)
        {

        }

        private void RenderControls(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "controls");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            Image img = GetSpriteImage("add");
            img.RenderControl(writer);

            HyperLink addRoleLink = new HyperLink() { Text = TextResource.CreateUser };
            addRoleLink.NavigateUrl = string.Format("{0}/create", this.Request.Path);
            addRoleLink.RenderControl(writer);

            writer.RenderEndTag(); // </div>
        }
    }
}
