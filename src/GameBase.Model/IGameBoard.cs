using System.Drawing;

namespace GameBase.Model
{
    public interface IGameBoard<out TP> where TP : IPiece
    {
        TP this[Point location] { get; }
        bool IsEmpty { get; }
        // ObservableList<Point> AvailableLocations { get; }
    }
}
