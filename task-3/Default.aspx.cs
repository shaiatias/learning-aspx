using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    Random random = new Random();
    Table table = new Table();
    Dictionary<string, Button> tagToButton = new Dictionary<string, Button>();
    List<Button> buttonsList = new List<Button>();

    protected void Page_Load(object sender, EventArgs e)
    {
        buildTable();
        shuffleButtons();
    }

    private void shuffleButtons()
    {
        for (var index = 0; index < 20; index++)
        {
            var x = random.Next(1, 4);
            var y = random.Next(1, 4);

            var x2 = random.Next(1, 4);
            var y2 = random.Next(1, 4);

            replaceButtons(x, y, x2, y2);
        }
    }

    private void replaceButtons(int i, int j, int i2, int j2)
    {
        string temp = tagToButton[i + "," + j].Text;
        Color tempColor = tagToButton[i + "," + j].BackColor;
        string tempDisplay = tagToButton[i + "," + j].Style["display"];

        tagToButton[i + "," + j].Text = tagToButton[i2 + "," + j2].Text;
        tagToButton[i + "," + j].BackColor = tagToButton[i2 + "," + j2].BackColor;
        tagToButton[i + "," + j].Style["display"] = tagToButton[i2 + "," + j2].Style["display"];

        tagToButton[i2 + "," + j2].Text = temp;
        tagToButton[i2 + "," + j2].BackColor = tempColor;
        tagToButton[i2 + "," + j2].Style["display"] = tempDisplay;
    }

    private void buildTable()
    {
        int counter = 1;

        for (int i = 1; i <= 4; i++)
        {
            TableRow row = new TableRow();

            for (int j = 1; j <= 4; j++, counter++)
            {
                TableCell cell = new TableCell();
                Button button = new Button();
                button.UseSubmitBehavior = false;

                button.Text = counter.ToString();

                button.Attributes.Add("data-tag", i + "," + j);
                button.Attributes.Add("data-expected", counter.ToString());
                
                button.OnClientClick = "onButtonClick(" + i + ", " + j + "); return false;";

                button.BackColor = counter == 16 ? Color.Transparent : getRandomColor();
                button.Style["display"] = counter == 16 ? "none" : "block";

                tagToButton.Add(i + "," + j, button);

                cell.Controls.Add(button);
                buttonsList.Add(button);

                row.Cells.Add(cell);
            }

            table.Rows.Add(row);
        }

        form1.Controls.Add(table);
    }

    private Color getRandomColor()
    {
        return Color.FromArgb(
            random.Next(100, 255), // r
            random.Next(100, 255), // g
            random.Next(100, 255)  // b
        );
    }
}