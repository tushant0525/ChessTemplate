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
    }

    // Abstract method to calculate possible moves, to be implemented in derived classes
    public abstract void CalculatePossibleMoves();


    // Check if the pawn can capture an opponent piece at the given position
    protected bool CanCapture(Vector2Int position)
    {
        if (!ChessBoardPlacementHandler.Instance.IsValidBoardPosition(position))
            return false;

        ChessPiece pieceAtPosition = ChessBoardPlacementHandler.Instance.GetPieceAt(position);
        return pieceAtPosition != null && pieceAtPosition.IsWhite != IsWhite;
    }
}