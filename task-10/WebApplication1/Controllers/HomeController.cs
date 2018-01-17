using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        Random random = new Random();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Shuffle()
        {
            var buttons = await Task.Run(() => doShuffle());
            return Json(buttons);
        }

        private List<Cell> doShuffle()
        {
            return getRandomizedList().Select((item, index) => {
                return item == 16 ?
                    new Cell("", "rgba(0, 0, 0, 0)", Math.Ceiling((decimal)(index / 4)).ToString(), (index % 4).ToString()) :
                    new Cell(item.ToString(), colorToString(getRandomColor()), Math.Ceiling((decimal)(index / 4)).ToString(), (index % 4).ToString());
            }).ToList();
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

        [HttpPost]
        public async Task<JsonResult> onClick(ClickRequest request)
        {
            var result = await Task.Run(() =>
            {
                request.cells = doMove(request);                

                return new
                {
                    cells = request.cells,
                    finish = didFinish(request.cells)
                };
            });

            return Json(result);
        }

        private bool didFinish(List<Cell> cells)
        {
            return cells[0].Text != null && cells[0].Text.Equals("1") && cells[1].Text != null && cells[1].Text.Equals("2");
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

            possibleNeighbors = possibleNeighbors.FindAll(a => {

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
}