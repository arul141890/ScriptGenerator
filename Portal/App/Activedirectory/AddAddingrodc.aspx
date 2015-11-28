<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddAddingrodc.aspx.cs" Inherits="Portal.App.Activedirectory.AddAddingrodc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Activedirectory/Addingrodcs.aspx") %>"> &lt; RODC Installation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Install RODC"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Hostname:</span>
                <asp:TextBox ID="txtHostname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">IP Address:</span>
                <asp:TextBox ID="txtIpaddress" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
             <li><span class="label">Only Critical Replication:</span>
                <asp:DropDownList ID="DDCriticalreplication" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">Allow Password Replication AccountName:</span>
                <asp:TextBox ID="txtAllowpraccount" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Deny Password Replication AccountName:</span>
                <asp:TextBox ID="txtdenypraccount" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Delegated Administrator Accountname:</span>
                <asp:TextBox ID="txtDelegatedacc" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Domain Name:</span>
                <asp:TextBox ID="txtdomainname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Install DNS:</span>
                <asp:DropDownList ID="DDInstallDNS" runat="server">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">Site Name:</span>
                <asp:TextBox ID="txtSitename" ClientIDMode="Static" runat="server"></asp:TextBox>
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