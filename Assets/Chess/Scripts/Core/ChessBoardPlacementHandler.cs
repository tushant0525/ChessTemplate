using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using Chess.Scripts.Core;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class ChessBoardPlacementHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _rowsArray;
    [SerializeField] private GameObject _highlightPrefab;

    private GameObject[,] _chessBoard;
    private ChessPiece[,] _chessPieces;

    internal static ChessBoardPlacementHandler Instance;

    private ChessPiece selectedPiece;
    private ChessPlayerPlacementHandler[] _availableChessPieces;
    [SerializeField] private bool isWhiteTurn = true; // Assuming white starts first

    private void Awake()
    {
        Instance = this;
        _availableChessPieces = new ChessPlayerPlacementHandler[16];
        _availableChessPieces = FindObjectsOfType<ChessPlayerPlacementHandler>();
        _chessPieces = new ChessPiece[8, 8];
        GenerateArray();
        InitializeChessPieces();
    }

    private void OnEnable()
    {
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            chessPiece.OnPositionChanged += UpdatePiecePosition;
        }
    }

    private void OnDisable()
    {
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            chessPiece.OnPositionChanged -= UpdatePiecePosition;
        }
    }

    private void UpdatePiecePosition(ChessPiece piece)
    {
        ClearHighlights();
        InitializeChessPieces();
        HighlightPossibleMoves(piece);
    }

    private void GenerateArray()
    {
        _chessBoard = new GameObject[8, 8];

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                _chessBoard[i, j] = _rowsArray[i].transform.GetChild(j).gameObject;
                // Debug.Log(" Chess piece at "+i+","+j+" is "+_chessBoard[i, j].name);
            }
        }
    }

    private void InitializeChessPieces()
    {
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            _chessPieces[chessPiece.GetPosition().x, chessPiece.GetPosition().y] =
                chessPiece.transform.GetComponent<ChessPiece>();
        }
    }

    internal GameObject GetTile(int i, int j)
    {
        try
        {
            return _chessBoard[i, j];
        }
        catch (Exception)
        {
            Debug.LogError("Invalid row or column.");
            return null;
        }
    }

    internal void Highlight(int row, int col)
    {
        var tile = GetTile(row, col).transform;

        if (tile == null)
        {
            Debug.LogError("Invalid row or column.");
            return;
        }

        GameObject highlightedTile =
            Instantiate(_highlightPrefab, tile.transform.position, Quaternion.identity, tile.transform);

        if (isWhiteTurn)
        {
            highlightedTile.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else
        {
            highlightedTile.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

    internal void ClearHighlights()
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var tile = GetTile(i, j);
                if (tile.transform.childCount <= 0) continue;

                foreach (Transform childTransform in tile.transform)
                {
                    Destroy(childTransform.gameObject);
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            ChessPiece clickedPiece = hit.transform.GetComponent<ChessPiece>();
        

            if (clickedPiece != null)
            {
                
                if (isWhiteTurn && clickedPiece.IsWhite)
                {
                    SelectPiece(clickedPiece);
                }
                else if (!isWhiteTurn && !clickedPiece.IsWhite)
                {
                    SelectPiece(clickedPiece);
                }
                else
                {
                    Debug.Log("Invalid move.");
                }
            }
            else
            {
                Vector2Int clickedPosition = GetTilePosition(hit.transform.position);

                if (selectedPiece != null)
                {
                    TryMovePiece(clickedPosition);
                }
            }
        }
    }

    private void SelectPiece(ChessPiece piece)
    {
        selectedPiece = piece;
        ClearHighlights();
        HighlightPossibleMoves(piece);
    }

    private void TryMovePiece(Vector2Int targetPosition)
    {
        if (IsMoveValid(selectedPiece, targetPosition))
        {
            MovePiece(selectedPiece, targetPosition);
            ClearHighlights();
            selectedPiece = null;
            isWhiteTurn = !isWhiteTurn;
        }
    }


    private void HighlightPossibleMoves(ChessPiece piece)
    {
        List<Vector2Int> possibleMoves = piece.GetPossibleMoves();

        foreach (Vector2Int move in possibleMoves)
        {
            Highlight(move.x, move.y);
        }
    }

    private bool IsMoveValid(ChessPiece piece, Vector2Int targetPosition)
    {
        List<Vector2Int> possibleMoves = piece.possibleMoves;
        return possibleMoves.Contains(targetPosition);
    }

    private void MovePiece(ChessPiece piece, Vector2Int targetPosition)
    {
        Vector2Int currentPosition = GetCellPosition(piece);
        _chessPieces[currentPosition.x, currentPosition.y] = null;
        _chessPieces[targetPosition.x, targetPosition.y] = piece;

        Vector3 worldPosition = _chessBoard[targetPosition.x, targetPosition.y].transform.position;
        piece.Move(worldPosition);
    }

    public Vector2Int GetCellPosition(ChessPiece piece)
    {
        return piece._placementHandler.GetPosition();
    }

    public Vector2 GetTilePosition(Vector2Int position)
    {
        return new Vector2(_chessBoard[position.x, position.y].transform.position.x,
            _chessBoard[position.x, position.y].transform.position.y);
    }

    public Vector2Int GetTilePosition(Vector3 position)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (_chessBoard[i, j].transform.position == position)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return new Vector2Int(-1, -1);
    }

    // This method allows ChessPiece to get information about the board
    public ChessPiece GetPieceAt(Vector2Int position)
    {
        foreach (var chessPiece in _availableChessPieces)
        {
            if (position.x == chessPiece.GetPosition().x && position.y == chessPiece.GetPosition().y)
            {
                return _chessPieces[position.x, position.y];
            }
        }

        return null;
    }

    #region Highlight Testing

    // private void Start() {
    //     StartCoroutine(Testing());
    // }

    // private IEnumerator Testing() {
    //     Highlight(2, 7);
    //     yield return new WaitForSeconds(1f);
    //
    //     ClearHighlights();
    //     Highlight(2, 7);
    //     Highlight(2, 6);
    //     Highlight(2, 5);
    //     Highlight(2, 4);
    //     yield return new WaitForSeconds(1f);
    //
    //     ClearHighlights();
    //     Highlight(7, 7);
    //     Highlight(2, 7);
    //     yield return new WaitForSeconds(1f);
    // }

    #endregion
}