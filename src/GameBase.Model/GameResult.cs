﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Model
{
    public enum GameResult
    {
        Unknown,
        Lose,
        Win,
        Draw
    }

    public static class GameResultExtensions
    {
        public static GameResult Opposite(this GameResult result)
        {
            switch(result)
            {
                case GameResult.Win:
                    return GameResult.Lose;
                case GameResult.Lose:
                    return GameResult.Win;
                default:
                    return result;
            }
        }
    }
}