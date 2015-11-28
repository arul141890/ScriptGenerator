<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddNamespacecreation.aspx.cs" Inherits="Portal.App.Filestorage.AddNamespacecreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Filestorage/Namespacecreatios.aspx") %>"> &lt; DFS Namespace Creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Create New DFS Namespace"></asp:Label>
        </p>
        <ul>
            <li><span class="label">DFS Server Name:</span>
                <asp:TextBox ID="txtDfsservername" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">DFS Path:</span>
                <asp:TextBox ID="txtDfspath" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">File Server Name:</span>
                <asp:TextBox ID="txtFileservername" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Namespace Target Path:</span>
                <asp:TextBox ID="txtTargetpath" ClientIDMode="Static" runat="server"></asp:TextBox>
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