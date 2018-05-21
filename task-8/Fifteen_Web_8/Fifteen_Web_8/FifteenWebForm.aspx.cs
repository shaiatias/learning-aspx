using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FifteenWebForm : System.Web.UI.Page
{
    private const int size = 4;
    private const int buttonSize = 50;

    private Button[,] buttons = new Button[size, size];
    private Table t = new Table();

    private HiddenField hiddenSize = new HiddenField();

    protected void Page_Load(object sender, EventArgs e)
    {
        BuildGame();
    }

    private void BuildGame()
    {
        hiddenSize.Value = size.ToString();
        hiddenSize.ID = "hiddenSize";
        form1.Controls.Add(hiddenSize);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                buttons[i, j] = new Button();
                buttons[i, j].ID = (i * size + j).ToString();
                buttons[i, j].Width = buttonSize;
                buttons[i, j].Height = buttonSize;
                buttons[i, j].Font.Size = new FontUnit("X-Large");
                buttons[i, j].Font.Name = "Tahoma";
                buttons[i, j].Font.Bold = true;

                buttons[i, j].Style.Add("top", i * buttonSize + "px");
                buttons[i, j].Style.Add("left", j * buttonSize + "px");
                buttons[i, j].Style.Add("position", "absolute");

                buttons[i, j].Attributes.Add("class", "buttons");
                buttons[i, j].Attributes.Add("onclick", "javascript:allButtonsClick(this); return false;");

                form1.Controls.Add(buttons[i, j]);
            }
        }
    }
}