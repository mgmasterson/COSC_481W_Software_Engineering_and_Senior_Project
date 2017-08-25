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
using System.Diagnostics;
using System.Web.UI.HtmlControls;

namespace WebApplication2
{
    public class mysql
    {
        //database connection variables
        static string defaultConnectionHost = "localhost";
        static string defaultConnectionPort = 3306.ToString();
        static string defaultConnectionUser = "root";
        static string defaultConnectionPassword = "g0d0fwar";

        //connection to database
        MySqlConnection con = new MySqlConnection(@"Data Source="+defaultConnectionHost+";port="+defaultConnectionPort+";Initial Catalog=advstats;User Id="+defaultConnectionUser+";password='"+defaultConnectionPassword+"'");

        //get current user
        string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        public void RecordFileAccess(string filename)
        {
            int retrievedValue = 0;
            con.Open();

            // first, we check if the file exists in the database already.
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select filename from fileAccess where userName = @username and fileName = @filename;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@filename", filename);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if(reader.GetString(0) == filename)
                    {
                        retrievedValue++;
                    }
                }
                reader.Close();
            }
            MySqlCommand cmd2 = con.CreateCommand();

            //if this is first time access, insert its own row in the table
            if(retrievedValue == 0)
            {
                cmd2.CommandText = "insert into fileAccess values ( @username, @filename, @timestamp, 1);";
                cmd2.Prepare();
                cmd2.Parameters.AddWithValue("@username", username);
                cmd2.Parameters.AddWithValue("@filename", filename);
                cmd2.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd2.ExecuteNonQuery();
            }

