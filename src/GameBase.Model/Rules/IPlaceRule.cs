namespace GameBase.Model.Rules
{
    /// <summary>
    /// Rules used to match edges
    /// <typeparamref name="P">The type of piece placed at each game board location</typeparamref>
    /// </summary>
    public interface IPlaceRule<TP, TM> where TP : Piece where TM : Move
    {
        bool Applies(IGameBoard<TP, TM> board, TP piece, TM move);
        bool Fits(IGameBoard<TP, TM> board, TP piece, TM move);
    }
}
