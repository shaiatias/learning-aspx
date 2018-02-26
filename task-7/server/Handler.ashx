<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class Handler : IHttpHandler
{

    private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
    Random random = new Random();

    public void ProcessRequest(HttpContext context)
    {
        string command = context.Request.QueryString["command"]?.ToString();

        switch (command)
        {
            case "shuffle":
                doShuffle(context);
                break;

            case "shouldReplace":
                doShouldReplace(context);
                break;

            default:
                break;
        }
    }

    private void doShuffle(HttpContext context)
    {
        var list = getRandomizedList().Select((item, index) =>
        {
            var x = Math.Ceiling((decimal)(index / 4)) + 1;
            var y = (index % 4) + 1;

            return item == 16 ?
                new Cell("", "rgba(0, 0, 0, 0)", x.ToString(), y.ToString()) :
                new Cell(item.ToString(), colorToString(getRandomColor()), x.ToString(), y.ToString());
        }).ToList();

        string result = jsSerializer.Serialize(list);
        context.Response.Write(result);
    }

    private List<int> getRandomizedList()
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

    private Color getRandomColor()
    {
        return Color.FromArgb(
            random.Next(100, 255), // r
            random.Next(100, 255), // g
            random.Next(100, 255)  // b
        );
    }

    private string colorToString(Color color)
    {
        return $"rgb({color.R}, {color.G}, {color.B})";
    }

    public class ClickRequest
    {
        public int row { get; set; }
        public int column { get; set; }
        public List<Cell> cells { get; set; }
    }

    public void doShouldReplace(HttpContext context)
    {
        int fromX = Int32.Parse(context.Request.QueryString["fromX"]);
        int fromY = Int32.Parse(context.Request.QueryString["fromY"]);
        int toX = Int32.Parse(context.Request.QueryString["toX"]);
        int toY = Int32.Parse(context.Request.QueryString["toY"]);

        bool replace = shouldReplace(fromX, fromY, toX, toY);

        string result = jsSerializer.Serialize(replace);

        context.Response.Write(result);
    }

    private bool didFinish(List<Cell> cells)
    {
        return cells[0].Text != null && cells[0].Text.Equals("1") && cells[1].Text != null && cells[1].Text.Equals("2");
    }

    private bool shouldReplace(int fromX, int fromY, int toX, int toY) {

        int x = fromX;
        int y = fromY;

        var possibleNeighbors = new List<int[]> {
                new int[2] { x - 1, y },
                new int[2] { x, y - 1 },
                new int[2] { x, y + 1 },
                new int[2] { x + 1, y }
            };

        possibleNeighbors = possibleNeighbors.FindAll(a =>
        {

            var x2 = a[0];
            var y2 = a[1];

            if (!(1 <= x2 && x2 <= 4))
            {
                return false;
            }

            if (!(1 <= y2 && y2 <= 4))
            {
                return false;
            }

            return true;
        });

        return possibleNeighbors.Find(a => {

            var x2 = a[0];
            var y2 = a[1];
            return (x2 == toX && y2 == toY);

        }) != null;
    }

    private List<Cell> doMove(ClickRequest request)
    {
        int x = request.row;
        int y = request.column;

        var possibleNeighbors = new List<int[]> {
                new int[2] { x - 1, y },
                new int[2] { x, y - 1 },
                new int[2] { x, y + 1 },
                new int[2] { x + 1, y }
            };

        possibleNeighbors = possibleNeighbors.FindAll(a =>
        {

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
            return (request.cells[(x2 * 4) + y2].Text == null);
        });

        if (emptyNeighbor == null)
        {
            return request.cells;
        }

        var myIndex = ((x * 4) + y);
        var neighborIndex = ((emptyNeighbor[0] * 4) + emptyNeighbor[1]);

        var temp = request.cells[myIndex].Text;
        request.cells[myIndex].Text = request.cells[neighborIndex].Text;
        request.cells[neighborIndex].Text = temp;

        temp = request.cells[myIndex].Color;
        request.cells[myIndex].Color = request.cells[neighborIndex].Color;
        request.cells[neighborIndex].Color = temp;

        return request.cells;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}