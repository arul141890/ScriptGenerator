<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddSmbsharecreation.aspx.cs" Inherits="Portal.App.Filestorage.AddSmbsharecreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/Filestorage/Smbsharecreations.aspx") %>"> &lt; SMB Share Creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="SMB Share Creation"></asp:Label>
        </p>
        <ul>
            <li><span class="label">SMB Directory:</span>
                <asp:TextBox ID="txtDirectoryname" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
            </li>
            <li><span class="label">SMB Share Name:</span>
                <asp:TextBox ID="txtSmbname" ClientIDMode="Static" runat="server" MaxLength="20"></asp:TextBox>
            </li>
             
            <li><span class="label">Specify Accessgroups:</span>
                <asp:TextBox ID="txtAccessgroups" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
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