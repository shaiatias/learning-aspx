using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FifteenWebForm : System.Web.UI.Page
{
    private const int size = 4;
    private const int numbers = size * size;
    private const int buttonSize = 50;

    private Button[,] buttons = new Button[size, size];
    private Table t = new Table();

    private HiddenField emptyRow = new HiddenField();
    private HiddenField emptyColumn = new HiddenField();

    private string url = "http://localhost:8317/Handler.ashx";
    private WebClient webClient = new WebClient();
    private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

    protected void Page_Load(object sender, EventArgs e)
    {
        BuildGame();

        if (!IsPostBack)
        {
            StartNewGame();
        }
    }

    private void BuildGame()
    {
        form1.Controls.Add(t);
        form1.Controls.Add(emptyRow);
        form1.Controls.Add(emptyColumn);

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

                buttons[i, j].Click += new EventHandler(AllButtons_Click);
            }
        }

        for (int i = 0; i < size; i++)
        {
            TableRow tr = new TableRow();
            t.Controls.Add(tr);

            for (int j = 0; j < size; j++)
            {
                TableCell tc = new TableCell();

                tc.Controls.Add(buttons[i, j]);
                tr.Controls.Add(tc);
            }
        }
    }

    private void StartNewGame()
    {
        Body.Attributes.CssStyle["background-color"] = "rgb(255,255,255)";

        string request = url + "?numbers=" + (numbers - 1) + "&cmd=Shuffle";
        string resultString = webClient.DownloadString(new Uri(request));
        TextAndColor[] result = (TextAndColor[])jsSerializer.Deserialize(resultString, typeof(TextAndColor[]));

        int index = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (index < numbers - 1)
                {
                    int[] color = result[index].color;
                    buttons[i, j].BackColor = Color.FromArgb(color[0], color[1], color[2]);
                    buttons[i, j].Text = result[index].text;
                    buttons[i, j].Visible = true;
                    index++;
                }
            }
        }

        emptyRow.Value = (size - 1).ToString();
        emptyColumn.Value = (size - 1).ToString();

        buttons[size - 1, size - 1].Text = "X";
        buttons[size - 1, size - 1].Visible = false;
    }

    private void AllButtons_Click(object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        int i = int.Parse(button.ID) / size;
        int j = int.Parse(button.ID) % size;

        int emptyButtonRow = Int32.Parse(emptyRow.Value);
        int emptyButtonColumn = Int32.Parse(emptyColumn.Value);

        string request = url + "?row=" + i + "&column=" + j + "&emptyRow=" + emptyButtonRow + "&emptyColumn=" + emptyButtonColumn + "&cmd=IsValidMove";
        string resultString = webClient.DownloadString(new Uri(request));
        bool isValid = (bool)jsSerializer.Deserialize(resultString, typeof(bool));

        if (!isValid)
        {
            return;
        }

        // Swap the empty cell with the clicked cell.
        buttons[emptyButtonRow, emptyButtonColumn].Text = buttons[i, j].Text;
        buttons[emptyButtonRow, emptyButtonColumn].BackColor = buttons[i, j].BackColor;
        buttons[emptyButtonRow, emptyButtonColumn].Visible = true;
        buttons[i, j].Text = "X";
        buttons[i, j].Visible = false;

        emptyRow.Value = i.ToString();
        emptyColumn.Value = j.ToString();

        ChangeBackgroundColor(button);
        CheckGameOver();
    }

    private void ChangeBackgroundColor(Button button)
    {
        TextAndColor buttonColor = new TextAndColor();
        buttonColor.color[0] = button.BackColor.R;
        buttonColor.color[1] = button.BackColor.G;
        buttonColor.color[2] = button.BackColor.B;
        string buttonColorSerialized = jsSerializer.Serialize(buttonColor);

        TextAndColor bodyColor = new TextAndColor();
        string bodyRgb = Body.Attributes.CssStyle["background-color"];
        bodyColor.color[0] = Int32.Parse(bodyRgb.Split('(')[1].Split(')')[0].Split(',')[0]);
        bodyColor.color[1] = Int32.Parse(bodyRgb.Split('(')[1].Split(')')[0].Split(',')[1]);
        bodyColor.color[2] = Int32.Parse(bodyRgb.Split('(')[1].Split(')')[0].Split(',')[2]);
        string bodyColorSerialized = jsSerializer.Serialize(bodyColor);

        string request = url + "?first=" + buttonColorSerialized + "&second=" + bodyColorSerialized + "&cmd=GetAverageColor";
        string resultString = webClient.DownloadString(new Uri(request));
        TextAndColor avg = (TextAndColor)jsSerializer.Deserialize(resultString, typeof(TextAndColor));
        Body.Attributes.CssStyle["background-color"] = "rgb(" + avg.color[0] + "," + avg.color[1] + "," + avg.color[2] + ")";
    }

    private void CheckGameOver()
    {
        TextAndColor[] textAndColorArray = new TextAndColor[numbers - 1];

        int index = 0;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (index < numbers - 1)
                {
                    textAndColorArray[index] = new TextAndColor();
                    textAndColorArray[index].text = buttons[i, j].Text;
                    index++;
                }
            }
        }

        string serializedArray = jsSerializer.Serialize(textAndColorArray);

        string request = url + "?array=" + serializedArray + "&cmd=CheckGameOver";
        string resultString = webClient.DownloadString(new Uri(request));
        bool isGameOver = (bool)jsSerializer.Deserialize(resultString, typeof(bool));

        if (isGameOver)
        {
            ClientScript.RegisterClientScriptBlock(typeof(Page), "", "<script>confirm('Game Over! New Game?');</script>");
            StartNewGame();
        }
    }
}