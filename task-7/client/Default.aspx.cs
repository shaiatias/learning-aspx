using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Web.Script.Serialization;

public partial class _Default : System.Web.UI.Page
{
    private string url = "http://localhost:61473/Handler.ashx";
    private WebClient webClient = new WebClient();
    private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

    Table table = new Table();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack || Session["cells"] == null)
        {
            BuildGame();
            List<Cell> mCells = DoShuffle();
            Session["cells"] = mCells;
        }

        else
        {
            List<Cell> mCells = (List<Cell>)Session["cells"];
            ReBuildGame(mCells);
        }
    }
    
    private List<Cell> DoShuffle()
    {
        string request = url + "?command=shuffle";
        string resultString = webClient.DownloadString(new Uri(request));

        List<Cell> response = (List<Cell>)jsSerializer.Deserialize(resultString, typeof(List<Cell>));

        ReBuildGame(response);

        return response;
    }

    private void BuildGame()
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

                if (counter == 16)
                {
                    button.Text = "";
                }
                else
                {
                    button.Text = counter.ToString();
                }

                button.Click += this.onButtonClick;

                cell.Controls.Add(button);
                row.Cells.Add(cell);

                //mCells.Add(new Cell { Color = "white", Column = (j+1).ToString(), Row = (i+1).ToString(), Text = button.Text });
            }

            table.Rows.Add(row);
        }

        form1.Controls.Add(table);
    }

    class ClickResponse
    {
        public bool replace { get; set; }
        public bool finish { get; set; }
    }

    private void onButtonClick(object sender, EventArgs e)
    {
        List<Cell> mCells = (List<Cell>)Session["cells"];

        Button button = (Button)sender;

        int row = int.Parse(button.Attributes["row"]);
        int col = int.Parse(button.Attributes["col"]);

        Cell emptyCell = mCells.Find(cell => cell.Text == "");

        int emptyRow = Int32.Parse(emptyCell?.Row);
        int emptyCol = Int32.Parse(emptyCell?.Column);

        string request = url + "?command=shouldReplace&fromX=" + row + "&fromY=" + col + "&toX=" + emptyRow + "&toY=" + emptyCol;
        string resultString = webClient.DownloadString(new Uri(request));

        var shouldReplace = (bool)jsSerializer.Deserialize(resultString, typeof(bool));

        if (shouldReplace)
        {
            var fromCell = mCells.Find(c => c.Row.Equals(row.ToString()) && c.Column.Equals(col.ToString()));

            if (fromCell != null)
            {
                emptyCell.Text = fromCell.Text;
                fromCell.Text = "";
            }
        }

        Session["cells"] = mCells;
        ReBuildGame(mCells);
    }

    private void ReBuildGame(List<Cell> cells)
    {
        form1.Controls.Clear();
        table.Rows.Clear();

        var iter = cells.GetEnumerator();

        for (int i = 0; i < 4; i++)
        {
            TableRow row = new TableRow();

            for (int j = 0; j < 4; j++)
            {
                iter.MoveNext();
                TableCell cell = new TableCell();
                Button button = new Button();

                button.Attributes.Add("row", (i + 1).ToString());
                button.Attributes.Add("col", (j + 1).ToString());

                button.Text = iter.Current.Text;

                button.Click += this.onButtonClick;

                cell.Controls.Add(button);
                row.Cells.Add(cell);
            }

            table.Rows.Add(row);
        }

        form1.Controls.Add(table);
    }
}