using System;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        [Range(0, 7)] [SerializeField] private int row, column;

        // Delegate for position change event
        public delegate void PositionChanged(ChessPiece piece);
        
        // Event triggered when position changes
        public event PositionChanged OnPositionChanged;
        
        private Vector3 _oldPosition, _newPosition;

        private void Start()
        {
            // Initialize position on the chessboard
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
            _oldPosition = transform.position;
            _newPosition = transform.position;
        }

        private void Update()
        {
            // Update the new position based on current row and column
            _newPosition = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;

            // Check if the position has changed
            if (_oldPosition != _newPosition)
            {
                // Trigger the position changed event
                OnPositionChanged?.Invoke(GetComponent<ChessPiece>());
                
                // Update the transform position and old position
                transform.position = _newPosition;
                _oldPosition = _newPosition;
            }
        }

        // Returns the current position as a Vector2Int
        public Vector2Int GetPosition()
        {
            return new Vector2Int(row, column);
        }

        // Sets the new position and returns it as a Vector2Int
        public Vector2Int SetPosition(Vector2Int position)
        {
            row = position.x;
            column = position.y;
            return new Vector2Int(row, column);
        }
    }
}