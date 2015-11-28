<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddDhcpinstallation.aspx.cs" Inherits="Portal.App.Dhcp.AddDhcpinstallation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Dhcp/Dhcpinstallations.aspx") %>"> &lt; DHCP Installation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Install New DHCP Server"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Is Static IP:</span>
                <asp:DropDownList ID="DDStaticIP" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">Is Domain Joined:</span>
                <asp:DropDownList ID="DDDomainjoined" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                </asp:DropDownList>
            </li>
               <li><span class="label">Authorize this DHCP:</span>
                <asp:DropDownList ID="DDAuthorize" runat="server">
                    <asp:ListItem>--SELECT--</asp:ListItem>
                    <asp:ListItem Selected="True">True</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">Host Name:</span>
                <asp:TextBox ID="txtHostname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">IP Address:</span>
                <asp:TextBox ID="txtIpaddress" ClientIDMode="Static" runat="server">C:\Windows\NTDS</asp:TextBox>
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