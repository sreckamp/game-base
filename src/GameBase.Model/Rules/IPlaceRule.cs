namespace GameBase.Model.Rules
{
    /// <summary>
    /// Rules used to match edges
    /// <typeparamref name="TP">The type of <see cref="Piece"/></typeparamref>
    /// <typeparamref name="TM">The type of <see cref="Move"/></typeparamref>
    /// </summary>
    public interface IPlaceRule<TP, TM> where TP : Piece where TM : Move
    {
        bool Applies(IGameBoard<TP, TM> board, TP piece, TM move);
        bool Fits(IGameBoard<TP, TM> board, TP piece, TM move);
    }
}
