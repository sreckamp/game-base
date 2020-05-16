namespace GameBase.Model.Rules
{
    /// <summary>
    /// Rules used to match edges
    /// <typeparamref name="TP">The type of <see cref="IPiece"/></typeparamref>
    /// <typeparamref name="TM">The type of <see cref="Move"/></typeparamref>
    /// </summary>
    public interface IPlaceRule<in TP, in TM> where TP : IPiece where TM : Move
    {
        bool Applies(IGameBoard<TP> board, TP piece, TM move);
        bool Fits(IGameBoard<TP> board, TP piece, TM move);
    }
}