            //otherwise update its existing row
            else
            {
                cmd2.CommandText = "update fileAccess set ts = @timestamp, amount = amount + 1 where userName = @username and fileName = @filename";
                cmd2.Prepare();
                cmd2.Parameters.AddWithValue("@username", username);
                cmd2.Parameters.AddWithValue("@filename", filename);
                cmd2.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
                cmd2.ExecuteNonQuery();
            }
            con.Close();
        }

        public void AddUsage(string filename)
        {
            //check if exists
            //if not insert
            //if it does, increment amount
            int retrievedValue = 0;
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select fileName from fileAccess where userName = @username and fileName = @filename;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@filename", filename);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(0) == filename)
                    {
                        retrievedValue++;
                    }
                }
                reader.Close();
            }
            con.Close();
            if (retrievedValue == 0)
            {
                con.Open();
                MySqlCommand cmd2 = con.CreateCommand();
                cmd2.CommandText = "insert into fileuseamount values( @username , @filename , 1);";
                cmd2.Prepare();
                cmd2.Parameters.AddWithValue("@username", username);
                cmd2.Parameters.AddWithValue("@filename", filename);
                cmd2.ExecuteNonQuery();
                con.Close();
            }
            else
            {
                con.Open();
                MySqlCommand cmd2 = con.CreateCommand();
                cmd2.CommandText = "update fileuseamount set amount = amount + 1 where userName = @username and fileName = @filename;";
                cmd2.Prepare();
                cmd2.Parameters.AddWithValue("@username", username);
                cmd2.Parameters.AddWithValue("@filename", filename);
                cmd2.ExecuteNonQuery();
                con.Close();

            }
            retrievedValue = 0;
        }

        public void ViewUsage(HtmlGenericControl mostOftenViewedArea)
        {
            con.Open();
            MySqlCommand cmd = con.CreateCommand();

            //retrieve top 5 entries
            cmd.CommandText = "select fileName from fileAccess where userName = @username order by amount DESC Limit 5; ";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                mostOftenViewedArea.InnerHtml = "";
                while (reader.Read())
                {
                    mostOftenViewedArea.Controls.Add(new LiteralControl("<div>"));
                    mostOftenViewedArea.Controls.Add(new LiteralControl(Path.GetFileName(reader.GetString(0))));

                    //generate download icon
                    HyperLink imgb = new HyperLink();
                    imgb.ImageUrl = "";
                    imgb.Attributes["class"] = "fa fa-download fa-fw";
                    imgb.Attributes["aria-hidden"] = "true";
                    imgb.NavigateUrl = "download.aspx?f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(reader.GetString(0)));
                    mostOftenViewedArea.Controls.Add(imgb);

                    //generate file icon (view)
                    HyperLink imgb2 = new HyperLink();
                    imgb2.ImageUrl = "";
                    imgb2.Attributes["class"] = "fa fa-file fa-fw";
                    imgb2.Attributes["aria-hidden"] = "true";
                    imgb2.NavigateUrl = "download.aspx?type=view&f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(reader.GetString(0)));
                    mostOftenViewedArea.Controls.Add(imgb2);
                    mostOftenViewedArea.Controls.Add(new LiteralControl("</div>"));
                }
                reader.Close();
            }
            con.Close();
        }

        public void AddTime(string filename)
        {
            con.Open();
            try
            {
                MySqlCommand ncmd = con.CreateCommand();
                ncmd.CommandText = "DELETE FROM fileusetime WHERE userName = @username AND fileName = @filename;";
                ncmd.Prepare();
                ncmd.Parameters.AddWithValue("@username", username);
                ncmd.Parameters.AddWithValue("@filename", filename);
                ncmd.ExecuteNonQuery();
            }
            catch(Exception ex) { }
            

            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "insert into fileusetime values( @username, @filename, @time);"; 
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@filename", filename);
            cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd H:mm:ss"));
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void DeleteTime(object sender, EventArgs e)
        {
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "delete from fileusetime where userName= @username;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.ExecuteNonQuery();
            con.Close();

        }

        public void ViewTime(HtmlGenericControl recentlyViewedArea)
        {
            con.Open();
            MySqlCommand cmd = con.CreateCommand();

            //retrieve top 5 entries
            cmd.CommandText = "select distinct fileName from fileAccess where userName = @username order by ts DESC Limit 5;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                recentlyViewedArea.InnerHtml = "";
                while (reader.Read())
                {
                    recentlyViewedArea.Controls.Add(new LiteralControl("<div>"));
                    recentlyViewedArea.Controls.Add(new LiteralControl(Path.GetFileName(reader.GetString(0))));

                    //generate download icon
                    HyperLink imgb = new HyperLink();
                    imgb.ImageUrl = "";
                    imgb.Attributes["class"] = "fa fa-download fa-fw";
                    imgb.Attributes["aria-hidden"] = "true";
                    imgb.NavigateUrl = "download.aspx?f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(reader.GetString(0)));
                    recentlyViewedArea.Controls.Add(imgb);

                    //generate file icon (view)
                    HyperLink imgb2 = new HyperLink();
                    imgb2.ImageUrl = "";
                    imgb2.Attributes["class"] = "fa fa-file fa-fw";
                    imgb2.Attributes["aria-hidden"] = "true";
                    imgb2.NavigateUrl = "download.aspx?type=view&f=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(reader.GetString(0)));
                    recentlyViewedArea.Controls.Add(imgb2);
                    recentlyViewedArea.Controls.Add(new LiteralControl("</div>"));
                }
            }
            con.Close();
        }

        public void AddFavorites(string folderName)
        {
            int retrievedValue = 0;
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select folderName from favorites where userName = @username and folderName = @foldername;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@foldername", folderName);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetString(0) == folderName)
                    {
                        retrievedValue++;
                    }
                }
                reader.Close();
            }

            con.Close();
            if (retrievedValue == 0)
            {
                con.Open();
                MySqlCommand cmd2 = con.CreateCommand();
                cmd2.CommandText = "insert into favorites values( @username, @foldername);";
                cmd2.Prepare();
                cmd2.Parameters.AddWithValue("@username", username);
                cmd2.Parameters.AddWithValue("@foldername", folderName);
                cmd2.ExecuteNonQuery();
                con.Close();
            }
            retrievedValue = 0;
        }

        public void DeleteFavorites(string folderName)
        {
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "delete from favorites where username = @username and folderName = @foldername;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@foldername", folderName);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public void ViewFavorites(HtmlGenericControl favoritesArea)
        {
            con.Open();
            MySqlCommand cmd = con.CreateCommand();
            cmd.CommandText = "select folderName from favorites where userName = @username;";
            cmd.Prepare();
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                favoritesArea.InnerHtml = "";
                while (reader.Read())
                {
                    favoritesArea.Controls.Add(new LiteralControl("<div>"));
                    var link = new LinkButton()
                    {
                        Text = reader.GetString(0),
                        ID = "favv" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader.GetString(0)))
                    };
                    favoritesArea.Controls.Add(link);
                    LinkButton deleteButton = new LinkButton();
                    deleteButton.Attributes["class"] = "fa fa-minus-square fa-fw";
                    deleteButton.Attributes["aria-hidden"] = "true";
                    deleteButton.OnClientClick = "DeleteFavorites";
                    deleteButton.ID = "favr" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(reader.GetString(0)));
                    favoritesArea.Controls.Add(deleteButton);
                    favoritesArea.Controls.Add(new LiteralControl("</div>"));
                }
                reader.Close();
            }
            con.Close();
        }
    }
}