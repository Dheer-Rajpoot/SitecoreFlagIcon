using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Security.AccessControl;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentManager.Galleries;
using Sitecore.Shell.Applications.ContentManager.Galleries.Languages;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using System;
using System.Globalization;

namespace SitecoreFlagIcon
{
    public class GalleryLanguagesForm : GalleryForm
    {
        /// <summary></summary>
        protected Scrollbox Languages;

        /// <summary></summary>
        protected GalleryMenu Options;
      
        /// <summary>
        /// Raises the load event.
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        ///             request for the page it is associated with, such as setting up a database query. At this
        ///             stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        ///             view state is restored, and form controls reflect client-side data. Use the IsPostBack
        ///             property to determine whether the page is being loaded in response to a client postback,
        ///             or if it is being loaded and accessed for the first time.
        /// 
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull((object)e, "e");
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
                return;
            Item currentItem = GalleryLanguagesForm.GetCurrentItem();
            if (currentItem == null)
                return;
            using (new ThreadCultureSwitcher(Context.Language.CultureInfo))
            {
                foreach (Language language in currentItem.Languages)
                {
                    string languageIcon = string.Empty;
                    ID languageItemId = LanguageManager.GetLanguageItemId(language, currentItem.Database);
                    if (!ItemUtil.IsNull(languageItemId))
                    {
                        Item obj = currentItem.Database.GetItem(languageItemId);
                        if (obj != null)
                        {
                            languageIcon = obj["__icon"];
                        }
                        if (obj == null || !obj.Access.CanRead() || obj.Appearance.Hidden && !UserOptions.View.ShowHiddenItems)
                            continue;
                    }
                    XmlControl xmlControl = ControlFactory.GetControl("Gallery.Languages.Option") as XmlControl;
                    Assert.IsNotNull((object)xmlControl, typeof(XmlControl));
                    Context.ClientPage.AddControl((System.Web.UI.Control)this.Languages, (System.Web.UI.Control)xmlControl);
                    Item obj1 = currentItem.Database.GetItem(currentItem.ID, language);
                    if (obj1 != null)
                    {
                        int length = obj1.Versions.GetVersionNumbers(false).Length;
                        string str1;
                        if (length != 1)
                            str1 = Translate.Text("{0} versions.", (object)length.ToString());
                        else
                            str1 = Translate.Text("1 version.");
                        string str2 = str1;
                        CultureInfo cultureInfo = language.CultureInfo;
                        xmlControl["Header"] = (object)(cultureInfo.DisplayName + " : " + cultureInfo.NativeName);
                        xmlControl["Description"] = (object)str2;
                        xmlControl["Click"] = (object)string.Format("item:load(id={0},language={1},version=0)", (object)currentItem.ID, (object)language);
                        xmlControl["ClassName"] = !language.Name.Equals(WebUtil.GetQueryString("la"), StringComparison.OrdinalIgnoreCase) ? (object)"scMenuPanelItem" : (object)"scMenuPanelItemSelected";
                        var lang = LanguageDefinitions.GetLanguageDefinition(language);
                        if(lang!=null)
                            languageIcon = lang.Icon;
                        
                     xmlControl["Icon"] = languageIcon; 
                    }
                }
            }
            Item obj2 = Sitecore.Client.CoreDatabase.GetItem("/sitecore/content/Applications/Content Editor/Menues/Languages");
            if (obj2 == null)
                return;
            this.Options.AddFromDataSource(obj2, string.Empty);
        }

        /// <summary>
        /// Gets the current item.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The current item.
        /// </returns>
        private static Item GetCurrentItem()
        {
            string queryString1 = WebUtil.GetQueryString("db");
            string queryString2 = WebUtil.GetQueryString("id");
            Language language = Language.Parse(WebUtil.GetQueryString("la"));
            Sitecore.Data.Version version = Sitecore.Data.Version.Parse(WebUtil.GetQueryString("vs"));
            Database database = Factory.GetDatabase(queryString1);
            Assert.IsNotNull((object)database, queryString1);
            return database.GetItem(queryString2, language, version);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (message.Name == "event:click")
            {
                return;
            }
            base.Invoke(message, true);
        }
    }
}
