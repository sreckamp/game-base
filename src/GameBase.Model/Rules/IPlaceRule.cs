using System.Drawing;

namespace GameBase.Model.Rules
{
    /// <summary>
    /// Rules used to validate placement
    /// <typeparamref name="TP">The type of <see cref="IPiece"/></typeparamref>
    /// </summary>
    public interface IPlaceRule<in TB, in TP> where TB:IGameBoard<TP>
    {
        bool Applies(TB board, TP piece, Point location);
        bool Fits(TB board, TP piece, Point location);
    }
}
