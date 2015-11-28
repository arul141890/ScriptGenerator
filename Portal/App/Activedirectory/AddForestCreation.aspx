<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddForestCreation.aspx.cs" Inherits="Portal.App.Activedirectory.AddForestCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Activedirectory/ForestCreation.aspx") %>"> &lt; Forest Creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Create New Active Directory Forest"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Hostname:</span>
                <asp:TextBox ID="Txthostname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
           <li><span class="label">IP Address:</span>
                <asp:TextBox ID="txtipaddress" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
                        
            <li><span class="label">DatabasePath:</span>
                <asp:TextBox ID="txtdbpath" ClientIDMode="Static" runat="server">C:\Windows\NTDS</asp:TextBox>
            </li>
            <li><span class="label">DomainMode:</span>
                <asp:DropDownList ID="DDdomainmode" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>Win2012R2</asp:ListItem>
                    <asp:ListItem>Win2012</asp:ListItem>
                    <asp:ListItem> Win2008R2</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">DomainName:</span>
                <asp:TextBox ID="txtdomainname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
           <li><span class="label">DomainNetbiosName:</span>
                <asp:TextBox ID="txtnetbios" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">ForestMode:</span>
                <asp:DropDownList ID="DDforestmode" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>Win2012R2</asp:ListItem>
                    <asp:ListItem>Win2012</asp:ListItem>
                    <asp:ListItem> Win2008R2</asp:ListItem>
                </asp:DropDownList>
            </li>
           
            <li><span class="label">LogPath:</span>
                <asp:TextBox ID="txtlogpath" ClientIDMode="Static" runat="server">C:\Windows\NTDS</asp:TextBox>
            </li>
            <li><span class="label">Reboot on completion:</span>
                <asp:DropDownList ID="DDreboot" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">SysvolPath:</span>
                <asp:TextBox ID="txtsysvol" ClientIDMode="Static" runat="server">C:\Windows\SYSVOL</asp:TextBox>
            </li>
            <li><span class="label">safemodeadministratorpassword:</span>
                <asp:TextBox runat="server" ID="txtsafemodepwd" TextMode="Password" />
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