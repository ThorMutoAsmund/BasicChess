using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ChessCore;

namespace BasicVisualizer
{
    public class BasicVizualizer : IVisualizer
    {
        private ChessGame Game;
        private BoardForm BoardForm;

        public void Init(ChessGame game)
        {
            this.Game = game;
            this.BoardForm = new BoardForm(Game);
            this.BoardForm.Show();
            this.BoardForm.DrawGame();

            this.Game.BoardChanged += Game_BoardChanged;
            this.Game.NumberChanged += Game_NumberChanged;
            this.Game.ExecutionTimeUpdated += Game_ExecutionTimeUpdated;
            this.Game.ResultChanged += Game_ResultChanged;
        }

        private void Game_BoardChanged(ChessGame game)
        {
            Redraw();
        }

        private void Game_ResultChanged(ChessGame game, GameResult result)
        {
            string s = String.Empty;
            switch (result)
            {
                case GameResult.BlackCheckMate:
                    s = "Black check mate";
                    break;
                case GameResult.WhiteCheckMate:
                    s = "White check mate";
                    break;
                case GameResult.BlackKingTaken:
                    s = "Black lost";
                    break;
                case GameResult.WhiteKingTaken:
                    s = "White lost";
                    break;
                case GameResult.StaleMate:
                    s = "Stale mate";
                    break;
                case GameResult.BlackResigned:
                    s = "Black resigned";
                    break;
                case GameResult.WhiteResigned:
                    s = "White resigned";
                    break;
                case GameResult.Draw:
                    s = "Draw";
                    break;
            }
            if (s != String.Empty)
            {
                this.BoardForm.ShowResult(s);
            }
        }

        private void Game_ExecutionTimeUpdated(TimeSpan executionTime)
        {
            this.BoardForm.ShowExecutionTime(executionTime.ToString());
        }

        private void Game_NumberChanged(int numberOfSearches, int numberOfCacheHits)
        {
            this.BoardForm.ShowNumber(numberOfSearches, numberOfCacheHits);
        }

        public void Redraw()
        {
            this.BoardForm.DrawGame();
        }
    }
}
