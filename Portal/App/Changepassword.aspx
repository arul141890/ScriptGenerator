<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Changepassword.aspx.cs" Inherits="Portal.Register" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
   <head id="Head1" runat="server">
        <title>Script Generator</title>

        <style type="text/css">
            .loginbox {
                background-color: whitesmoke;
                border: 2px solid #45AEEA;
                font-family: Arial, Helvetica, sans-serif;
                font-size: small;
                font-weight: bold;
                margin: 150px auto;
                width: 400px;
            }



            .loginbox .header {
                background-color: #45AEEA;
                border-bottom: 2px solid #45AEEA;
                height: 22px;
                padding-top: 5px;
            }

            .loginbox .header span {
                color: white;
                padding: 10px;
                text-align: center;
            }

            .loginbox .content {
                margin: 0px auto;
                width: 380px;
            }

            .loginbox .content ul { list-style: none; }

            .loginbox .content ul li {
                display: inline-block;
                padding: 5px;
                width: 120px;
            }

            .error {
                background-color: whitesmoke;
                border-top: 2px dotted #006666;
                height: 22px;
                padding-top: 4px;
            }

            .error span {
                color: red;
                font-size: small;
                padding-left: 60px;
            }
            #form1 {
                margin-left: auto;
                margin-right: auto;
            }
        </style>

    </head>
    <body style="height: auto">

        <form id="form1" runat="server">
                    
            <div>
                <div class="loginbox">
                    <div class="header">
                        <span>change Password</span>
                    </div>
                    <div class="content">
                        <ul>
                            
                            <li>Current Password</li>
                            <li>
                                <asp:TextBox runat="server" ID="txtOldPassword" TextMode="Password" />
                            </li>
                            <li>New Password</li>
                            <li>
                                <asp:TextBox runat="server" ID="txtNewPassword" TextMode="Password" />
                            </li>
                            <li></li>
                            <li>
                                <asp:Button Text="Submit" ID="BtnChangepwd" runat="server" OnClick="Btnchange" />
                            </li>
                        </ul>
                    </div>

                       <p class="error">
                       <asp:Label runat="server" ID="lblErrorMessage" Visible="False"></asp:Label>
                       </p>

                       <p class="success">
                       <asp:Label runat="server" ID="lblSuccessMessage" Visible="False"></asp:Label>
                       </p>

                    
                </div>
                </div>
         </form>
    </body>
</html>