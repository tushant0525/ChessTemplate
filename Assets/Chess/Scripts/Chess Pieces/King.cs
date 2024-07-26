using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override void CalculatePossibleMoves()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();

        Vector2Int currentPosition = placementHandler.GetPosition();

        // Define all possible move directions for the King
        Vector2Int[] moves =
        {
            new Vector2Int(currentPosition.x + 1, currentPosition.y), // Move Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y), // Move Left
            new Vector2Int(currentPosition.x, currentPosition.y + 1), // Move Up
            new Vector2Int(currentPosition.x, currentPosition.y - 1), // Move Down
            new Vector2Int(currentPosition.x + 1, currentPosition.y + 1), // Move Up-Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y + 1), // Move Up-Left
            new Vector2Int(currentPosition.x + 1, currentPosition.y - 1), // Move Down-Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y - 1) // Move Down-Left
        };

        foreach (var move in moves)
        {
            if (ChessBoardPlacementHandler.Instance.IsValidBoardPosition(move))
            {
                ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(move);

                if (pieceAtNewPosition == null || pieceAtNewPosition.IsWhite != IsWhite)
                {
                    // Simulate the move to check if it leaves the King in check
                    if (IsMoveSafe(move))
                    {
                        possibleMoves.Add(move);

                        if (pieceAtNewPosition != null && pieceAtNewPosition.IsWhite != IsWhite)
                        {
                            capturedMoves.Add(move);
                        }
                    }
                }
            }
        }
    }

    public void CalculatePossibleMovesWithoutSafety()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();

        Vector2Int currentPosition = placementHandler.GetPosition();

        // Define all possible move directions for the King
        Vector2Int[] moves =
        {
            new Vector2Int(currentPosition.x + 1, currentPosition.y), // Move Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y), // Move Left
            new Vector2Int(currentPosition.x, currentPosition.y + 1), // Move Up
            new Vector2Int(currentPosition.x, currentPosition.y - 1), // Move Down
            new Vector2Int(currentPosition.x + 1, currentPosition.y + 1), // Move Up-Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y + 1), // Move Up-Left
            new Vector2Int(currentPosition.x + 1, currentPosition.y - 1), // Move Down-Right
            new Vector2Int(currentPosition.x - 1, currentPosition.y - 1) // Move Down-Left
        };

        foreach (var move in moves)
        {
            if (ChessBoardPlacementHandler.Instance.IsValidBoardPosition(move))
            {
                ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(move);

                if (pieceAtNewPosition == null || pieceAtNewPosition.IsWhite != IsWhite)
                {
                    possibleMoves.Add(move);

                    if (pieceAtNewPosition != null && pieceAtNewPosition.IsWhite != IsWhite)
                    {
                        capturedMoves.Add(move);
                    }
                }
            }
        }
    }

    private bool IsMoveSafe(Vector2Int newPosition)
    {
        // Check if any opponent piece can attack the King's position
        foreach (var piece in ChessBoardPlacementHandler.Instance.GetAllPieces())
        {
            if (piece.IsWhite != IsWhite)
            {
                // Special case for pawn capture moves
                if (piece is Pawn)
                {
                    Pawn pawn = (Pawn)piece;
                    pawn.CalculatePossibleMovesWithDiagonals();

                    if (piece.possibleMoves.Contains(newPosition))
                    {
                        return false;
                    }
                }
                else if (piece is King)
                {
                    King king = (King)piece;
                    king.CalculatePossibleMovesWithoutSafety();

                    if (piece.possibleMoves.Contains(newPosition))
                    {
                        return false;
                    }
                }
                else
                {
                    piece.CalculatePossibleMoves();

                    if (piece.possibleMoves.Contains(newPosition))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}