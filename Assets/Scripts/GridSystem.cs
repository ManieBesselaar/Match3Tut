using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
// Based on  https://www.youtube.com/watch?v=ErHEZ5YGQ5M
//Thank you git-amend!
namespace Match3
{
    public class GridSystem2D<T>
    {
        int _width;
        int _height;
        float _cellSize;
        Vector3 _origin;
        T[,] _gridArray;

        CoordinateConverter _coordinateConverter;
        public Action<int, int, T> OnvalueChangeEvent;
        public static GridSystem2D<T> VerticalGrid(int width, int height, float cellSize, Vector3 origin,
            bool debug)
        {
          
            return new GridSystem2D<T>(width, height, cellSize, origin,  new VerticalConverter(), debug);
        }
        public static GridSystem2D<T> HorizontalGrid(int width, int height, float cellSize, Vector3 origin,
            bool debug)
        {
            return  new GridSystem2D<T>(width, height, cellSize, origin, new HorizontalConverter(), debug);
        }
        public GridSystem2D(int width, int height, float cellSize,
            Vector3 origin, CoordinateConverter coordinateConverter, bool debug)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            _origin = origin;
            // the ?? is a null coalescing operator
            // it returns the left hand side if it is not null, otherwise it returns the right hand side
            // so if the coordinateConverter is null, it will create a new VerticalConverter
            _coordinateConverter = coordinateConverter ?? new VerticalConverter();
            _gridArray = new T[width, height];

            if (debug)
            {
                DrawDebugLines();
            }
        }
        /// <summary>
        /// Returns the grid position at the specified world position
        /// </summary>
        /// <param name="worldPosition">World position of cell to check</param>
        /// <returns></returns>
        public Vector2Int GetXY(Vector3 worldPosition)
        {
            return _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);
        }

        public void SetValue(Vector3 worldPosition, T value)
        {
            Vector2Int pos = _coordinateConverter.WorldToGrid(worldPosition, _cellSize, _origin);
            SetValue(pos.x, pos.y, value);
        }

        public void SetValue(int x, int y, T value)
        {
            if (IsValid(x, y))
            {
                _gridArray[x, y] = value;
                OnvalueChangeEvent?.Invoke(x, y, value);
            }
        }

        public T GetValue(Vector3 worldPosition)
        {
            Vector2Int pos = GetXY(worldPosition);
            return GetValue(pos.x, pos.y);
        }

        public T GetValue(int x, int y)
        {
            //Check if the position is valid, if it is not return the default value of T
            return IsValid(x, y) ? _gridArray[x, y] : default;
        }

        /// <summary>
        /// Check if the position is valid or if it is outside the grid
        /// </summary>
        /// <param name="x">row </param>
        /// <param name="y">columb</param>
        /// <returns></returns>
        bool IsValid(int x, int y) => x >= 0 && x < _width && y < _height && y >= 0;

        ///<summary>
        ///Draws debug lines for the grid
        /// </summary>
        void DrawDebugLines()
        {
            GameObject parent = new GameObject("GridSystem2D_Debug");
           
            const float duration = 100f;
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CreateWorldText(parent, x + "," + y, GetWorldPositonCenter(x, y), _coordinateConverter.Forward, 2);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, duration);
        }

        TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 dir,
            int fontSize, Color color = default, TextAlignmentOptions textAnchor = TextAlignmentOptions.Center,
            int sortOrder = 0)
        {
          
            GameObject gameObject = new GameObject("debug_Text" + text, typeof(TextMeshPro));
            gameObject.transform.SetParent(parent.transform);
            gameObject.transform.position = position;
            gameObject.transform.forward = dir;

            TextMeshPro textMeshPro = gameObject.GetComponent<TextMeshPro>();

            textMeshPro.text = text;
            textMeshPro.fontSize = fontSize;
            textMeshPro.color = color == default ? Color.white : color;
            textMeshPro.alignment = textAnchor;
            textMeshPro.GetComponent<MeshRenderer>().sortingOrder = sortOrder;
            return textMeshPro;
        }


        public Vector3 GetWorldPositonCenter(int x, int y)
        {
            return _coordinateConverter.GridToWorldCenter(x, y, _cellSize, _origin);
        }

        Vector3 GetWorldPosition(int x, int y)
        {
            return _coordinateConverter.GridToWorld(x, y, _cellSize, _origin);
        }

       

        public abstract class CoordinateConverter
        {
            public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
            public abstract Vector3 GetWorldCenter(int x, int y, float cellSize, Vector3 origin);
            public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);
            /// <summary>
            /// Gets the center in world position of the center of a cell on our grid.
            /// </summary>
            /// <param name="x">row of cell</param>
            /// <param name="y">colum of cell</param>
            /// <param name="cellSize">cell size</param>
            /// <param name="origin">origin of grid.</param>
            /// <returns></returns>
            public abstract Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin);
            

            public abstract Vector3 Forward { get; }
        }
        /// <summary>
        /// Coordinate converter for 2D vertical grid
        /// </summary>
       public class VerticalConverter : CoordinateConverter
        {
            /// <summary>
            /// Convert grid coordinate to world coordinate
            /// </summary>
            /// <param name="x">x on grid</param>
            /// <param name="y">y on grid</param>
            /// <param name="cellSize">size of cells</param>
            /// <param name="origin">origin of the grid</param>
            /// <returns></returns>
            public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x, y, 0) * cellSize + origin;
            }
            public override Vector3 GetWorldCenter(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x * cellSize + cellSize * .5f, y * cellSize + cellSize * .5f, 0) + origin;
            }
            public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x * cellSize + cellSize * .5f, y * cellSize + cellSize * .5f, 0) + origin;
            }
            public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
            {
                Vector3 gridPosition = (worldPosition - origin) / cellSize;
                int x = Mathf.FloorToInt(gridPosition.x);
                int y = Mathf.FloorToInt(gridPosition.y);

                return new Vector2Int(x, y);
            }
            public override Vector3 Forward => Vector3.forward;
        }
        public class HorizontalConverter : CoordinateConverter
        {
            /// <summary>
            /// Convert grid coordinate to world coordinate
            /// </summary>
            /// <param name="x">x on grid</param>
            /// <param name="y">y on grid</param>
            /// <param name="cellSize">size of cells</param>
            /// <param name="origin">origin of the grid</param>
            /// <returns></returns>
            public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x, 0, y) * cellSize + origin;
            }
            public override Vector3 GetWorldCenter(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x * cellSize + cellSize * .5f,0, y * cellSize + cellSize * .5f) + origin;
            }
            public override Vector3 GridToWorldCenter(int x, int y, float cellSize, Vector3 origin)
            {
                return new Vector3(x * cellSize + cellSize * .5f,0, y * cellSize + cellSize * .5f) + origin;
            }
            public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin)
            {
                Vector3 gridPosition = (worldPosition - origin) / cellSize;
                int x = Mathf.FloorToInt(gridPosition.x);
                int y = Mathf.FloorToInt(gridPosition.z);

                return new Vector2Int(x, y);
            }
            public override Vector3 Forward => -Vector3.up;
        }

    }
}


     
      
    
