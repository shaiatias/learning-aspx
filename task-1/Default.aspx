<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" EnableSessionState="True" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        input[type="submit"] {
            width: 50px;
            height: 50px;
            border: 0;
        }
    </style>
</head>
<body>


    <form id="form1" runat="server">
        <div id="container" runat="server">
            <asp:Button Visible="false" ID="Button1" runat="server" OnClick="Restart_Click" Text="Button" />
        </div>
    </form>
</body>
</html>
