using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Web.UI.HtmlControls;

namespace WebApplication2
{
    public partial class Default : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //pull up the query part of the program
            mysql query = new mysql();

            //directory location of files
            string currentFolder;

            // Check to see whether or not the pub\reports\[yourname] folder exists. If it does...
            try
            {
                Directory.GetFiles(HttpContext.Current.Server.MapPath("pub\\reports\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name));
                currentFolder = "pub\\reports\\" + System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch (Exception ex)
            {
                currentFolder = "pub\\reports\\";
            }
            Panel1.Controls.Clear();

            // If the path is not already defined, set it to the default folder, and skip the rest of the logic. 
            // We shouldn't need it, since the only state where the path is undefined is when you've just loaded the page.
            if ((string)(ViewState["path"]) == "" || ViewState["path"] == null)
            {
                ViewState["path"] = currentFolder;
            }
            else
            {
                // Since we know the path is not null, and exists, we set where we're located by that value.
                currentFolder = (string)ViewState["path"]; 

                if (IsPostBack)
                {
                    if (acsfafileupload.HasFile)
                    {
                        try
                        {
                            acsfafileupload.PostedFile.SaveAs(Server.MapPath(currentFolder) + "\\" + acsfafileupload.FileName);
                        }
                        catch (Exception ex)
                        {
                            TargetArea.InnerText += ex.ToString();
                        }
                    }
                }
                try
                {
                    // To determine what action to take, we grab the ID of the linkbutton that was clicked.
                    // We take the first four characters, and use them to select what action to take
                    string sw = this.Request.Params["__EVENTTARGET"].Substring(0, 4);
                    string targetPath = Encoding.UTF8.GetString(Convert.FromBase64String(this.Request.Params["__EVENTTARGET"].Substring(4)));
                    switch (sw)
                    {
                        case "path":
                        case "favv":
                            ViewState["path"] = targetPath;
                            currentFolder = targetPath;
                            break;
                        case "addf":
                            query.AddFavorites(targetPath);
                            break;
                        case "favr":
                            query.DeleteFavorites(targetPath);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex) { }


            }
            currentFolderName.InnerText = currentFolder;

            // Create a new linkbutton, to refresh the page. Because we're lazy
            LinkButton refresh = new LinkButton();
            refresh.Attributes["class"] = "fa fa-refresh";
            refresh.Attributes["aria-hidden"] = "true";
            refreshButton.Controls.Add(refresh);

            // Create a new linkbutton to add the current folder to your favorites. If for some reason you want to add the default folder to your favorites, we won't judge. Much.
            LinkButton addFavoritesButton = new LinkButton();
            addFavoritesButton.Attributes["class"] = "fa fa-star fa-fw";
            addFavoritesButton.Attributes["aria-hidden"] = "true";

            // Decode the file name
            addFavoritesButton.ID = "addf" + Convert.ToBase64String(Encoding.UTF8.GetBytes(currentFolder));
            currentFolderName.Controls.Add(addFavoritesButton);
            query.ViewFavorites(favoritesArea);
            query.ViewTime(recentlyViewedArea);
            query.ViewUsage(mostOftenViewedArea);
            DirectoryViewer.post_dir_contents(currentFolder, Panel1, TargetArea);
        }
    }
}