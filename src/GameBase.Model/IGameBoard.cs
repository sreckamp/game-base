using System.Drawing;

namespace GameBase.Model
{
    public interface IGameBoard<TP,TM> where TP : Piece where TM : Move
    {
        TP this[Point location] { get; }
        bool IsEmpty { get; }
        ObservableList<Point> AvailableLocations { get; }
    }
}
