using System.Collections.Generic;
using Chess.Scripts.Core;
using UnityEngine;

[RequireComponent(typeof(ChessPlayerPlacementHandler))]
public abstract class ChessPiece : MonoBehaviour
{
    [HideInInspector] public ChessPlayerPlacementHandler placementHandler;
    public bool IsWhite = false;
    public List<Vector2Int> possibleMoves = new List<Vector2Int>();
    public List<Vector2Int> capturedMoves = new List<Vector2Int>();

    private void Awake()
    {
        // Initialize the placement handler
        placementHandler = GetComponent<ChessPlayerPlacementHandler>();
    }

    // Move the piece to the new position and clear possible moves
    public virtual void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
        possibleMoves.Clear();
    }

    // Abstract method to calculate possible moves, to be implemented in derived classes
    public abstract void CalculatePossibleMoves();

    // Check if the position is within the bounds of the board
    protected bool IsValidBoardPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }

    // Check if the pawn can capture an opponent piece at the given position
    protected bool CanCapture(Vector2Int position)
    {
        if (!IsValidBoardPosition(position))
            return false;

        ChessPiece pieceAtPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(position);
        return pieceAtPosition != null && pieceAtPosition.IsWhite != IsWhite;
    }
}