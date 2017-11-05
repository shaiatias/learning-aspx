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
            transition: left 0.2s ease-in-out, top 0.2s ease-in-out;
        }
    </style>
</head>
<body>

    <form id="form1" runat="server">
    </form>

    <script>

        var isAnimating = false;

        function onButtonClick(x, y) {

            if (isAnimating) {
                console.log("ignoring while animating");
                return;
            }

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

            isAnimating = true;

            let from = document.querySelectorAll(`[data-tag='${fromx},${fromy}']`)[0];
            let to = document.querySelectorAll(`[data-tag='${tox},${toy}']`)[0];

            let fromTop =   from.style.top,
                fromLeft =  from.style.left,
                toTop =     to.style.top,
                toLeft =    to.style.left;

            from.style.top = to.style.top;
            from.style.left = to.style.left;

            to.style.top = fromTop;
            to.style.left = fromLeft;


            let tempExpected = from.getAttribute("data-expected"),
                tempTag = from.getAttribute("data-tag");

            from.setAttribute("data-expected", to.getAttribute("data-expected"));
            from.setAttribute("data-tag", to.getAttribute("data-tag"));

            to.setAttribute("data-expected", tempExpected);
            to.setAttribute("data-tag", tempTag);
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

        window.addEventListener('transitionend', e => {
            isAnimating = false;
        });

    </script>
</body>
</html>
