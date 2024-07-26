using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override void CalculatePossibleMoves()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();
        Vector2Int currentPosition = placementHandler.GetPosition();

        // Add all horizontal and vertical moves (like a Rook)
        AddMovesInDirection(currentPosition, 1, 0); // Right
        AddMovesInDirection(currentPosition, -1, 0); // Left
        AddMovesInDirection(currentPosition, 0, 1); // Up
        AddMovesInDirection(currentPosition, 0, -1); // Down

        // Add all diagonal moves (like a Bishop)
        AddMovesInDirection(currentPosition, 1, 1); // Up-Right
        AddMovesInDirection(currentPosition, 1, -1); // Up-Left
        AddMovesInDirection(currentPosition, -1, 1); // Down-Right
        AddMovesInDirection(currentPosition, -1, -1); // Down-Left
    }


    private void AddMovesInDirection(Vector2Int currentPosition, int xIncrement, int yIncrement)
    {
        Vector2Int newPosition = currentPosition;

        // Continue moving in the specified direction until an invalid position or obstacle is encountered
        while (true)
        {
            newPosition = new Vector2Int(newPosition.x + xIncrement, newPosition.y + yIncrement);

            // Break the loop if the new position is outside the board boundaries
            if (!ChessBoardPlacementHandler.Instance.IsValidBoardPosition(newPosition))
                break;

            ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(newPosition);

            // If the new position is empty, add it to possible moves
            if (pieceAtNewPosition == null)
            {
                possibleMoves.Add(newPosition);
            }
            else
            {
                // If there is an opponent's piece at the new position, add it to possible and captured moves
                if (pieceAtNewPosition.IsWhite != IsWhite)
                {
                    possibleMoves.Add(newPosition);
                    capturedMoves.Add(newPosition);
                }

                // Stop further movement in this direction after encountering any piece
                break;
            }
        }
    }
}