// Based on  https://www.youtube.com/watch?v=ErHEZ5YGQ5M
//Thank you git-amend!
namespace Match3
{
    public class GridObject<T> 
    {
        GridSystem2D<GridObject<T>> _grid;
        int x;
        int y;
        T _value;
        public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y)
        {
            this._grid = grid;
            this.x = x;
            this.y = y;
        }
        public void SetValue(T value)
        {
            _value = value;
        }
        public T GetValue()
        {
            return _value;
        }
    }
        }
    
