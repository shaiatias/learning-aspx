<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading.Tasks;

public class Handler : HttpTaskAsyncHandler
{
    private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

    public override async Task ProcessRequestAsync(HttpContext context)
    {
        if (context.Request.QueryString["cmd"] == "Shuffle")
        {
            await Shuffle(context);
        }

        if (context.Request.QueryString["cmd"] == "GetDirection")
        {
            await GetDirection(context);
        }

        if (context.Request.QueryString["cmd"] == "GetAverageColor")
        {
            await GetAverageColor(context);
        }

        if (context.Request.QueryString["cmd"] == "CheckGameOver")
        {
            await CheckGameOver(context);
        }
    }

    private async Task Shuffle(HttpContext context)
    {
        int n = (int)jsSerializer.Deserialize(context.Request.QueryString["numbers"], typeof(int));

        TextAndColor[] textAndColorArray = await Task<TextAndColor[]>.Run(() => GetArray(n));

        string result = jsSerializer.Serialize(textAndColorArray);
        context.Response.Write(result);
    }

    private TextAndColor[] GetArray(int n)
    {
        TextAndColor[] textAndColorArray = new TextAndColor[n];
        Random r1 = new Random(), r2 = new Random();

        int[] randomNumbers = new int[n];

        for (int i = 0; i < n; i++)
        {
            randomNumbers[i] = i + 1;
        }

        for (int i = 1; i < n; i++)
        {
            int index1 = r1.Next(n);
            int index2 = r1.Next(n);

            int temp = randomNumbers[index1];
            randomNumbers[index1] = randomNumbers[index2];
            randomNumbers[index2] = temp;
        }

        for (int i = 0; i < n; i++)
        {
            TextAndColor textAndColor = new TextAndColor();
            textAndColor.text = randomNumbers[i].ToString();
            textAndColor.color = "rgb(" + r2.Next(100, 255) + "," + r2.Next(100, 255) + "," + r2.Next(100, 255) + ")";

            textAndColorArray[i] = textAndColor;
        }

        return textAndColorArray;
    }

    private async Task GetDirection(HttpContext context)
    {
        int buttonRow = (int)jsSerializer.Deserialize(context.Request.QueryString["buttonRow"], typeof(int));
        int buttonColumn = (int)jsSerializer.Deserialize(context.Request.QueryString["buttonColumn"], typeof(int));
        int emptyRow = (int)jsSerializer.Deserialize(context.Request.QueryString["emptyRow"], typeof(int));
        int emptyColumn = (int)jsSerializer.Deserialize(context.Request.QueryString["emptyColumn"], typeof(int));

        string direction = await Task<string>.Run(() => GetDistance(buttonRow, buttonColumn, emptyRow, emptyColumn));

        string result = jsSerializer.Serialize(direction);
        context.Response.Write(result);
    }

    private string GetDistance(int buttonRow, int buttonColumn, int emptyRow, int emptyColumn)
    {
        if (buttonRow == emptyRow)
        {
            if (buttonColumn - emptyColumn == -1)
            {
                return "Right";
            }
            if (buttonColumn - emptyColumn == 1)
            {
                return "Left";
            }
        }

        if (buttonColumn == emptyColumn)
        {
            if (buttonRow - emptyRow == -1)
            {
                return "Down";
            }
            if (buttonRow - emptyRow == 1)
            {
                return "Up";
            }
        }

        return "None";
    }

    private async Task GetAverageColor(HttpContext context)
    {
        TextAndColor first = (TextAndColor)jsSerializer.Deserialize(context.Request.QueryString["first"], typeof(TextAndColor));
        TextAndColor second = (TextAndColor)jsSerializer.Deserialize(context.Request.QueryString["second"], typeof(TextAndColor));

        TextAndColor avg = await Task<TextAndColor>.Run(() => GetAvg(first, second));

        string result = jsSerializer.Serialize(avg);
        context.Response.Write(result);
    }

    private TextAndColor GetAvg(TextAndColor first, TextAndColor second)
    {
        int firstR = Int32.Parse(first.color.Split('(')[1].Split(')')[0].Split(',')[0]);
        int firstG = Int32.Parse(first.color.Split('(')[1].Split(')')[0].Split(',')[1]);
        int firstB = Int32.Parse(first.color.Split('(')[1].Split(')')[0].Split(',')[2]);

        int secondR = Int32.Parse(second.color.Split('(')[1].Split(')')[0].Split(',')[0]);
        int secondG = Int32.Parse(second.color.Split('(')[1].Split(')')[0].Split(',')[1]);
        int secondB = Int32.Parse(second.color.Split('(')[1].Split(')')[0].Split(',')[2]);

        TextAndColor avg = new TextAndColor();
        avg.color = "rgb(" + (firstR + secondR) / 2 + "," + (firstG + secondG) / 2 + "," + (firstB + secondB) / 2 + ")";

        return avg;
    }

    public async Task CheckGameOver(HttpContext context)
    {
        TextAndColor[] a = (TextAndColor[])jsSerializer.Deserialize(context.Request.QueryString["array"], typeof(TextAndColor[]));

        bool isGameOver = await Task<TextAndColor>.Run(() => IsGameOver(a));

        string result = jsSerializer.Serialize(isGameOver);
        context.Response.Write(result);
    }

    private bool IsGameOver(TextAndColor[] a)
    {
        bool isGameOver = false;

        for (int i = 0; i < a.Length; i++)
        {
            int value = Int32.Parse(a[i].text);
            int name = Int32.Parse(a[i].color);

            if (value != 1 && value != 2)
            {
                continue;
            }

            if (value == name + 1)
            {
                isGameOver = true;
            }
            else
            {
                return false;
            }
        }

        return isGameOver;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}