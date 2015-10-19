using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessCore
{
    public class ValidMoveSet
    {
        private int FromX { get; set; }
        private int FromY { get; set; }

        public List<ValidMove> Moves { get; private set; }
        public ValidMoveCaptureRule CaptureRule { get; private set; }
        public bool Breakable { get; private set; }
        
        public ValidMoveSet(int fromX, int fromY, ValidMoveCaptureRule captureRule = ValidMoveCaptureRule.None, bool breakable = true)
        {
            this.FromX = fromX;
            this.FromY = fromY;
            this.Moves = new List<ValidMove>();
            this.CaptureRule = captureRule;
            this.Breakable = breakable;
        }

        public void AddMove(int toX, int toY)
        {
            this.Moves.Add(new ValidMove(this.FromX, this.FromY, toX, toY));
        }
    }
}
