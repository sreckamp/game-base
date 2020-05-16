// ReSharper disable UnusedMember.Global
namespace GameBase.Model
{
    public enum GameResult
    {
        Unknown,
        Lose,
        Win,
        Draw
    }

    // ReSharper disable once UnusedType.Global
    public static class GameResultExtensions
    {
        public static GameResult Opposite(this GameResult result)
        {
            return result switch
            {
                GameResult.Win => GameResult.Lose,
                GameResult.Lose => GameResult.Win,
                _ => result
            };
        }
    }
}
