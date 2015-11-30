<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddWebsitecreation.aspx.cs" Inherits="Portal.App.Webserver.AddWebsitecreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Webserver/Websitecreations.aspx") %>"> &lt; Website Creation Dashboard </a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Create New Website"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Application pool Name:</span>
                <asp:TextBox ID="txtapppool" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
            <li><span class="label">Website Name:</span>
                <asp:TextBox ID="txtwebsite" ClientIDMode="Static" runat="server" MaxLength="25"></asp:TextBox>
            </li>
            <li><span class="label">Port Number:</span>
                <asp:TextBox ID="txtportnumber" ClientIDMode="Static" runat="server" MaxLength="6"></asp:TextBox>
                
            </li>
            
            <li><span class="label">Physical Path:</span>
                <asp:TextBox ID="Txtphysicalpath" ClientIDMode="Static" runat="server" MaxLength="100"></asp:TextBox>
            </li>
        </ul>
        <p class="error">
            <asp:Label runat="server" ID="lblErrorMessage" Visible="False"></asp:Label>
        </p>

        <p class="success">
            <asp:Label runat="server" ID="lblSuccessMessage" Visible="False"></asp:Label>
        </p>

        <hr />
        <p> 
            <asp:HiddenField  runat="server" ID="hdnfileName" />
            <asp:LinkButton runat="server" visible="false" ID="lbdownload" Text="Please Click Here to Download file" onclick="lbdownload_Click"></asp:LinkButton>
            
        </p>
        <p class="command">
            <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="ButtonClick" />

        </p>
    </div>
</asp:Content>