using System;
using UnityEngine;

namespace Chess.Scripts.Core
{
    public class ChessPlayerPlacementHandler : MonoBehaviour
    {
        [Range(0, 7)] [SerializeField] private int row, column;
        
        public delegate void PositionChanged(ChessPiece piece);

        public event PositionChanged OnPositionChanged;
        private Vector3 _oldPosition, _newPosition;

        private void Start()
        {
            transform.position = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;
            _oldPosition = transform.position;
            _newPosition = transform.position;
        }

        private void Update()
        {
            _newPosition = ChessBoardPlacementHandler.Instance.GetTile(row, column).transform.position;

            if (_oldPosition != _newPosition)
            {
                OnPositionChanged?.Invoke(GetComponent<ChessPiece>());
                transform.position = _newPosition;
                _oldPosition = _newPosition;
            }
        }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(row, column);
        }

        public Vector2Int SetPosition(Vector2Int position)
        {
            row = position.x;
            column = position.y;
            return new Vector2Int(row, column);
        }
    }
}