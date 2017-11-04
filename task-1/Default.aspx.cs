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
        
        if (!this.IsPostBack)
        {
            startNewGame();
        }
    }

    private void startNewGame()
    {
        foreach (Button button in buttonsList)
        {
            button.BackColor = Color.FromArgb(10, random.Next(100, 250), random.Next(100, 250), random.Next(100, 250));
        }

        buttonsList[15].BackColor = Color.Transparent;

        for (var index = 0; index < 20; index++)
        {
            var x = random.Next(4) + 1;
            var y = random.Next(4) + 1;

            var x2 = random.Next(4) + 1;
            var y2 = random.Next(4) + 1;

            replaceButtons(x, y, x2, y2);
        }
    }

    private void buildTable()
    {
        int counter = 1;

        for (int i = 0; i < 4; i++)
        {
            TableRow row = new TableRow();

            for (int j = 0; j < 4; j++, counter++)
            {
                TableCell cell = new TableCell();
                Button button = new Button();

                button.Attributes.Add("row", (i + 1).ToString());
                button.Attributes.Add("col", (j + 1).ToString());

                if (counter == 16) {
                    button.Text = "";
                }
                else {
                    button.Text = counter.ToString();
                }
                
                button.Click += this.onButtonClick;

                tagToButton.Add((i+1) + "," + (j+1) , button);

                cell.Controls.Add(button);
                buttonsList.Add(button);

                row.Cells.Add(cell);
            }

            table.Rows.Add(row);
        }

        form1.Controls.Add(table);
    }

    private void onButtonClick(object sender, EventArgs e)
    {
        Button button = (Button)sender;

        int row = int.Parse(button.Attributes["row"]);
        int col = int.Parse(button.Attributes["col"]);

        var neighbor = isNeighborEmpty(row, col);

        if (neighbor != null)
        {
            int row2 = int.Parse(neighbor.Attributes["row"]);
            int col2 = int.Parse(neighbor.Attributes["col"]);

            replaceButtons(row, col, row2, col2);
        }

        if (didFinish())
        {
            string strconfirm = "<script>if(confirm('Click ok to start new game?')){Button1.click()}</script>";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Confirm", strconfirm, false);

            startNewGame();
        }
    }

    private void replaceButtons(int i, int j, int i2, int j2)
    {
        string temp = tagToButton[i + "," + j].Text;
        Color tempColor = tagToButton[i + "," + j].BackColor;

        tagToButton[i + "," + j].Text = tagToButton[i2 + "," + j2].Text;
        tagToButton[i + "," + j].BackColor = tagToButton[i2 + "," + j2].BackColor;

        tagToButton[i2 + "," + j2].Text = temp;
        tagToButton[i2 + "," + j2].BackColor = tempColor;
    }

    private Button isNeighborEmpty(int x, int y)
    {

        List<int> xs = new List<int>();
        List<int> ys = new List<int>();

        xs.Add(x);
        ys.Add(y);

        if ((x + 1) % 5 != 0)
        {
            xs.Add(x + 1);
        }

        if ((x - 1) % 5 != 0)
        {
            xs.Add(x - 1);
        }

        if ((y + 1) % 5 != 0)
        {
            ys.Add(y + 1);
        }

        if ((y - 1) % 5 != 0)
        {
            ys.Add(y - 1);
        }

        for (var index = 0; index < xs.Count; index++)
        {
            var x2 = xs[index];

            for (var index2 = 0; index2 < ys.Count; index2++)
            {
                var y2 = ys[index2];

                var dist = Math.Abs(x - x2) + Math.Abs(y - y2);
                
                if (dist == 1) {

                    var button = tagToButton[x2.ToString() + "," + y2.ToString()];

                        if (button.Text.Trim() == "") {
                            return button;
                        }
                    }
                }
            }

            return null;
        }

    private bool didFinish()
    {
        for (int i = 0; i < buttonsList.Count - 1; i++)
        {
            if (!buttonsList[i].Text.Equals((i + 1).ToString()))
            {
                return false;
            }
        }

        return true;
    }

    protected void Restart_Click(object sender, EventArgs e)
    {
        startNewGame();
    }
}