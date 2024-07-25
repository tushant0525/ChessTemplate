using System;
using System.Collections;
using System.Collections.Generic;
using Chess.Scripts.Core;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ChessPlayerPlacementHandler))]
public abstract class ChessPiece : MonoBehaviour
{
    public bool IsWhite = false;
    [HideInInspector] public ChessPlayerPlacementHandler placementHandler;
    public List<Vector2Int> possibleMoves = new List<Vector2Int>();
    public List<Vector2Int> capturedMoves = new List<Vector2Int>();

    private void Awake()
    {
        placementHandler = GetComponent<ChessPlayerPlacementHandler>();
    }

    public virtual void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
        possibleMoves.Clear();
    }

    public abstract void CalculatePossibleMoves();
}