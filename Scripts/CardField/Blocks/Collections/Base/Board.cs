using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeathFortUnoCard.Scripts.CardField.Blocks.Collections.Base
{
    [Serializable]
    public class Board<T, TK> : IEnumerable<Cell<T,TK>>
    {
        protected Cell<T, TK>[,] board;

        public int Width => board.GetLength(1);
        public int Height => board.GetLength(0);
        
        public Board(int size) : this(size, size) { }

        public Board(int height, int width)
        {
            if (width <= 1 || height <= 1)
            {
                Debug.LogError("Дано неверное значение ширины или высоты!");
                return;
            }

            board = new Cell<T, TK>[height, width];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    board[y, x] = new Cell<T, TK>();
                }
            }
        }

        public Cell<T, TK> this[int y, int x] => board[y, x];

        public Cell<T, TK> GetAt(int y, int x) => board[y, x];

        public void SetAt(int y, int x, T key, TK value)
            => board[y, x] = new Cell<T, TK> {Key = key, Value = value};

        public IEnumerator<Cell<T, TK>> GetEnumerator() => new BoardEnumerator(board);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected class BoardEnumerator : IEnumerator<Cell<T, TK>>
        {
            private Cell<T, TK>[,] _board;
            private int _y;
            private int _x;

            public Cell<T, TK> Current
            {
                get
                {
                    if (_y >= _board.GetLength(0) || _x >= _board.GetLength(1))
                    {
                        throw new InvalidOperationException();
                    }

                    return _board[_y, _x];
                }
            }

            object IEnumerator.Current => Current;

            public BoardEnumerator(Cell<T, TK>[,] board)
            {
                _board = board;
                Reset();
            }

            public bool MoveNext()
            {
                if (_x < _board.GetLength(1) - 1)
                {
                    _x++;
                    return true;
                }

                if (_y < _board.GetLength(0) - 1)
                {
                    _x = 0;
                    _y++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                _y = 0;
                _x = -1;
            }

            public void Dispose() => _board = null;
        }
    }
    
    
}