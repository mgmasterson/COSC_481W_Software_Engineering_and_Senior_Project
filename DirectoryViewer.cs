using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public class DirectoryViewer
    {
        private static Boolean showUnauthorizedDirectories = true;

        // Write file links and descriptors.
        private static void post_dir_files(string loc, Control area) 
        {
            // Get the proper full name of the file directory
            System.IO.FileInfo df = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(loc)); 
            var files = System.IO.Directory.EnumerateFiles(df.FullName, "*");

            foreach (string cfile in files)
            {
                // strip out the path of the file folder of this
                string ncfile = cfile.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty);

                // Create the div, allowing for coloring
                area.Controls.Add(new LiteralControl("<div class=\"fileEntry\">")); 

                //text space               
                area.Controls.Add(new LiteralControl(Path.GetFileName(ncfile)));

                //generate download icon
                HyperLink imgb = new HyperLink();
                imgb.ImageUrl = "";
                imgb.Attributes["class"] = "fa fa-download fa-fw";
                imgb.Attributes["aria-hidden"] = "true";
                imgb.NavigateUrl = "download.aspx?f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(ncfile));
                area.Controls.Add(imgb);

                //generate file icon (view)
                HyperLink imgb2 = new HyperLink();
                imgb2.ImageUrl = "";
                imgb2.Attributes["class"] = "fa fa-file fa-fw";
                imgb2.Attributes["aria-hidden"] = "true";
                imgb2.NavigateUrl = "download.aspx?type=view&f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(ncfile));
                area.Controls.Add(imgb2);
                FileInfo fin = new FileInfo(cfile);

                // close the div, like a fake \n.
                area.Controls.Add(new LiteralControl("</div>")); 
            }
        }

        private static void post_dirs(string loc, Panel pan)
        {
            System.IO.FileInfo df = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(loc));
            
            // Get all the directories in the current dir
            var directories = System.IO.Directory.EnumerateDirectories(df.FullName); 

            foreach (string cfile in directories)
            {
                try
                {
                    // This throws an exception if we can't read the contents of the directory.
                    Directory.GetFiles(cfile); 
                    pan.Controls.Add(new LiteralControl("<div class=\"directoryEntry\">"));

                    // Get just the directory's name
                    string labelfile = new DirectoryInfo(cfile).Name;

                    // Get JUST the directory name, without its path in the heirarchy 
                    string ncfile = cfile.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty); 
                    LinkButton link = new LinkButton();
                    link.Text = labelfile; // Get
                    link.ID = "path" + Convert.ToBase64String(Encoding.UTF8.GetBytes(ncfile));
                    pan.Controls.Add(link);
                    pan.Controls.Add(new LiteralControl("</div>"));
                }
                catch (UnauthorizedAccessException uaex)
                {
                    if (showUnauthorizedDirectories)
                    {
                        pan.Controls.Add(new LiteralControl("<div class=\"directoryEntry\">"));
                        pan.Controls.Add(new LiteralControl(cfile.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty)));
                        pan.Controls.Add(new LiteralControl("</div>"));
                    }
                }
            }
        }

        //post directory contents
        public static void post_dir_contents(string loc, Panel pan, Control area)
        {
            try
            {
                post_dir_files(loc, area);
                post_dirs(loc, pan);
            }
            catch (Exception err)
            {
                ((HtmlGenericControl)area).InnerHtml += (err.Message);
            }
        }
    }
}