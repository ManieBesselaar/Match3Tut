using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestGrid : MonoBehaviour

{
    [SerializeField] GameObject _spritePrefab;
    GameObject[,] _grid = new GameObject[10, 10];
    int _postition = 0;
    List<int> _postitions = new List<int>() { 0, 1, 11, 21, 15,24,50,30,31,40,51, 27,28,29,38,48,61,71,80,81,82};

   [SerializeField] Pattern[] _patterns;
    
    List<int> _result = new List<int>();
    List<List<int>> _foundShapes = new List<List<int>>();
    List<List<int>> _allShapes = new List<List<int>>();
    int _searchPostitionX = 0,_searchPositionY = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Pattern pattern in _patterns)
        {
            List<int> shape = new List<int>();
            for (int i = 0; i < pattern.Positions.Length; i++)
            {
             //TODO: fix   shape.Add(pattern.Positions[i]);
            }
            _allShapes.Add(shape);
        }
       // _allShapes.Add(_tShape);
       // _allShapes.Add(_tShapeI);
      //  _allShapes.Add(_tLShape);
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                _grid[x, y] = Instantiate(_spritePrefab, new Vector3(x, y, 0), Quaternion.identity);

                if (_postitions.Contains(x + 10 * y))
                {
                    _grid[x, y].GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }

      
    }
    [ContextMenu("FindAllMatches")]
    private void FindAllMatches()
    {
        foreach (Pattern shape in _patterns)
        {
            _searchPostitionX = 0;
            _searchPositionY = 0;
            for (int i = 0; i < _grid.Length; i++)
            {
                MoveSearchGrid(shape);
            }
        }
            Debug.Log($"I found {_foundShapes.Count} shapes");
    }

    // Update is called once per frame
    [ContextMenu("UpdateGrid")]
    void UpdateGrid()
    {
        for(int i = 0;i <_postitions.Count; i++)
        {
            _postitions[i]++;;

            
           
        }
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (_postitions.Contains(x + 10 * y))
                {
                    _grid[x, y].GetComponent<SpriteRenderer>().color = Color.red;
                }
                else
                {
                    _grid[x, y].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
    [ContextMenu("MoveSearchGrid")]
    bool MoveSearchGrid(Pattern shape)

    {
        _result.Clear();
       
        if (_searchPostitionX > 10 - shape.Width)
        {
            _searchPostitionX = 0;
            _searchPositionY++;
        }
        if (_searchPositionY > 10 - shape.Height) return false;


        for (int i= 0; i< shape.Positions.Length;i++)
        {
            //TODO: fix   int x = shape.Positions[i] % shape.Width + _searchPostitionX;
            //TODO: fix   int y =shape.Positions[ i] / shape.Height + _searchPositionY;

            //TODO: fix   if (_grid[x, y].GetComponent<SpriteRenderer>().color == Color.red)
            //TODO: fix        _result.Add(x + 10 * y);
            // _grid[x, y].GetComponent<SpriteRenderer>().color = Color.blue;

        }
        /*   for (int x = _searchPostitionX; x < _searchPostitionX + shape.Width; x++)
           {
               for (int y = _searchPositionY; y < _searchPositionY + searchHeight; y++)
               {

                      if( _grid[x, y].GetComponent<SpriteRenderer>().color == Color.red)
                       _result.Add(x + 10 * y);

               }
           }*/
        _result.Sort();
        /*TODO:Fix
        if (CompareLists(new List<Vector2Int> (shape.Positions), _result ))
        {
            Debug.Log("Found");
            foreach (int i in _result)
            {
                _grid[i % 10, i / 10].GetComponent<SpriteRenderer>().color = Color.blue;
            }
            return true;
        }
        else
        {
            Debug.Log("Not Found");
        }
        _searchPostitionX++;
        */
        return false;
        
    }
    [ContextMenu("ResetGrid")]
    void ResetGrid()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                    _grid[x, y].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    bool CompareLists(List<int> shape, List<int> result )
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

