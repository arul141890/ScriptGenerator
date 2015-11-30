<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddCollectioncreation.aspx.cs" Inherits="Portal.App.Remotedesktopservices.AddCollectioncreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Remotedesktopservices/Collectioncreations.aspx") %>"> &lt; RDS Collection creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Create RDS Collections"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Collection Name:</span>
                <asp:TextBox ID="txtCollectionname" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
             <li><span class="label">Collection Description:</span>
                <asp:TextBox ID="txtCollectiondescription" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Session host server:</span>
                <asp:TextBox ID="txtSessionhost" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Connection Broker Server:</span>
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