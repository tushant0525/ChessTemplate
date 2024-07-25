using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override void CalculatePossibleMoves()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();
        Vector2Int currentPosition = placementHandler.GetPosition();

        // All possible knight moves
        Vector2Int[] moves = GetKnightMoves(currentPosition);

        foreach (var move in moves)
        {
            if (IsValidBoardPosition(move))
            {
                ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(move);

                if (pieceAtNewPosition == null || pieceAtNewPosition.IsWhite != IsWhite)
                {
                    possibleMoves.Add(move);

                    // If there is a piece of the opposite color, it can be captured
                    if (pieceAtNewPosition != null && pieceAtNewPosition.IsWhite != IsWhite)
                    {
                        capturedMoves.Add(move);
                    }
                }
            }
        }
    }

    // Helper method to get all possible knight moves from the current position
    private Vector2Int[] GetKnightMoves(Vector2Int currentPosition)
    {
        return new[]
        {
            new Vector2Int(currentPosition.x + 2, currentPosition.y + 1),
            new Vector2Int(currentPosition.x + 2, currentPosition.y - 1),
            new Vector2Int(currentPosition.x - 2, currentPosition.y + 1),
            new Vector2Int(currentPosition.x - 2, currentPosition.y - 1),
            new Vector2Int(currentPosition.x + 1, currentPosition.y + 2),
            new Vector2Int(currentPosition.x + 1, currentPosition.y - 2),
            new Vector2Int(currentPosition.x - 1, currentPosition.y + 2),
            new Vector2Int(currentPosition.x - 1, currentPosition.y - 2)
        };
    }
}