<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddRdsinstallation.aspx.cs" Inherits="Portal.App.Remotedesktopservices.AddRdsinstallation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Remotedesktopservices/Rdsinstallations.aspx") %>"> &lt; RDS installation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Install RDS Servers"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Connection Broker FQDN:</span>
                <asp:TextBox ID="txtConnectionbroker" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Web Access Server FQDN:</span>
                <asp:TextBox ID="txtWebaccessserver" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Session Host FQDN:</span>
                <asp:TextBox ID="txtSessionhost" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
                <li><span class="label">Gateway Server FQDN:</span>
                <asp:TextBox ID="txtGatewayserver" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Website FQDN:</span>
                <asp:TextBox ID="txtGatewayfqdn" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
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