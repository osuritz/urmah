using System;
using System.Web.UI;

namespace Urmah
{
    internal sealed class UserPageFactory
    {
        internal static Page GetPage(string name)
        {
            switch (name)
            {
                case "create":
                case "new":
                    return null;

                case "detail":
                case "view":
                    return new UserDetailPage();

                case "addrole":
                    return new UserInRoleUtil(UserInRoleUtil.OperationType.Add);
                
                case "deleterole":
                    return new UserInRoleUtil(UserInRoleUtil.OperationType.Remove);

                default:
                    return name.Length == 0 ? new UsersBrowserPage() : null;
            }
        }
    }
}
