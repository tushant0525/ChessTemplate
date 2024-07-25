using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : ChessPiece
{
    // Overrides the base class method to calculate possible moves for the Bishop piece.
    public override void CalculatePossibleMoves()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();
        Vector2Int currentPosition = placementHandler.GetPosition();

        // Diagonal moves: four directions
        AddMovesInDirection(currentPosition, 1, 1);    // Up-Right
        AddMovesInDirection(currentPosition, 1, -1);   // Up-Left
        AddMovesInDirection(currentPosition, -1, 1);   // Down-Right
        AddMovesInDirection(currentPosition, -1, -1);  // Down-Left
    }

    // Adds possible moves in a given direction until an invalid board position or another piece is encountered.
    private void AddMovesInDirection(Vector2Int currentPosition, int xIncrement, int yIncrement)
    {
        Vector2Int newPosition = currentPosition;
        while (true)
        {
            // Increment the position in the given direction.
            newPosition = new Vector2Int(newPosition.x + xIncrement, newPosition.y + yIncrement);
            
            // Break if the new position is outside the board.
            if (!IsValidBoardPosition(newPosition))
                break;

            // Get the piece at the new position, if any.
            ChessPiece pieceAtNewPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(newPosition);
            
            // If there is no piece at the new position, add it to possible moves.
            if (pieceAtNewPosition == null)
            {
                possibleMoves.Add(newPosition);
            }
            else
            {
                // If the piece at the new position is of the opposite color, add it to possible and captured moves.
                if (pieceAtNewPosition.IsWhite != IsWhite)
                {
                    possibleMoves.Add(newPosition);
                    capturedMoves.Add(newPosition);
                }
                // Break as a piece is encountered, blocking further moves in this direction.
                break;
            }
        }
    }
}