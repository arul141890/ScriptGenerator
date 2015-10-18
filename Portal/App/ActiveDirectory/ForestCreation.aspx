<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master"  CodeBehind="ForestCreation.aspx.cs" Inherits="Portal.App.ActiveDirectory.ForestCreation" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#mnuActive").addClass("active");
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <ul class="breadcrumb">
            <li>Category<span class="divider">/</span></li>
            <li class="active">Top Charts</li>
        </ul>
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" style="text-decoration: none" data-toggle="collapse"
                    data-parent="#accordionFilter" href="#collapseOne">Add New </a>
            </div>
            <div id="collapseOne" class="accordion-body collapse in">
                <div class="accordion-inner">
                    <div class="well">
                        <div class="row-fluid">
                            <span class="span2">Language</span>
                            <div class="span5">
                                <asp:DropDownList ID="ddlLanguage" runat="server" AutoPostBack="True" OnSelectedIndexChanged="OnSeletedLanguage" ValidationGroup="Create">
                                    <Items>
                                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                                    </Items>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please select language" ControlToValidate="ddlLanguage" InitialValue="-1" />
                            </div>
                        </div>
                        <div class="row-fluid">
                            <span class="span2">Song Name</span>
                            <div class="span5">
                                <asp:DropDownList ID="ddlSongId" runat="server" ValidationGroup="Create">
                                    <Items>
                                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                                    </Items>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please select language" ControlToValidate="ddlSongId" InitialValue="-1" />
                            </div>
                        </div>
                        <div class="row-fluid">
                            <span class="span2">Priority</span>
                            <div class="span5">
                                <asp:TextBox ID="txtPriority" runat="server" ValidationGroup="Create"></asp:TextBox>
                            </div>
                        </div>
                        <div class="row-fluid">
                            <div class="span2">
                                <asp:Button ID="btnAdd" Text="Add" runat="server" CssClass="btn btn-primary" OnClick="OnCreate" ValidationGroup="Create" />
                            </div>
                            <div class="span2">
                                <asp:Label runat="server" ID="lblErrorMessage" ForeColor="Red" Visible="False"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="accordion-group">
            <div class="accordion-heading">
                <a class="accordion-toggle" style="text-decoration: none" data-toggle="collapse"
                    data-parent="#accordionFilter" href="#collapseTwo">Filters </a>
            </div>
            <div id="collapseTwo" class="accordion-body collapse in">
                <div class="accordion-inner">
                    <div class="well">
                        <div class="row-fluid">
                            <span class="span2">Language</span>
                            <div class="span3">
                                <asp:DropDownList ID="ddlLanguageFilter" runat="server" ValidationGroup="Filter">
                                    <Items>
                                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                                    </Items>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please select language" ControlToValidate="ddlLanguageFilter" InitialValue="-1" />
                            </div>
                        </div>
                        <div class="row-fluid">
                            <div class="span2">
                                <asp:Button ID="btnFilter" Text="Filter" runat="server" CssClass="btn btn-primary" OnClick="OnFilter" ValidationGroup="Filter" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row-fluid">
            <div class="span2">
                <asp:Button ID="BtnDelete" Text="Delete Selected" runat="server" CssClass="btn btn-primary" OnClick="OnDeleteAll" ValidationGroup="Delete" />
            </div>
        </div>
      
    </div>
</asp:Content>
