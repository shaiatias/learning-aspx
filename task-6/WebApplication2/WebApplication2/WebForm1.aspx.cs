using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication2
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        static Random random = new Random();

        protected void Page_Load(object sender, EventArgs e)
        {
            buildTable();
        }

        private void buildTable()
        {
            Table table = new Table();

            int counter = 1;

            for (int i = 1; i <= 4; i++)
            {
                TableRow row = new TableRow();

                for (int j = 1; j <= 4; j++)
                {
                    var currentIndex = (i - 1) * 4 + j;

                    TableCell cell = new TableCell();
                    Button button = new Button();
                    button.UseSubmitBehavior = false;

                    button.Text = currentIndex.ToString();

                    button.Attributes.Add("data-tag", i + "," + j);
                    button.Attributes.Add("data-expected", currentIndex.ToString());

                    button.OnClientClick = "OnButtonClick(" + (i - 1) + "," + (j - 1) + "); return false;";

                    button.BackColor = currentIndex == 16 ? Color.Transparent : getRandomColor();

                    button.Style["top"] = ((i - 1) * 55) + "px";
                    button.Style["left"] = ((j - 1) * 55) + "px";
                    button.Style["color"] = currentIndex == 16 ? "transparent" : "black";
                    button.Style["position"] = "absolute";

                    if (counter == 16)
                    {
                        button.Text = "";
                    }
                    else
                    {
                        button.Text = counter.ToString();
                    }

                    cell.Controls.Add(button);
                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }

            form1.Controls.Add(table);
        }

        private static List<int> getRandomizedList()
        {
            List<int> original = new List<int>(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 });
            List<int> randomized = new List<int>();

            while (original.Count != 0)
            {
                var index = random.Next(0, original.Count);
                var item = original[index];
                randomized.Add(item);
                original.RemoveAt(index);
            }

            return randomized;
        }

        private static Color getRandomColor()
        {
            return Color.FromArgb(
                random.Next(100, 255), // r
                random.Next(100, 255), // g
                random.Next(100, 255)  // b
            );
        }

        private static String RGBConverter(Color c)
        {
            return "rgb(" + c.R.ToString() + "," + c.G.ToString() + "," + c.B.ToString() + ")";
        }

        [System.Web.Services.WebMethod]
        public static List<TextAndColor> StartNewGame()
        {
            var list = getRandomizedList().Select(
                (num) => new TextAndColor(
                    num.ToString(),
                    num == 16 ? "transparent" : RGBConverter(getRandomColor())
                ));
            
            return list.ToList();
        }

        [System.Web.Services.WebMethod]
        public static bool DidFinished(List<TextAndColor> list)
        {
            for (int i = 0; i <= list.Count; i++)
            {
                if (i+1 != int.Parse(list[i].Text))
                {
                    return false;
                }
            }

            return true;
        }

        [System.Web.Services.WebMethod]
        public static TextAndColor[] OnButtonClick(TextAndColor[] list, int x, int y)
        {
            var possibleNeighbors = new List<int[]> {
                new int[2] { x - 1, y },
                new int[2] { x, y - 1 },
                new int[2] { x, y + 1 },
                new int[2] { x + 1, y }
            };

            possibleNeighbors = possibleNeighbors.FindAll(a => {

                var x2 = a[0];
                var y2 = a[1];

                if (!(0 <= x2 && x2 <= 3))
                {
                    return false;
                }

                if (!(0 <= y2 && y2 <= 3))
                {
                    return false;
                }

                return true;                
            });

            var emptyNeighbor = possibleNeighbors.Find(a =>
            {
                var x2 = a[0];
                var y2 = a[1];
                return (list[(x2 * 4) + y2].Text == "16");
            });

            if (emptyNeighbor == null) {
                return list;
            }

            var myIndex = ((x * 4) + y);
            var neighborIndex = ((emptyNeighbor[0] * 4) + emptyNeighbor[1]);

            var temp = list[myIndex];
            list[myIndex] = list[neighborIndex];
            list[neighborIndex] = temp;

            return list;
        }
    }

    [Serializable]
    public class TextAndColor
    {
        public string Text;
        public string Color;

        public TextAndColor()
        {
        }

        public TextAndColor(string text, string color)
        {
            Text = text;
            Color = color;
        }
    }
}