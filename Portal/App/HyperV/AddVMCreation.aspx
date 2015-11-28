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
                <asp:TextBox ID="txtVmname" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Virtual Machine Path:</span>
                <asp:TextBox ID="txtVmpath" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Physical Switch Adapter:</span>
                <asp:TextBox ID="txtPhysicaladapter" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Switch Name:</span>
                <asp:TextBox ID="txtSwitchName" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Maximum Memory:</span>
                <asp:TextBox ID="txtMaxmem" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">Minimum Memory:</span>
                <asp:TextBox ID="txtMinmem" ClientIDMode="Static" runat="server"></asp:TextBox>
            </li>
            <li><span class="label">OS ISO Path:</span>
                <asp:TextBox ID="txtIsopath" ClientIDMode="Static" runat="server"></asp:TextBox>
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