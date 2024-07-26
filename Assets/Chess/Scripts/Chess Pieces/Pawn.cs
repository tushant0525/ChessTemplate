using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    private bool hasMoved = false;

    // Override to calculate the possible moves for the pawn
    public override void CalculatePossibleMoves()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();
        Vector2Int currentPosition = placementHandler.GetPosition();

        // Check if the pawn has moved from its initial position
        if ((!IsWhite && currentPosition.x != 1) || (IsWhite && currentPosition.x != 6))
        {
            hasMoved = true;
        }

        int direction = IsWhite ? -1 : 1; // White pawns move down, black pawns move up

        // Calculate forward move
        Vector2Int forwardMove = new Vector2Int(currentPosition.x + direction, currentPosition.y);

        if (ChessBoardPlacementHandler.Instance.IsValidBoardPosition(forwardMove) &&
            ChessBoardPlacementHandler.Instance.GetPieceAt(forwardMove) == null)
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

        // Calculate capture moves
        Vector2Int leftCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y - 1);
        Vector2Int rightCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y + 1);

        if (CanCapture(leftCapture))
        {
            possibleMoves.Add(leftCapture);
            capturedMoves.Add(leftCapture);
        }

        if (CanCapture(rightCapture))
        {
            possibleMoves.Add(rightCapture);
            capturedMoves.Add(rightCapture);
        }
    }

    public void CalculatePossibleMovesWithDiagonals()
    {
        possibleMoves.Clear();
        capturedMoves.Clear();
        Vector2Int currentPosition = placementHandler.GetPosition();

        // Check if the pawn has moved from its initial position
        if ((!IsWhite && currentPosition.x != 1) || (IsWhite && currentPosition.x != 6))
        {
            hasMoved = true;
        }

        int direction = IsWhite ? -1 : 1; // White pawns move down, black pawns move up

        // Calculate capture moves
        Vector2Int leftCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y - 1);
        Vector2Int rightCapture = new Vector2Int(currentPosition.x + direction, currentPosition.y + 1);


        possibleMoves.Add(leftCapture);
        possibleMoves.Add(rightCapture);
    }

    // Override to move the pawn and update its state
    public override void Move(Vector3 newPosition)
    {
        base.Move(newPosition);
        hasMoved = true;

        //Write pawn promotion code when it reaches end
        if ((!IsWhite && placementHandler.GetPosition().x == 7) || (IsWhite && placementHandler.GetPosition().x == 0))
        {
            UIManager.Instance.ShowPawnPromotionPanel(IsWhite, this);
        }
    }
}