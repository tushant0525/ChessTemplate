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
            if (IsValidBoardPosition(move))
            {
                ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(move);

                if (pieceAtNewPosition == null || pieceAtNewPosition.IsWhite != IsWhite)
                {
                    // Simulate the move to check if it leaves the King in check
                    if (IsMoveSafe(currentPosition, move))
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

    private bool IsMoveSafe(Vector2Int currentPosition, Vector2Int newPosition)
    {
        // Temporarily simulate the move
        ChessPiece[,] originalBoardState = CloneBoardState();

        // Move the piece temporarily
        ChessPiece pieceAtCurrentPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(currentPosition);
        ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(newPosition);

        if (pieceAtCurrentPosition != null)
        {
            pieceAtCurrentPosition.placementHandler.SetPosition(newPosition);
        }

        if (pieceAtNewPosition != null)
        {
            // Temporarily remove the piece from the board
            pieceAtNewPosition.placementHandler.SetPosition(new Vector2Int(-1, -1)); // Move it off the board
        }

        // Check if the King is in check after the move
        bool isSafe = !IsKingInCheck();

        // Restore the original board state
        RestoreBoardState(originalBoardState);

        return isSafe;
    }

    private bool IsKingInCheck()
    {
        // Get the current position of the King
        Vector2Int kingPosition = placementHandler.GetPosition();

        // Check if any opponent piece can attack the King's position
        foreach (var piece in ChessBoardPlacementHandler.Instance.GetAllPieces())
        {
            if (piece.IsWhite != IsWhite)
            {
                piece.CalculatePossibleMoves();

                if (piece.possibleMoves.Contains(kingPosition))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private ChessPiece[,] CloneBoardState()
    {
        ChessPiece[,] clone = new ChessPiece[8, 8];

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                clone[x, y] = ChessBoardPlacementHandler.Instance.GetPieceAt(new Vector2Int(x, y));
            }
        }

        return clone;
    }

    private void RestoreBoardState(ChessPiece[,] originalBoardState)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece piece = originalBoardState[x, y];
                Vector2Int pos = new Vector2Int(x, y);

                if (piece != null)
                {
                    piece.placementHandler.SetPosition(pos);
                }
                else
                {
                    ChessPiece currentPiece = ChessBoardPlacementHandler.Instance.GetPieceAt(pos);

                    if (currentPiece != null)
                    {
                        currentPiece.placementHandler.SetPosition(pos);
                    }
                }
            }
        }
    }
}