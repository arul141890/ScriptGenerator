<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddDnsrecordcreation.aspx.cs" Inherits="Portal.App.Dns.AddDnsrecordcreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Dns/Dnsrecordcreations.aspx") %>"> &lt; DNS Record Creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Create DNS Record"></asp:Label>
        </p>
        <ul>
            <li><span class="label">DNS Server Name:</span>
                <asp:TextBox ID="txtHostname" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">IP Address:</span>
                <asp:TextBox ID="txtIpaddress" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
            <li><span class="label">Zone Name:</span>
                <asp:TextBox ID="txtZonename" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
             <li><span class="label">CSV File Name:</span>
                <asp:TextBox ID="txtCsvfilename" ClientIDMode="Static" runat="server" MaxLength="100"></asp:TextBox>
                 <asp:Label ID="Label1" runat="server" Text="Label">CSV Columns{HostName,IPAddress}</asp:Label>
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