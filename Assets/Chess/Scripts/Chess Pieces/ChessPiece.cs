using System;
using System.Collections;
using System.Collections.Generic;
using Chess.Scripts.Core;
using UnityEngine;

[RequireComponent(typeof(ChessPlayerPlacementHandler))]
public abstract class ChessPiece : MonoBehaviour
{
    public bool IsWhite = false;
    [HideInInspector] public ChessPlayerPlacementHandler _placementHandler;
    public List<Vector2Int> possibleMoves = new List<Vector2Int>();

    private void Awake()
    {
        _placementHandler = GetComponent<ChessPlayerPlacementHandler>();
    }

    public virtual void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public abstract List<Vector2Int> GetPossibleMoves();
}