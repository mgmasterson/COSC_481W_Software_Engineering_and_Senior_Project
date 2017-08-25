<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication2.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <%--latest rending engine on browsers --%>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
     <%--mobile browser--%>
    <meta name="viewport" content= "width = device-width, initial-scale =1" />

    <title>ACS Viewer</title>
    <%--Bootstrap--%>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <%--Font Awesome--%>
    <link href="css/font-awesome.min.css" rel="stylesheet" />
    <%--Database--%>
    <link href="mysql.cs" rel="stylesheet" />

    <style>
        .fileEntry:nth-child(even) { background: #FFFFFF }
        .fileEntry:nth-child(odd) { background: #F0F0F0 }
        .auto-style1 {
            width: 372px;
            height: 99px;
            margin-left: 0px;
        }

        .site-footer 
{
    background-color:grey;
    color:white;
    margin-top: 30px;
    padding-top: 10px;
    text-align: center;
    padding-bottom:10px;
}
    </style>
</head>
<body>
    <form id="form2" runat="server">
        <%--space for logo --%>
        <p><img alt="logo" class="auto-style1" src="Images/logo.png" /></p>
        <nav class="navbar navbar-inverse" style="min-height:0px;">
  <div class="container-fluid">
    <div class="nav navbar-nav">
        <%--space for home,forward back buttons --%>
      <div style="float:left;width:30px"class="active"><a href="/"><i class="fa fa-home fa-fw" aria-hidden="true"></i></a></div>
      <div style="float:left;width:30px"><a> <i class="fa fa-arrow-left" aria-hidden="true"onclick="JavaScript:window.history.back(1);return false;"></i></a></div>
      <div style="float:left;width:30px"><a> <i class="fa fa-arrow-right" aria-hidden="true"onclick="JavaScript:window.history.forward(1);return false;"></i></a></div>
      <div style="float:left;width:30px" runat="server" id="refreshButton"></div>
      <div style="color:ivory;float:left" runat="server" id="currentFolderName"></div>
    </div>
  </div>
</nav>
        <div class="row1">
            <div class="col-sm-3" style="background-color:#D3D3D3;">
                <%--Path for server address --%>
                <div class="left" style=""width: 240px; vertical-align:top; float: left">
                    <asp:Panel ID="Panel1" runat="server"></asp:Panel>
                </div>
                </div>
            </div>
        <div class="row3">
            <div class="col-sm-5">
                 <div style="margin-left:auto">
                    <%--Displays the taget location and files --%>
                <div class="middle" id="TargetArea" runat="server" >
                </div>
            </div>
            </div>
        </div>

        <div class="row3">
            <div class="col-sm-4" style="background-color:white;">
                <div class="right">
                    <%--will show favorites, most/recently viewed files --%>
                    <asp:Panel ID="Panel4" runat="server" style="color:#868787;">
                        <h4><u>Favorites</u></h4>
                        <div id="favoritesArea" runat="server">
                        </div>
                        <br />
                        <h4><u>Recently viewed</u></h4>
                        <div id="recentlyViewedArea" runat="server">
                        </div>
                        <br />
                        <h4><u>Most Often Viewed</u></h4>
                        <div id="mostOftenViewedArea" runat="server">
                        </div>
                        </asp:Panel>
                </div>
            </div>
        </div>
        <%--file upload --%>
        <br />
        <asp:FileUpload ID="acsfafileupload" runat="server" autopostback="true" />
        <input type="submit" id="Submit1" value="Upload" runat="server" />
        <asp:Panel ID="Panel3" runat="server">

        </asp:Panel>
    </form>
    <br />
    <footer class="site-footer">
        <p>&copy; AdvantageCS</p>
    </footer>
</body>
</html>