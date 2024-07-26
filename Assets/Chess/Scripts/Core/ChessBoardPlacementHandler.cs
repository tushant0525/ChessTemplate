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
    [SerializeField] private bool isWhiteTurn = true; // Assuming white starts first

    private GameObject[,] _chessBoard;
    private ChessPiece[,] _chessPieces;

    internal static ChessBoardPlacementHandler Instance { get; private set; }

    private ChessPiece selectedPiece;
    private List<ChessPlayerPlacementHandler> _availableChessPieces;

    private LayerMask whitePiecesLayer;
    private LayerMask blackPiecesLayer;
    private LayerMask emptyCellsLayer;

    private InputMap _inputMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _inputMap = new InputMap();
        _availableChessPieces = new List<ChessPlayerPlacementHandler>(FindObjectsOfType<ChessPlayerPlacementHandler>());
        whitePiecesLayer = LayerMask.GetMask("WhitePieces");
        blackPiecesLayer = LayerMask.GetMask("BlackPieces");
        emptyCellsLayer = LayerMask.GetMask("Tiles");
        UIManager.Instance.SetTurnText(isWhiteTurn);
        GenerateArray();
        InitializeChessPieces();
    }

    private void OnEnable()
    {
        _inputMap.Enable();
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            chessPiece.OnPositionChanged += UpdatePiecePosition;
        }
    }

    private void OnDisable()
    {
        _inputMap.Disable();
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            chessPiece.OnPositionChanged -= UpdatePiecePosition;
        }
    }

    // Updates the position of the piece when it changes
    private void UpdatePiecePosition(ChessPiece piece)
    {
        ClearHighlights();
        InitializeChessPieces();
    }

    // Generates the chessboard array
    private void GenerateArray()
    {
        _chessBoard = new GameObject[8, 8];

        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                _chessBoard[i, j] = _rowsArray[i].transform.GetChild(j).gameObject;
            }
        }
    }

    // Initializes the chess pieces on the board
    private void InitializeChessPieces()
    {
        _chessPieces = new ChessPiece[8, 8];

        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            _chessPieces[chessPiece.GetPosition().x, chessPiece.GetPosition().y] =
                chessPiece.transform.GetComponent<ChessPiece>();
        }
    }

    // Returns the tile at the specified position
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

    // Highlights the tile at the specified position with the selected color
    internal void Highlight(int row, int col, Color selecteColor)
    {
        var tile = GetTile(row, col).transform;

        if (tile == null)
        {
            Debug.LogError("Invalid row or column.");
            return;
        }

        GameObject highlightedTile =
            Instantiate(_highlightPrefab, tile.transform.position, Quaternion.identity, tile.transform);
        highlightedTile.GetComponent<SpriteRenderer>().color = selecteColor;
    }

    // Clears all highlights from the board
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
        if (_inputMap.Player.Touch.IsPressed())
        {
            HandleMouseClick();
        }
    }

    // Handles mouse click events to select or move pieces
    private void HandleMouseClick()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LayerMask layerMask = isWhiteTurn ? (whitePiecesLayer | emptyCellsLayer) : (blackPiecesLayer | emptyCellsLayer);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            ChessPiece clickedPiece = hit.transform.GetComponent<ChessPiece>();

            if (clickedPiece != null && selectedPiece != clickedPiece)
            {
                if ((isWhiteTurn && clickedPiece.IsWhite) || (!isWhiteTurn && !clickedPiece.IsWhite))
                {
                    SelectPiece(clickedPiece);
                }
                else
                {
                    Debug.Log("Invalid move while selecting piece.");
                }
            }
            else if (selectedPiece != null)
            {
                Vector2Int clickedPosition = GetTilePosition(hit.transform.position);

                if (selectedPiece.possibleMoves.Contains(clickedPosition))
                {
                    TryMovePiece(clickedPosition);
                }
                else
                {
                    Debug.Log("Invalid move while moving piece.");
                }
            }
        }
    }

    // Selects the specified piece and highlights possible moves
    private void SelectPiece(ChessPiece piece)
    {
        selectedPiece = piece;
        selectedPiece.CalculatePossibleMoves();
        ClearHighlights();
        HighlightPossibleMoves(piece);
    }

    // Tries to move the selected piece to the target position
    private void TryMovePiece(Vector2Int targetPosition)
    {
        if (IsMoveValid(selectedPiece, targetPosition))
        {
            MovePiece(selectedPiece, targetPosition);
            ClearHighlights();
            selectedPiece = null;
            isWhiteTurn = !isWhiteTurn;
            UIManager.Instance.SetTurnText(isWhiteTurn);
        }
    }

    // Highlights the possible moves for the specified piece
    private void HighlightPossibleMoves(ChessPiece piece)
    {
        List<Vector2Int> possibleMoves = piece.possibleMoves;

        foreach (Vector2Int move in possibleMoves)
        {
            Highlight(move.x, move.y, Color.green);
        }

        foreach (Vector2Int move in piece.capturedMoves)
        {
            Highlight(move.x, move.y, Color.red);
        }
    }

    // Checks if the move to the target position is valid
    private bool IsMoveValid(ChessPiece piece, Vector2Int targetPosition)
    {
        List<Vector2Int> possibleMoves = piece.possibleMoves;
        return possibleMoves.Contains(targetPosition);
    }

    // Moves the piece to the target position
    private void MovePiece(ChessPiece piece, Vector2Int targetPosition)
    {
        if (CheckCapturedMoves(piece, targetPosition))
        {
            Destroy(_chessPieces[targetPosition.x, targetPosition.y].gameObject);
            _availableChessPieces.Remove(_chessPieces[targetPosition.x, targetPosition.y].placementHandler);
        }

        Vector2Int currentPosition = GetCellPosition(piece);
        piece.placementHandler.SetPosition(targetPosition);
        _chessPieces[currentPosition.x, currentPosition.y] = null;
        _chessPieces[targetPosition.x, targetPosition.y] = piece;

        Vector3 worldPosition = _chessBoard[targetPosition.x, targetPosition.y].transform.position;
        piece.Move(worldPosition);
    }

    // Returns the cell position of the specified piece
    public Vector2Int GetCellPosition(ChessPiece piece)
    {
        return piece.placementHandler.GetPosition();
    }

    // Returns the tile position for the specified world position
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

    // Returns the chess piece at the specified position
    public ChessPiece GetPieceAt(Vector2Int position)
    {
        foreach (ChessPlayerPlacementHandler chessPiece in _availableChessPieces)
        {
            if (position.x == chessPiece.GetPosition().x && position.y == chessPiece.GetPosition().y)
            {
                return _chessPieces[position.x, position.y];
            }
        }

        return null;
    }

    // Check if the position is within the bounds of the board
    public bool IsValidBoardPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }


    public void RemovePieceAt(Vector2Int position)
    {
        if (IsValidBoardPosition(position))
        {
            _chessPieces[position.x, position.y] = null;
        }
    }

    public void PlacePieceAt(ChessPiece piece, Vector2Int position)
    {
        if (IsValidBoardPosition(position))
        {
            _chessPieces[position.x, position.y] = piece;
            piece.placementHandler.SetPosition(position);
        }
    }

    // Checks if the move to the target position captures an opponent's piece
    private bool CheckCapturedMoves(ChessPiece piece, Vector2Int targetPosition)
    {
        List<Vector2Int> capturedMoves = piece.capturedMoves;
        return capturedMoves.Contains(targetPosition);
    }

    // Returns all chess pieces currently on the board as a List
    public List<ChessPiece> GetAllPieces()
    {
        List<ChessPiece> activePieces = new List<ChessPiece>();

        foreach (ChessPiece chessPiece in _chessPieces)
        {
            if (chessPiece != null)
            {
                activePieces.Add(chessPiece);
            }
        }

        return activePieces;
    }

    public void PromotePawn(string name, Sprite availablePromotionSprite, ChessPiece pawnToPromote)
    {
        pawnToPromote.GetComponent<SpriteRenderer>().sprite = availablePromotionSprite;
        pawnToPromote.gameObject.name = name;
        ChessPiece piece = null;

        if (name == "Queen")
        {
            piece
                = pawnToPromote.gameObject.AddComponent<Queen>();
        }
        else if (name == "Rook")
        {
            piece = pawnToPromote.gameObject.AddComponent<Rook>();
        }
        else if (name == "Bishop")
        {
            piece = pawnToPromote.gameObject.AddComponent<Bishop>();
        }
        else if (name == "Knight")
        {
            piece = pawnToPromote.gameObject.AddComponent<Knight>();
        }

        piece.IsWhite = pawnToPromote.IsWhite;
        Destroy(pawnToPromote.GetComponent<Pawn>());
        //Add script of type of piece it is promoted to
        //Remove script of pawn
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