<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddApppublish.aspx.cs" Inherits="Portal.App.Remotedesktopservices.AddApppublish" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Remotedesktopservices/Apppublishs.aspx") %>"> &lt; Application Publishing Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Publish RDS Applications"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Application Alias Name:</span>
                <asp:TextBox ID="txtAlias" ClientIDMode="Static" runat="server" MaxLength="15"></asp:TextBox>
            </li>
            <li><span class="label">Application Displayname:</span>
                <asp:TextBox ID="txtDisplayname" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
            <li><span class="label">App Executable path:</span>
                <asp:TextBox ID="txtFilepath" ClientIDMode="Static" runat="server" MaxLength="100"></asp:TextBox>
            </li>
            <li><span class="label">Collection Name:</span>
                <asp:TextBox ID="txtcollectionname" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
            <li><span class="label">RDS Connection Broker:</span>
                <asp:TextBox ID="txtConnectionbroker" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
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