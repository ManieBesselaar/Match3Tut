using Match3;
using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    GridSystem2D<GridObject<Gem>> _grid;
    List<Vector2Int> _matches;
    [SerializeField] Pattern[] _patterns;
    private int _searchPostitionX;
    private int _searchPositionY;
    private List<List<Vector2Int>> _foundShapes;
    List<Vector2Int> _result = new List<Vector2Int>();
    int _gridHeight, _gridWidth;
    GridObject<Gem> gridObject;

    Gem gem;
    Gemtype foundGemtype;
    Gemtype _targetGemType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Init(GridSystem2D<GridObject<Gem>> grid) 
    {
        _matches = new List<Vector2Int>();
        _foundShapes = new List<List<Vector2Int>>();
        _grid = grid;
        _gridHeight = grid.GetHeight();
        _gridWidth = grid.GetWidth();

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    //public List<Vector2Int> FindMatches()
    //{
    //    _matches.Clear();


    //}

    [ContextMenu("FindAllMatches")]
    public List<List<Vector2Int>> FindAllMatches()
    {
        ///TODO: Find only 1 shape and return, let gems explode. Call this again from game until all matches gone.
        _foundShapes.Clear();
        foreach (Pattern shape in _patterns)
        {
            Debug.Log("Searchfor " + shape.Name);
            _searchPostitionX = 0;
            _searchPositionY = 0;
            int rows = _gridHeight - shape.Height;
            int columns = _gridWidth - shape.Width;
            Debug.Log($"count to {_gridWidth * _gridHeight}");
            for (int i = 0; i < _gridWidth * _gridHeight; i++)
            {
                 
               
                Debug.Log($"Poscount {_searchPostitionX} , {_searchPositionY} i= {i}");
                if (_searchPositionY > _gridHeight - shape.Height)
                {
                    Debug.Log($"foundShapes {_foundShapes.Count} shapes");
                    return _foundShapes;
                }
                    MoveSearchGrid(shape);

                _searchPostitionX++;
                if (_searchPostitionX > columns) 
                {
                    _searchPostitionX = 0;
                    _searchPositionY++;
                }

                
            }
        }
        Debug.Log($"foundShapes {_foundShapes.Count} shapes");
        return _foundShapes;
    }

    public List<Vector2Int> ProcessMatchList()
    {
        List<Vector2Int> returnList = new List<Vector2Int>();
        foreach (Pattern shape in _patterns)
        {
            returnList = FindMatch(shape);
            if ( returnList.Count > 0 ) return returnList;
        }
    return returnList;
    }

    [ContextMenu("Pop single shape")]
    public void PopSingle()
    {
        MoveSearchGrid(_patterns[0]);
    }

    public List<Vector2Int> FindMatch(Pattern shape)
    ///TODO: Find only 1 shape and return, let gems explode. Call this again from game until all matches gone.

    { 
           
            _searchPostitionX = 0;
            _searchPositionY = 0;
            int rows = _gridHeight - shape.Height;
            int columns = _gridWidth - shape.Width;
           
            for (int i = 0; i < _gridWidth * _gridHeight; i++)
            {


               // Debug.Log($"Poscount {_searchPostitionX} , {_searchPositionY} i= {i}");
                if (_searchPositionY > _gridHeight - shape.Height)
                {
                    //All areas covered, nothing found
                    return new List<Vector2Int>();
                }
              var foundShape =  MoveSearchGrid(shape);
            if (foundShape != null) return foundShape;
                _searchPostitionX++;
                if (_searchPostitionX > columns)
                {
                    _searchPostitionX = 0;
                    _searchPositionY++;
                }


            
        }
        return new List<Vector2Int>();
    }


    [ContextMenu("MoveSearchGrid")]
   List< Vector2Int> MoveSearchGrid(Pattern shape)

    {
       
        _result.Clear();
        try
        {
            _targetGemType = _grid.GetValue(_searchPostitionX + shape.Positions[0].x, _searchPositionY + shape.Positions[0].y).GetValue().GetGemType();
        }
        catch
        {
            Debug.LogError($"null at {_searchPostitionX} {_searchPositionY} ");
        }
        

        for (int i = 0; i < shape.Positions.Length; i++)
        {
            gridObject = null;

            gem = null;
            foundGemtype = null;

            int x = shape.Positions[i].x + _searchPostitionX;
            int y = shape.Positions[i].y + _searchPositionY;
             gridObject = _grid.GetValue(x, y);

          
            if (gridObject != null) 
            {
                gem = gridObject.GetValue();

                foundGemtype = gem?.GetGemType();
                //
                
            }
           
          
            if (foundGemtype == _targetGemType)
            {
               
                Debug.Log($"GemMatching found {_result.Count}");
                _result.Add(new Vector2Int(x, y));
              
            }
            else
            {
                Debug.Log("GemMatching not found");
               
            }
                

        }
        
   //  _result.Sort();
        if (shape.Positions.Length == _result.Count)
        {
            Debug.Log($"GemMatching Found {shape}");
            Debug.Log($"ShapeFound {shape.Name}");
           
            return _result;
          
           // return true;
        }
        else
        {
            return null;
        }

       
    }
    bool CompareLists(List<Vector2Int> shape, List<Vector2Int> result)
    {
        Debug.Log("result count " + result.Count);
        if (shape.Count != result.Count)
        {
            return false;
        }
        /* for (int i = 0; i < list1.Count; i++)
         {
             if (list1[i] != list2[i])
             {
                 return false;
             }
         }*/
        _foundShapes.Add(result);
        return true;
    }
}
