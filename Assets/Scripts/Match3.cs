using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UIElements;

namespace Match3
{
    public class Match3 : MonoBehaviour
    {
        [SerializeField] int _width = 8;
        [SerializeField] int _height = 8;
        [SerializeField] float _cellSize = 2f;
        [SerializeField] Vector3 _originPosition = Vector3.zero;
        [SerializeField] bool _debug = true;
        [SerializeField] Gem _gemPrefab;
        [SerializeField] Gemtype[] _gemTypes;
        [SerializeField] Ease _ease;
        [SerializeField]
       GameObject[] _explosionFX;
        [SerializeField] CamShaker _camShaker;
        InputReader _inputReader;
        Quaternion rotation;
        GridSystem2D<GridObject<Gem>> _grid;
       Vector2Int _selectedGem = Vector2Int.one *-1;
        enum RotationType
        {
            Horizontal,
            Vertical
        }

        [SerializeField] RotationType _rotationType;
        AudioManager _audioManager;
        private void Awake()
        {
            _inputReader = GetComponent<InputReader>();
            _audioManager = GetComponent<AudioManager>();
        }
        private void Start()
        {
          
           _inputReader.Fire += OnSelectGem;
            InitializeGrid();
        }

        private void OnSelectGem()
        {
            var gridpos = _grid.GetXY(Camera.main.ScreenToWorldPoint( _inputReader.Selected));

            //Validate position
           if(!isvalidPosition(gridpos) || IsemptyPosition(gridpos))
            {
                return;
            }
            if(_selectedGem == gridpos)
            {
                DeselectGem();
            }
            else if(_selectedGem == Vector2Int.one * -1)
            {
                SelectGem(gridpos);
            }
            else
            {
                StartCoroutine(RungameLoop(_selectedGem, gridpos));
            }
            

        }

        private bool IsemptyPosition(Vector2Int gridpos)
        {
            return _grid.GetValue(gridpos.x, gridpos.y) == null;
        }

        private bool isvalidPosition(Vector2Int gridpos)
        {
            //Two different ways of writing the same statement
          return  gridpos is {x: >=0 ,y: >=0 } && gridpos.x < _width && gridpos.y < _height;
         //   return gridpos.x >= 0 && gridpos.y >= 0 && gridpos.x < _width && gridpos.y < _height;
        }

