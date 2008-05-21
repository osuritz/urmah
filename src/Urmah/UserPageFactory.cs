using System;
using System.Web;
using System.Web.UI;

namespace Urmah
{
    internal sealed class UserPageFactory
    {
        internal static Page GetHandler(string name)
        {
            switch (name)
            {
                case "create":
                case "new":
                    return new EditUserPage();

                case "detail":
                case "view":
                    return new UserDetailPage();

                case "addrole":
                    return new UserInRoleUtil(UserInRoleUtil.OperationType.Add);
                
                case "deleterole":
                    return new UserInRoleUtil(UserInRoleUtil.OperationType.Remove);

                case "do":
                    return new UserActions();

                default:
                    return name.Length == 0 ? new UsersBrowserPage() : null;
            }
        }
    }
}
