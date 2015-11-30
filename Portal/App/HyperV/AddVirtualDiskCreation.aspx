<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddVirtualDiskCreation.aspx.cs" Inherits="Portal.App.HyperV.AddVirtualDiskCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/HyperV/VirtualDiskCreations.aspx") %>"> &lt; Virtual Disk Creation Daskboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Add Virtual Switch Creations"></asp:Label>
        </p>
        <ul>
            <li><span class="label">VHDPath:</span>
                <asp:TextBox ID="txtvhdpath" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
                <asp:Label ID="Label2" runat="server" Text="Label">Eg: "C:\test\testvhd.vhdx"</asp:Label>
            </li>
            <li><span class="label">Virtual Disk Size:</span>
                <asp:TextBox ID="txtVHDSize" ClientIDMode="Static" runat="server" MaxLength="10"></asp:TextBox>
                <asp:Label ID="Label3" runat="server" Text="Label">Values are in GigaBytes. </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtVHDSize" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            </li>
             <li><span class="label">Disk Type:</span>
                <asp:DropDownList ID="DDdisktype" runat="server" AutoPostBack="true" OnTextChanged="disktypeDD_SelectedIndexChanged">
                    <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>Fixed</asp:ListItem>
                    <asp:ListItem>Dynamic</asp:ListItem>
                    <asp:ListItem>Differencing</asp:ListItem>
                </asp:DropDownList>
            </li>
            <li><span class="label">Parent VHD Path:</span>
                <asp:TextBox ID="txtParentPath" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
                <asp:Label ID="Label1" runat="server" Text="Label">Eg: "C:\test\testvhd.vhdx"</asp:Label>
            </li>
            </ul>
        <ul>
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