<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddVMCreation.aspx.cs" Inherits="Portal.App.HyperV.AddVMCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/HyperV/VMCreations.aspx") %>"> &lt; VM Creation Dashboard</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Add Virtual Switch Creations"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Virtual Machine Name:</span>
                <asp:TextBox ID="txtVmname" ClientIDMode="Static" runat="server" MaxLength="25"></asp:TextBox>
            </li>
            <li><span class="label">Virtual Machine Path:</span>
                <asp:TextBox ID="txtVmpath" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
                <asp:Label ID="Label4" runat="server" Text="Label">Eg: "C:\VMFolder\"</asp:Label>
            </li>
            <li><span class="label">Physical Switch Adapter:</span>
                <asp:TextBox ID="txtPhysicaladapter" ClientIDMode="Static" runat="server" MaxLength="25"></asp:TextBox>
            </li>
            <li><span class="label">Switch Name:</span>
                <asp:TextBox ID="txtSwitchName" ClientIDMode="Static" runat="server" MaxLength="25"></asp:TextBox>
                <asp:Label ID="Label5" runat="server" Text="Label">Existing Switch name should be specified</asp:Label>
            </li>
            <li><span class="label">Maximum Memory:</span>
                <asp:TextBox ID="txtMaxmem" ClientIDMode="Static" runat="server" MaxLength="4"></asp:TextBox>
                <asp:Label ID="Label1" runat="server" Text="Label">Specify memory in GigaBytes. </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" ControlToValidate="txtMaxmem" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            </li>
            <li><span class="label">Minimum Memory:</span>
                <asp:TextBox ID="txtMinmem" ClientIDMode="Static" runat="server" MaxLength="4"></asp:TextBox>
                <asp:Label ID="Label2" runat="server" Text="Label">Specify memory in GigaBytes. </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" ControlToValidate="txtMinmem" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            </li>
            <li><span class="label">Harddisk Size:</span>
                <asp:TextBox ID="Txthddsize" ClientIDMode="Static" runat="server" MaxLength="4"></asp:TextBox>
                <asp:Label ID="Label6" runat="server" Text="Label">Specify memory in GigaBytes. </asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" ControlToValidate="txtMinmem" runat="server" ErrorMessage="Only Numbers allowed" ValidationExpression="\d+"></asp:RegularExpressionValidator>
            </li>
            <li><span class="label">OS ISO Path:</span>
                <asp:TextBox ID="txtIsopath" ClientIDMode="Static" runat="server" MaxLength="90"></asp:TextBox>
                <asp:Label ID="Label3" runat="server" Text="Label">Eg: "C:\Test\testimage.iso"</asp:Label>
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