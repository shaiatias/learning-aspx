<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <style>
        input[type="button"] {
            width: 50px;
            height: 50px;
            border: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>

    <script>

        function onButtonClick(x, y) {

            let result = isNeighborEmpty(x, y);

            if (result) {
                replaceCells(x, y, result.x, result.y);
            }

            if (didFinish()) {

                let restart = confirm("game finished! yay!, click ok to start again.");

                if (restart) {
                    window.location.reload();
                }
            }
        }

        function isNeighborEmpty(x, y) {

            let xs = [x];
            let ys = [y];

            if ((x + 1) % 5 !== 0) {
                xs.push(x + 1);
            }

            if ((x - 1) % 5 !== 0) {
                xs.push(x - 1);
            }

            if ((y + 1) % 5 !== 0) {
                ys.push(y + 1);
            }

            if ((y - 1) % 5 !== 0) {
                ys.push(y - 1);
            }

            let result = null;

            for (var index = 0; index < xs.length; index++) {
                var x2 = xs[index];

                for (var index2 = 0; index2 < ys.length; index2++) {
                    var y2 = ys[index2];

                    var dist = Math.abs(x - x2) + Math.abs(y - y2);

                    if (dist === 1) {

                        let cell = document.querySelectorAll(`[data-tag='${x2},${y2}']`)[0];

                        if (cell.value.trim() === "16") {
                            result = { x: x2, y: y2 };
                        }
                    }
                }
            }

            return result;
        }

        function replaceCells(fromx, fromy, tox, toy) {

            let from = document.querySelectorAll(`[data-tag='${fromx},${fromy}']`)[0];
            let to = document.querySelectorAll(`[data-tag='${tox},${toy}']`)[0];

            let temp = from.value,
                tempColor = from.style.backgroundColor,
                tempDisplay = from.style.display;

            from.value = to.value;
            from.style.backgroundColor = to.style.backgroundColor;
            from.style.display = to.style.display;

            to.value = temp;
            to.style.backgroundColor = tempColor;
            to.style.display = tempDisplay;
        }

        function didFinish() {

            let cells = document.querySelectorAll(`[data-tag]`);

            for (var index = 0; index < cells.length; index++) {
                var cell = cells[index];

                if (cell.getAttribute("data-expected") !== cell.value) {
                    return false;
                }
            }

            return true;
        }

    </script>
</body>
</html>
