using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class download : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //get file path
                string path = Encoding.UTF8.GetString(Convert.FromBase64String(Request.QueryString["f"]));

                //the type of request
                string type = "application/octet-stream";

                //if request isn't null...
                if (!(Request.QueryString["type"] == null))
                {
                    //if request is of type view...
                    if (Request.QueryString["type"] == "view")
                    {
                        //the file extention
                        string extension = System.IO.Path.GetExtension(System.IO.Path.GetFileName(path));
                        switch (extension)
                        {
                            case ".pdf":
                                type = "Application/pdf";
                                break;
                            case ".jpg":
                            case ".jpeg":
                                type = "image/jpeg";
                                break;
                            case ".png":
                                type = "image/png";
                                break;
                            case ".txt":
                                type = "text/plain";
                                break;
                            case ".doc":
                            case ".docx":
                                type = "Application/msword";
                                break;
                            case ".xls":
                            case ".xlsx":
                                type = "Application/msexcel";
                                break;
                            default:
                                type = "application/octet-stream";
                                break;
                        }

                    }
                }
                mysql qery = new mysql();
                qery.RecordFileAccess(path);
                download_file(System.IO.Path.GetFileName(path), path, type);
            }
            catch (Exception ex)
            {

            }
        }

        //download files
        public static void download_file(string fileName, string filePath, string type)
        {
            HttpContext.Current.Response.ContentType = type;
            string header;
            if (type == "application/octet-stream")
            {
                header = "Attachment; Filename=" + fileName;
            }
            else
            {
                header = "Filename=" + fileName;
            }
            HttpContext.Current.Response.AppendHeader("Content-Disposition", header);
            HttpContext.Current.Response.WriteFile(filePath);
            HttpContext.Current.Response.End();
        }
    }
}