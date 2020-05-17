using System.Drawing;

namespace GameBase.Model
{
    public interface IGameBoard<out TP>
    {
        TP this[Point location] { get; }
        bool IsEmpty { get; }
    }
}
