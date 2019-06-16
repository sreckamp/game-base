using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GameBase.Model.Rules
{
    /// <summary>
    /// Rules used to match edges
    /// <typeparamref name="P">The type of piece placed at each game board location</typeparamref>
    /// </summary>
    public interface IPlaceRule<P, M> where P : Piece where M : Move
    {
        bool Applies(IGameBoard<P, M> board, P piece, M move);
        bool Fits(IGameBoard<P, M> board, P piece, M move);
    }
}
