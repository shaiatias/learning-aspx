<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication2.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <style>
        input[type="button"] {
            width: 50px;
            height: 50px;
            border: 0;
            transition: left 0.2s ease-in-out, top 0.2s ease-in-out;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">

    <asp:ScriptManager ID="smMain" runat="server" EnablePageMethods="true" />
    <script type="text/javascript">

        function updateTextAndColor(list) {

            let elements = Array.from(document.querySelectorAll("[data-tag]"));

            for (var i = 0; i < list.length; i++) {
                elements[i].value = list[i].Text;
                elements[i].style.backgroundColor = list[i].Color;
                elements[i].style.color = (list[i].Color === "transparent" ? "transparent" : "black");
            }
        }

        function getAllButtons() {

            let items = document.querySelectorAll("[data-tag]");

            return Array.from(items).map(i => {
                return { Text: i.value, Color: i.style.backgroundColor };
            });
        }

        function OnButtonClick(x, y) {

            let success = (list) => {
                console.log("success", list);

                updateTextAndColor(list);

                DidFinished();
            };

            let error = (err) => {
                console.error("error", err);
            };

            PageMethods.OnButtonClick(getAllButtons(), x, y, success, error);
        }

        function StartNewGame() {

            let success = (list) => {
                console.log("success", list);

                updateTextAndColor(list);
            };

            let error = (err) => {
                console.error("error", arguments, err);
            };

            PageMethods.StartNewGame(success, error);
        }

        function DidFinished() {

            let success = (result) => {
                console.log("success", arguments, result);

                if (result) {

                    let response = confirm("Game is finished, restart?");

                    if (response) {
                        StartNewGame();
                    }
                }
            };

            let error = (err) => {
                console.error("error", err);
            };

            PageMethods.DidFinished(getAllButtons(), success, error);
        }

        window.addEventListener('load', (e) => {
            StartNewGame();
        });

    </script>

    </form>

</body>
</html>