        IEnumerator RungameLoop(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            
           yield return StartCoroutine(SwapGems(gridPosA, gridPosB));

            List<Vector2Int> matches = FindMatches();
            yield return StartCoroutine(ExplodeGems(matches));

            yield return StartCoroutine(MakeGemsFall());
            yield return StartCoroutine(ReplaceEmptySpots());
            DeselectGem();
            //Swap

        }
        IEnumerator MakeGemsFall()
        {
            for (var x = 0; x < _width; x++) 
            {
                for (var y = 0; y < _height; y++) 
                { 
                if(_grid.GetValue(x, y) == null)
                    {
                        for (var i=y +1;i < _height; i++)
                        {
                            if(_grid.GetValue(x,i) != null)
                            {
                                var gem = _grid.GetValue(x,i).GetValue();
                                _grid.SetValue(x,y,_grid.GetValue(x,i));
                                _grid.SetValue(x, i, null);
                               gem.transform.DOMove(_grid.GetWorldPositonCenter(x,y),.5f).SetEase(_ease);

                                yield return new WaitForSeconds(.1f);
                                break;
                            }
                        }
                    }
                }
            }
        }
        IEnumerator ReplaceEmptySpots()
        {

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    if (_grid.GetValue(x, y) == null)
                    {
                        CreateGem(x, y);
                        yield return new WaitForSeconds(.1f);
                    }
                }
            }
        }

     /*
      * This was my attempt
      * IEnumerator MakeGemsFall(List<Vector2Int> matches)
        {
            int xColumn = 0;
            int yMin = _height -1;
            bool fallThisRow = false;
            foreach (var match in matches)
            {

               if(match.x > xColumn)
                {
                    xColumn = match.x;
                    fallThisRow = false;
                    yMin = _height - 1;
                }
                if (match.y < yMin) 
                {
                yMin = match.y;
                fallThisRow = true;
                }
                for (int y = yMin; y < _height; y++)
                {
                    var gridObject = _grid.GetValue(xColumn, y);
                    if (gridObject != null)
                    {
                        var gem = gridObject.GetValue();
                        _grid.SetValue(xColumn, y, null);
                        gridObject.SetValue(null);
                        _grid.SetValue(xColumn, yMin,gridObject);
                        if (gem == null) continue;
                        gem.transform.DOMove(_grid.GetWorldPositonCenter(xColumn, yMin), .5f);
                        yMin++;
                    }
                }
            }

            for (int x = 0; x < _width; x++)
                {
                
                    if (_grid.GetValue(x, 0) != null)
                    { 
                    
                    }
           
            }
            yield return new WaitForSeconds(.5f);
        }
     */
        IEnumerator ExplodeGems(List<Vector2Int> matches)
        {
            foreach (var match in matches)
            {
            Gem gem = _grid.GetValue(match.x, match.y).GetValue();
                             
               _grid.SetValue(match.x, match.y, null);
                gem.transform.DOPunchScale(Vector3.one * 1.5f, .5f,1,.05f);
                GameObject explosion = Instantiate(_explosionFX[Random.Range(0, _explosionFX.Length)], gem.transform.position,Quaternion.Euler(90,0,0));
                _audioManager.PlayPop();
                _camShaker.ShakeCam();
                yield return new WaitForSeconds(.5f);
                //Spawn an explosion effect
                gem.DestroyGem();
                Destroy(explosion, .1f);
               
            }
            yield return new WaitForSeconds(.5f);
           // yield return StartCoroutine(ReplaceEmptySpots());
        }
        private List<Vector2Int> FindMatches()
        {
            //see https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.hashset-1?view=netstandard-2.1
            //and https://www.geeksforgeeks.org/hashset-in-c-sharp-with-examples/
            // Hashset is unordered collection of unique elements
            HashSet<Vector2Int> matches = new HashSet<Vector2Int>();

            //Test horizontal matches
            //Wonder if this can be done using Vector2.distance to handle diagonal matches as well
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width - 2; x++)
                {
                    GridObject<Gem> gemA = _grid.GetValue(x, y);
                    GridObject<Gem> gemB = _grid.GetValue(x + 1, y);
                    GridObject<Gem> gemC = _grid.GetValue(x + 2, y);
                   
                    /*
                     In a loop using continue will just skip this iteration of the loop
                    using break will entirely exit the loop
                     */
                    if (gemA == null || gemB == null || gemC == null) continue;

                    if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() &&
                        gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                    {
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x + 1, y));
                        matches.Add(new Vector2Int(x + 2, y));

                    }
                }
            }
            //Test for vertical matches
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height - 2; y++)
                {
                    GridObject<Gem> gemA = _grid.GetValue(x, y);
                    GridObject<Gem> gemB = _grid.GetValue(x, y + 1);
                    GridObject<Gem> gemC = _grid.GetValue(x, y + 2);
                   // Gem gemValue = gemA.GetValue();
                  //  var gemtype = gemValue.GetGemType();
                    /*
                     In a loop using continue will just skip this iteration of the loop
                    using break will entirely exit the loop
                     */
                    if (gemA == null || gemB == null || gemC == null) continue;

                    Debug.Log($"Testing match at {x}, {y} {gemA.GetValue().GetGemType()}" +
                        $"{gemB.GetValue().GetGemType()} {gemC.GetValue().GetGemType()}");
                    if (gemA.GetValue().GetGemType() == gemB.GetValue().GetGemType() &&
                        gemB.GetValue().GetGemType() == gemC.GetValue().GetGemType())
                    {
                        Debug.Log($"Found match at {x}, {y}");
                        matches.Add(new Vector2Int(x, y));
                        matches.Add(new Vector2Int(x, y + 1));
                        matches.Add(new Vector2Int(x, y + 2));

                    }
                }
            }
            Debug.Log($"Found {matches.Count} matches");
            return new List<Vector2Int>(matches);
                }

        IEnumerator SwapGems(Vector2Int gridPosA, Vector2Int gridPosB)
        {
            var gridObjectA = _grid.GetValue(gridPosA.x, gridPosA.y);
            var gridObjectB = _grid.GetValue(gridPosB.x, gridPosB.y);

            Debug.Log($" a pos ={_grid.GetWorldPositonCenter(gridPosA.x, gridPosA.y)} " +
                $"and b pos is {_grid.GetWorldPositonCenter(gridPosB.x, gridPosB.y)}");
            gridObjectA.GetValue().transform
                .DOMove(_grid.GetWorldPositonCenter(gridPosB.x, gridPosB.y), .5f).SetEase(_ease);
            gridObjectB.GetValue().transform
                .DOMove(_grid.GetWorldPositonCenter(gridPosA.x, gridPosA.y), .5f).SetEase(_ease);
            _audioManager.PlayWoosh();
            _grid.SetValue(gridPosA.x, gridPosA.y, gridObjectB);
            _grid.SetValue(gridPosB.x, gridPosB.y, gridObjectA);
            yield return new WaitForSeconds(.5f);

            

        }
        private void SelectGem(Vector2Int gridpos)
        {
           _selectedGem = gridpos;
            _audioManager.PlayClick();
        }

        private void DeselectGem()
        {
            _selectedGem = Vector2Int.one * -1;
            _audioManager.PlayDeselect();
        }

        void InitializeGrid()
        {
         
            if (_rotationType == RotationType.Horizontal)
            {
                rotation = Quaternion.Euler(90, 0, 0);
                _grid = GridSystem2D<GridObject<Gem>>.HorizontalGrid(_width, _height, _cellSize, _originPosition, _debug);
            }
            else
            {
                _grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(_width, _height, _cellSize, _originPosition, _debug);
                rotation = Quaternion.Euler(0, 0, 0);
            }
        
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    CreateGem(x, y);
                }
            }
            //Init the grid
            //Read player inputs

            //StartCoroutine:
            //swapAnimation
            //Check for matches
            //Make gems explode
            //Replace emptySpot
            //Check game over
        }

        private void CreateGem(int x, int y)
        {
            //Fix gem rotation to match the grid

            Vector3 destination = _grid.GetWorldPositonCenter(x, y);


;            Gem gem = Instantiate(_gemPrefab,destination + transform.up * _height * Random.Range(1,3), rotation, transform);
            gem.transform.DOMove(destination, .5f).SetEase(_ease);
            gem.SetType(_gemTypes[Random.Range(0, _gemTypes.Length)]);
            GridObject<Gem> gridObject = new GridObject<Gem>(_grid, x, y);
            gridObject.SetValue(gem);
            _grid.SetValue(x, y, gridObject);

        }
        private void OnDestroy()
        {
            _inputReader.Fire += OnSelectGem;
        }
    }
}
