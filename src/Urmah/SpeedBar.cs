using System;
using System.Web;
using System.Web.UI;

using TextResource = Urmah.Resources.Resource;

namespace Urmah
{
    internal sealed class SpeedBar
    {
        public static readonly ItemTemplate Home = new ItemTemplate() { Text = "Dashboard", Title = "Main Control Panel", Href = "{0}" };
        public static readonly ItemTemplate Users = new ItemTemplate() { Text = TextResource.UsersTitle, Title = TextResource.UsersLinkDescription, Href = "{0}/users" };
        public static readonly ItemTemplate Roles = new ItemTemplate() { Text = TextResource.RolesTitle, Title = TextResource.RolesLinkDescription, Href = "{0}/roles" };
        public static readonly FormattedItem Help = new FormattedItem() { Text = "Help", Title = "Documentation, discussions, issues and more", Href = "http://urmah.googlecode.com/" };
        public static readonly ItemTemplate About = new ItemTemplate() { Text = "About", Title = "Information about this version and build", Href = "{0}/about" };

        public static void Render(HtmlTextWriter writer, params FormattedItem[] items)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (items == null || items.Length == 0)
                return;

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "SpeedList");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (FormattedItem item in items)
            {
                if (item != null)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    item.Render(writer);
                    writer.RenderEndTag( /* li */);
                }
            }

            writer.RenderEndTag( /* ul */);
        }

        private SpeedBar() { }

        [Serializable]
        public abstract class Item
        {
            public string Text { get; set; }
            public string Title { get; set; }
            public string Href { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        [Serializable]
        public sealed class ItemTemplate : Item
        {
            public FormattedItem Format(string url)
            {
                return new FormattedItem() { Text = this.Text, Title = this.Title, Href = string.Format(this.Href, url) };
            }
        }

        [Serializable]
        public sealed class FormattedItem : Item
        {

            public void Render(HtmlTextWriter writer)
            {
                if (writer == null)
                {
                    throw new ArgumentNullException("writer");
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Href, Href);
                writer.AddAttribute(HtmlTextWriterAttribute.Title, Title);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                HttpUtility.HtmlEncode(Text, writer);
                writer.RenderEndTag( /* a */);
            }
        }
    }
}
