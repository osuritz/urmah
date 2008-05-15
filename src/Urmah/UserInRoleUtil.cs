using System;
using System.Web;
using System.Web.Security;

namespace Urmah
{
    internal sealed class UserInRoleUtil : PageBase
    {
        internal enum OperationType
        {
            Add,
            Remove
        }

        private OperationType _operationType;


        private string Username
        {
            get { return Request.QueryString["id"]; }
        }

        private string RoleName
        {
            get { return Request.QueryString["role"]; }
        }

        
        internal UserInRoleUtil(OperationType op)
        {
            _operationType = op;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Roles.Enabled && !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(RoleName))
            {
                switch (_operationType)
                {
                    case OperationType.Add:
                        AddRole();
                        break;

                    case OperationType.Remove:
                        DeleteRole();
                        break;
                }
                Response.Redirect(string.Format("{0}/users/detail?id={1}", BasePageName, Username));
            }
            else
            {
                Response.Redirect(BasePageName);
            }
        }

        private void AddRole()
        {
            Roles.AddUserToRole(Username, RoleName);
        }

        private void DeleteRole()
        {
            Roles.RemoveUserFromRole(Username, RoleName);
        }
    }
}
