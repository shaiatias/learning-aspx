using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[Serializable]
public class Cell
{
    public string Text { get; set; }
    public string Color { get; set; }
    public string Row { get; set; }
    public string Column { get; set; }

    public Cell()
    {
        Text = null;
        Color = null;
        Row = null;
        Column = null;
    }

    public Cell(string text, string color, string row, string column)
    {
        Text = text;
        Color = color;
        Row = row;
        Column = column;
    }
}