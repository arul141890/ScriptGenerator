<%@ Page Language="C#" AutoEventWireup="true"  MasterPageFile="~/Site.Master" CodeBehind="Scopecreations.aspx.cs" Inherits="Portal.App.Dhcp.Scopecreations" %>

<%@ Register TagPrefix="asp" Namespace="Portal.App.WebControls" Assembly="Portal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
      
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="s-breadcrumb">
        <ul>
            
            <li>
                <a class="command" href="<%= this.ResolveClientUrl("~/App/Dhcp/AddScopecreation.aspx") %>">Create New DHCP Scope</a>
            </li>
        </ul>
    </div>
    <div>
        <asp:GridViewExFilterPanel runat="server" ID="pnlFilter" GridViewExID="gvData">
            <p class="title"><span>Filter</span></p>

        </asp:GridViewExFilterPanel>

        <div class="s-form-container">
            <ul>
                <li><span>Total no. of rows returned:&nbsp;</span>
                    <asp:Label ID="lblCount" runat="server"></asp:Label>
                    <%--<asp:Button ID="btnExport" runat="server" CssClass="command" OnClick="btnExport_Click" Text="Export" />--%>
                </li>
            </ul>
        </div>
        <div class="s-gridview-container">
            <asp:GridViewEx runat="server" ID="gvData" DataSourceID="dsData" />
        </div>

        <asp:ObjectDataSourceEx runat="server" ID="dsData">
        </asp:ObjectDataSourceEx>
    </div>
</asp:Content>
