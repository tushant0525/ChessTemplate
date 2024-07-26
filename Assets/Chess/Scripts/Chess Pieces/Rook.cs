using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    
    public override void CalculatePossibleMoves()
    {
        // Clear previous possible and captured moves
        possibleMoves.Clear();
        capturedMoves.Clear();

        // Get the current position of the Rook
        Vector2Int currentPosition = placementHandler.GetPosition();

        // Add all horizontal and vertical moves
        AddMovesInDirection(currentPosition, 1, 0);  // Move Up
        AddMovesInDirection(currentPosition, -1, 0); // Move Down
        AddMovesInDirection(currentPosition, 0, 1);  // Move Right
        AddMovesInDirection(currentPosition, 0, -1); // Move Left
    }
    
    private void AddMovesInDirection(Vector2Int currentPosition, int xIncrement, int yIncrement)
    {
        // Start from the current position and move in the specified direction
        Vector2Int newPosition = currentPosition;

        while (true)
        {
            // Update the position in the specified direction
            newPosition = new Vector2Int(newPosition.x + xIncrement, newPosition.y + yIncrement);

            // Break the loop if the new position is outside the board boundaries
            if (!ChessBoardPlacementHandler.Instance.IsValidBoardPosition(newPosition))
                break;

            // Get the ChessPiece at the new position
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