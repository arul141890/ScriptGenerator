﻿<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="AddVirtualSwitchCreation.aspx.cs" Inherits="Portal.App.HyperV.AddVirtualSwitchCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <a class="back" href="<%= this.ResolveClientUrl("~/App/HyperV/VirtualSwitchCreations.aspx") %>"> &lt; Back to VirtualSwitchCreations</a>
    </div>

    <div class="s-form-container">
        <p class="title">
            <asp:Label ID="lblTitle" runat="server" Text="Add Virtual Switch Creations"></asp:Label>
        </p>
        <ul>
            <li><span class="label">Switch Name:</span>
                <asp:TextBox ID="txtSwitchName" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
            <li><span class="label">Physical Adapter:</span>
                <asp:TextBox ID="txtAdapter" ClientIDMode="Static" runat="server" MaxLength="30"></asp:TextBox>
            </li>
           <li><span class="label">Allow Management OS:</span>
                <asp:DropDownList ID="DDAllowManagementOs" runat="server">
                     <asp:ListItem Selected="True">--SELECT--</asp:ListItem>
                    <asp:ListItem>False</asp:ListItem>
                    <asp:ListItem>True</asp:ListItem>
                </asp:DropDownList>
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