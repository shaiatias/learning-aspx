<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Drawing;
using System.Web;
using System.Web.Script.Serialization;

public class Handler : IHttpHandler
{
    private JavaScriptSerializer jsSerializer = new JavaScriptSerializer();

    public void ProcessRequest(HttpContext context)
    {
        if (context.Request.QueryString["cmd"] == "Shuffle")
        {
            Shuffle(context);
        }

        if (context.Request.QueryString["cmd"] == "IsValidMove")
        {
            IsValidMove(context);
        }

        if (context.Request.QueryString["cmd"] == "GetAverageColor")
        {
            GetAverageColor(context);
        }

        if (context.Request.QueryString["cmd"] == "CheckGameOver")
        {
            CheckGameOver(context);
        }
    }

    private void Shuffle(HttpContext context)
    {
        int n = (int)jsSerializer.Deserialize(context.Request.QueryString["numbers"], typeof(int));

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
            textAndColor.color[0] = r2.Next(100, 255); // r
            textAndColor.color[1] = r2.Next(100, 255); // g
            textAndColor.color[2] = r2.Next(100, 255); // b

            textAndColorArray[i] = textAndColor;
        }

        string result = jsSerializer.Serialize(textAndColorArray);
        context.Response.Write(result);
    }

    private void IsValidMove(HttpContext context)
    {
        bool isValid = false;

        int row = (int)jsSerializer.Deserialize(context.Request.QueryString["row"], typeof(int));
        int column = (int)jsSerializer.Deserialize(context.Request.QueryString["column"], typeof(int));
        int emptyRow = (int)jsSerializer.Deserialize(context.Request.QueryString["emptyRow"], typeof(int));
        int emptyColumn = (int)jsSerializer.Deserialize(context.Request.QueryString["emptyColumn"], typeof(int));

        int distance = Math.Abs(emptyRow - row) + Math.Abs(emptyColumn - column);

        if (distance == 1)
        {
            isValid = true;
        }

        string result = jsSerializer.Serialize(isValid);
        context.Response.Write(result);
    }

    private void GetAverageColor(HttpContext context)
    {
        TextAndColor first = (TextAndColor)jsSerializer.Deserialize(context.Request.QueryString["first"], typeof(TextAndColor));
        TextAndColor second = (TextAndColor)jsSerializer.Deserialize(context.Request.QueryString["second"], typeof(TextAndColor));

        TextAndColor avg = new TextAndColor();
        avg.color[0] = (first.color[0] + second.color[0]) / 2;
        avg.color[1] = (first.color[1] + second.color[1]) / 2;
        avg.color[2] = (first.color[2] + second.color[2]) / 2;

        string result = jsSerializer.Serialize(avg);
        context.Response.Write(result);
    }

    public void CheckGameOver(HttpContext context)
    {
        TextAndColor[] a = (TextAndColor[])jsSerializer.Deserialize(context.Request.QueryString["array"], typeof(TextAndColor[]));
        bool isGameOver = true;

        for (int i = 0; i < 2 /* a.Length */; i++)
        {
            string index = (i + 1).ToString();

            if (!a[i].text.Equals(index))
            {
                isGameOver = false;
                break;
            }
        }

        string result = jsSerializer.Serialize(isGameOver);
        context.Response.Write(result);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}