using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    private bool hasMoved = false;

    public override List<Vector2Int> GetPossibleMoves()
    {
        possibleMoves.Clear();
        Vector2Int currentPosition = _placementHandler.GetPosition();

        if ((!IsWhite && currentPosition.x != 1) || (IsWhite && currentPosition.x != 6))
        {
            hasMoved = true;
        }

        int direction = IsWhite ? -1 : 1; // White pawns move down, black pawns move up
     //   Debug.Log("IsWhite " + IsWhite + " with direction " + direction);
        // Forward move
        Vector2Int forwardMove = new Vector2Int(currentPosition.x + direction, currentPosition.y);
//        Debug.Log("Forward move " + forwardMove);
        if (IsValidBoardPosition(forwardMove) && ChessBoardPlacementHandler.Instance.GetPieceAt(forwardMove) == null)
        {
            possibleMoves.Add(forwardMove);

            // Double move on first turn
            if (!hasMoved)
            {
                Vector2Int doubleMove = new Vector2Int(currentPosition.x + 2 * direction, currentPosition.y);

                if (ChessBoardPlacementHandler.Instance.GetPieceAt(doubleMove) == null)
                {
                    possibleMoves.Add(doubleMove);
                }
            }
        }

        // Capture moves
        Vector2Int leftCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y - 1);
        Vector2Int rightCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y + 1);
        /*Vector2Int leftCapture = new Vector2Int(currentPosition.x - 1, currentPosition.y + direction);
        Vector2Int rightCapture = new Vector2Int(currentPosition.x + 1, currentPosition.y + direction);*/

        if (CanCapture(leftCapture))
            possibleMoves.Add(leftCapture);

        if (CanCapture(rightCapture))
            possibleMoves.Add(rightCapture);

        // TODO: Implement en passant rule here
        for (int i = 0; i < possibleMoves.Count; i++)
        {
            Debug.Log("Possible moves "+possibleMoves[i]);
        }
        return possibleMoves;
    }

    private bool CanCapture(Vector2Int position)
    {
        if (!IsValidBoardPosition(position))
            return false;

        ChessPiece pieceAtPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(position);
        return pieceAtPosition != null && pieceAtPosition.IsWhite != IsWhite;
    }

    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        hasMoved = true;
    }

    private bool IsValidBoardPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    // TODO: Implement promotion when pawn reaches the end of the board
}