using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TextAndColor
/// </summary>

[Serializable]
public class TextAndColor
{
    public string text;
    public int[] color;

    public TextAndColor()
    {
        text = null;
        color = new int[3]; // rgb
    }
}