using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public class ValidMove
    {
        public int To { get; private set; }
        //public List<int> Breaks { get; private set; }

        public ValidMove(int fromX, int fromY, int toX, int toY)
        {
            this.To = toX + toY*8;
            //this.Breaks = new List<int>();

            //if (breakable)
            //{
            //    int x = fromX, y = fromY;
            //    if (x != toX || y != toY)
            //    {
            //        while (true)
            //        {
            //            x += (fromX < toX ? 1 : (fromX > toX ? -1 : 0));
            //            y += (fromY < toY ? 1 : (fromY > toY ? -1 : 0));
            //            if (x == toX && y == toY)
            //            {
            //                break;
            //            }
            //            this.Breaks.Add(x + 8 * y);
            //        }
            //    }
            //}

        }
    }
}
