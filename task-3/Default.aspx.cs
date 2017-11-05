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

    protected void Page_Load(object sender, EventArgs e)
    {
        buildTable();
    }

    private void buildTable()
    {
        var enumerator = getRandomizedList().GetEnumerator();
        enumerator.MoveNext();

        for (int i = 1; i <= 4; i++)
        {
            for (int j = 1; j <= 4; j++, enumerator.MoveNext())
            {
                
                var currentIndex = (i - 1) * 4 + j;

                Button button = new Button();
                button.UseSubmitBehavior = false;

                button.Text = enumerator.Current.ToString();

                button.Attributes.Add("data-tag", i + "," + j);
                button.Attributes.Add("data-expected", currentIndex.ToString());
                
                button.OnClientClick = "onButtonClick(" + i + ", " + j + "); return false;";

                button.BackColor = enumerator.Current == 16 ? Color.Transparent : getRandomColor();

                button.Style["top"] = ((i - 1) * 55) + "px";
                button.Style["left"] = ((j - 1) * 55) + "px";
                button.Style["color"] = enumerator.Current == 16 ? "transparent" : "block";
                button.Style["position"] = "absolute";

                form1.Controls.Add(button);
            }
        }
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
}