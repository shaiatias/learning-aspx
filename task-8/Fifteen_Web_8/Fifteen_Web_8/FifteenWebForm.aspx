<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FifteenWebForm.aspx.cs" Inherits="FifteenWebForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<script language="javascript">
    function startNewGame() {
        try {
            xmlHttpRequest = new ActiveXObject("Microsoft.XMLHTTP");
        }
        catch (e) {
            try {
                xmlHttpRequest = new XMLHttpRequest();
            }
            catch (e) {
            }
        }

        document.body.style.background = "rgb(255,255,255)"

        size = parseInt(document.getElementById("hiddenSize").value);
        numbers = size * size;
        buttons = document.getElementsByClassName("buttons");
        url = "http://localhost:2108/Handler.ashx";

        var request = url + "?numbers=" + (numbers - 1).toString() + "&cmd=Shuffle";

        xmlHttpRequest.open("GET", request, true);
        xmlHttpRequest.onreadystatechange = onGetRandomTextAndColors;
        xmlHttpRequest.send();
    }

    function onGetRandomTextAndColors() {
        if (xmlHttpRequest.readyState == 4) {
            var response = xmlHttpRequest.responseText;
            var numbersAndColors = JSON.parse(response);

            for (var i = 0; i < numbers - 1; i++) {
                buttons[i].value = numbersAndColors[i].text;
                buttons[i].style.background = numbersAndColors[i].color;
                buttons[i].style.visibility = "visible";
            }

            emptyRow = size - 1;
            emptyColumn = size - 1;
            buttons[numbers - 1].value = "empty";
            buttons[numbers - 1].style.visibility = "hidden";
        }
    }

    function allButtonsClick(button) {
        clickedButton = button;
        buttonRow = Math.floor(button.name / size);
        buttonColumn = Math.floor(button.name % size);
        buttonSize = parseInt(button.style.width, 10);

        var request = url + "?buttonRow=" + buttonRow + "&buttonColumn=" + buttonColumn + "&emptyRow=" + emptyRow + "&emptyColumn=" + emptyColumn + "&cmd=GetDirection";

        xmlHttpRequest.open("GET", request, true);
        xmlHttpRequest.onreadystatechange = onGetDirection;
        xmlHttpRequest.send();
    }

    function onGetDirection() {
        if (xmlHttpRequest.readyState == 4) {
            var response = xmlHttpRequest.responseText;
            var direction = JSON.parse(response);

            var x, y;

            switch (direction) {
                case "Left":
                    x = -1;
                    y = 0;
                    break;
                case "Right":
                    x = 1;
                    y = 0;
                    break;
                case "Down":
                    x = 0;
                    y = 1;
                    break;
                case "Up":
                    x = 0;
                    y = -1;
                    break;
                default:
                    x = 0;
                    y = 0;
                    break;
            }

            clickedButton.name = emptyRow * size + emptyColumn;
            emptyRow = parseInt(buttonRow);
            emptyColumn = parseInt(buttonColumn);

            for (var i = buttonSize; i > 0; i--) {
                setTimeout(function () {
                    clickedButton.style.left = parseInt(clickedButton.style.left) + x + "px";
                    clickedButton.style.top = parseInt(clickedButton.style.top) + y + "px";
                    buttonSize--;
                }, i * 10);
            }

            var buttonColor = new Object();
            buttonColor.color = clickedButton.style.background;
            var buttonColorString = JSON.stringify(buttonColor);
            var bodyColor = new Object();
            bodyColor.color = document.body.style.background;
            var bodyColorString = JSON.stringify(bodyColor);

            var request = url + "?first=" + buttonColorString + "&second=" + bodyColorString + "&cmd=GetAverageColor";

            xmlHttpRequest.open("GET", request, true);
            xmlHttpRequest.onreadystatechange = onGetAverageColor;
            xmlHttpRequest.send();
        }
    }

    function onGetAverageColor() {
        if (xmlHttpRequest.readyState == 4) {
            var response = xmlHttpRequest.responseText;
            var avgColor = JSON.parse(response);

            document.body.style.background = avgColor.color;

            textAndColorArray = new Array();

            for (var i = 0; i < numbers - 1; i++) {
                textAndColorArray[i] = new Object();
                textAndColorArray[i].text = buttons[i].value;
                textAndColorArray[i].color = buttons[i].name;
            }

            var textAndColorArrayString = JSON.stringify(textAndColorArray);

            var request = url + "?array=" + textAndColorArrayString + "&cmd=CheckGameOver";

            xmlHttpRequest.open("GET", request, true);
            xmlHttpRequest.onreadystatechange = onCheckGameOver;
            xmlHttpRequest.send();
        }
    }

    function onCheckGameOver() {
        if (xmlHttpRequest.readyState == 4) {
            var response = xmlHttpRequest.responseText;
            var isGameOver = JSON.parse(response);

            if (isGameOver) {
                if (confirm("Game Over! New Game?")) {
                    location.reload();
                }
            }
        }
    }
</script>
<body onload="startNewGame();">
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
