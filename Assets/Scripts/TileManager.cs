using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class TileManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static int GridSize = 4;
    private readonly Transform[,] _tilePositions=new Transform[GridSize,GridSize];
    private readonly Tile[,] _tiles = new Tile[GridSize,GridSize];
    [SerializeField] private Tile tilePrefab;
    private bool _isAnimating;
    [SerializeField] private TileSetting tileSetting;
    private bool _tilesUpdated;

    [SerializeField] private UnityEvent<int> scoreUpdated;
    [SerializeField] private UnityEvent<int> bestscoreUpdated;
    [SerializeField] private UnityEvent<int> moveCountUpdated;
    [SerializeField] private UnityEvent<System.TimeSpan> gameTimeUpdated;
    [SerializeField] private GameOverScene gameOverScene;
    private Stack<GameState> _gameStates=new Stack<GameState>();

    private System.Diagnostics.Stopwatch _gameStopwatch=new System.Diagnostics.Stopwatch();
    private int _score;
    private int _bestScore;
    private int _moveCount;
    void Start()
    {
        GetTilePositions();
        TrySpawnTile();
        TrySpawnTile();
        UpdateTilePosition(true);
        _gameStopwatch.Start();
        _bestScore = PlayerPrefs.GetInt("BestScore", 0);
        bestscoreUpdated.Invoke(_bestScore);
    }
    private int _lastXinput;
    private int _lastYinput;
    // Update is called once per frame
    void Update()    
    {
        gameTimeUpdated.Invoke(_gameStopwatch.Elapsed);
        var xInput = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        var yInput = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        if(_lastXinput==0&&_lastYinput==0)
        {
            if (!_isAnimating)
                TryMove(xInput, yInput);
        }
        _lastXinput = xInput;
        _lastYinput = yInput;
    }
    
    public void RestartGame()
    {
        var activeScene=SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
    public void AddScore(int value)
    {
        _score += value;
        scoreUpdated.Invoke(_score);
        if (_score > _bestScore)
        {
            _bestScore = _score;
            bestscoreUpdated.Invoke(_bestScore);
            PlayerPrefs.SetInt("BestScore", _bestScore);
        }
        
    }
    private void GetTilePositions()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(this.GetComponent<RectTransform>());
        int x = 0;
        int y = 0;
        foreach(Transform transform in this.transform)
        {
            _tilePositions[x,y]=transform;
            x++;
            if (x >= GridSize)
            {
                x = 0;
                y++;
            }
        }
    }
    private bool TrySpawnTile()
    {
         List<Vector2Int> availableSpots=new List<Vector2Int>();
         for( int i = 0; i < GridSize; i++){
            for(int j = 0; j < GridSize; j++)
            {
                if (_tiles[i, j] == null)
                {
                    availableSpots.Add(new Vector2Int(i, j));
                }
            }
         }
        if (!availableSpots.Any())
            return false;
        int randomIndex=Random.Range(0,availableSpots.Count);  
        Vector2Int spot=availableSpots[randomIndex];
        var tile = Instantiate(tilePrefab,transform.parent);
        tile.SetValue(GetRandomValue());
        _tiles[spot.x, spot.y] = tile;
        return true;
    }

    private int GetRandomValue()
    {
        var rand= Random.Range(0f, 1f);
        if(rand <= 0.8f)
        {
            return 2;
        }
        else
        {
            return 4;
        }
    }
    private void UpdateTilePosition(bool instant)
    {
        if (!instant)
        {
            _isAnimating = true;
            StartCoroutine(WaitForTileAnimation());
        }
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (_tiles[i, j] != null){
                    _tiles[i, j].SetPosition(_tilePositions[i,j].position,instant);
                }
            }
        }
    }

    private IEnumerator WaitForTileAnimation()
    {
        yield return new WaitForSeconds(tileSetting.AnimationTime);
        if(!TrySpawnTile())
        {
            Debug.Log("Game Over");
        }
        if (!AnyMoveLeft())
        {
            gameOverScene.SetGameOver(true);
        }
        UpdateTilePosition(true);
        _isAnimating = false;
    }

    private bool AnyMoveLeft()
    {
        return CanMoveDown() || CanMoveUp() || CanMoveLeft() || CanMoveRight();
    }

    private void TryMove(int x,int y)
    {
        if(x==0&& y == 0)
        {
            return;
        }
        if(Mathf.Abs(x)==1 && Mathf.Abs(y)==1)
        {
            return;
        }
        _tilesUpdated = false;
        int[,] preMoveTileValues = GetCurrentTileValues();
        if (x == 0)
        {
            if (y>0)
            {
                TryMoveUp();
            }
            else
            {
                TryMoveDown();
            }
        }
        else
        {
            if (x > 0)
            {
                TryMoveRight();
            }
            else 
            {
                TryMoveLeft();
            }
        }
        if (_tilesUpdated)
        {
            _gameStates.Push(new GameState { tileValue = preMoveTileValues ,score=_score, moveCount=_moveCount});
            _moveCount++;
            moveCountUpdated.Invoke(_moveCount);
            UpdateTilePosition(false);
        }
    }

    private int[,] GetCurrentTileValues()
    {
        int[,] result = new int[GridSize, GridSize];
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                if (_tiles[i, j] != null)
                {
                    result[i, j] = _tiles[i, j].GetValue();
                }
            }
        }
        return result;
    }
    public void LoadLastGameState()
    {
        if(_isAnimating) return;
        if (!_gameStates.Any())
        {
            return;
        }
        GameState previousGameState = _gameStates.Pop();
        gameOverScene.SetGameOver(false);
        _score = previousGameState.score;
        scoreUpdated.Invoke(_score);
        _moveCount = previousGameState.moveCount;
        moveCountUpdated.Invoke(_moveCount);
        foreach (Tile t in _tiles)
        {
            if(t!=null)
            {
                Destroy(t.gameObject);
            }
        }
        for(int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                _tiles[i, j] = null;
                if (previousGameState.tileValue[i, j] == 0)
                {
                    continue;
                }
                Tile tile=Instantiate(tilePrefab, transform.parent);
                tile.SetValue(previousGameState.tileValue[i, j]);
                _tiles[i, j] = tile;
            }
        }
        UpdateTilePosition(true);
    }
    private bool TileExistsBetween(int x ,int y,int x2,int y2)
    {
        if (x == x2)
            return TileExistsBetweenVertical(x, y, y2);
        else if (y == y2)
            return TileExistsBetweenHorizontal(x, x2, y);
        return true;
    }

    private bool TileExistsBetweenHorizontal(int x, int x2, int y)
    {
        int minX=Mathf.Min(x, x2);
        int maxX=Mathf.Max(x, x2);
        for(int i=minX+1;i<maxX;i++)
        {
            if (_tiles[i, y] != null)
                return true;
        }
        return false;
    }

    private bool TileExistsBetweenVertical(int x, int y, int y2)
    {
        int minY = Mathf.Min(y, y2);
        int maxY = Mathf.Max(y, y2);
        for (int i = minY + 1; i < maxY; i++)
        {
            if (_tiles[x, i] != null)
                return true;
        }
        return false;
    }
    private bool CanMoveLeft()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = 0; x2 < x; x2++)
                {

                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }
                        if (_tiles[x2, y].CanMerge(_tiles[x, y]))
                        {
                            return true;
                        }
                        continue;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanMoveRight()
    {
        for (int y = 0; y < GridSize; y++)
        {
            for (int x = GridSize - 1; x >= 0; x--)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }
                        if (_tiles[x2, y].CanMerge(_tiles[x, y]))
                        {
                            return true;
                        }
                        continue;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanMoveDown()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = GridSize - 1; y >= 0; y--)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = GridSize - 1; y2 > y; y2--)
                {

                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }
                        if (_tiles[x, y2].CanMerge(_tiles[x, y]))
                        {
                            return true;
                        }
                        continue;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private bool CanMoveUp()
    {
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = 0; y2 < y; y2++)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }
                        if (_tiles[x, y2].CanMerge(_tiles[x, y]))
                        {
                            return true;
                        }
                        continue;
                    }
                    return true;
                }
            }
        }
        return false;
    }
    private void TryMoveLeft()
    {
        for(int y=0;y<GridSize; y++)
        {
            for (int x = 0; x < GridSize; x++)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = 0; x2 < x; x2++)
                {
                    
                    if (_tiles[x2, y] != null)
                    {
                        if (TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }
                        if (_tiles[x2, y].Merge(_tiles[x, y]))
                        {
                            _tilesUpdated = true;
                            _tiles[x, y] = null;
                            break;
                        }
                        continue;
                    }
                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveRight()
    {
        for(int y=0;y<GridSize; y++)
        {
            for (int x = GridSize - 1; x >= 0; x--)
            {
                if (_tiles[x, y] == null) continue;
                for (int x2 = GridSize - 1; x2 > x; x2--)
                {
                    if (_tiles[x2, y] != null)
                    {
                        if(TileExistsBetween(x, y, x2, y))
                        {
                            continue;
                        }
                        if (_tiles[x2,y].Merge(_tiles[x,y]))
                        {
                            _tilesUpdated = true;
                            _tiles[x, y] = null;
                            break;
                        }
                        continue;
                    }
                    _tilesUpdated = true;
                    _tiles[x2, y] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveDown()
    {
        for(int x=0;x<GridSize; x++)
        {
            for (int y = GridSize - 1; y >= 0; y--)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = GridSize - 1; y2 > y; y2--)
                {
                    
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }
                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tilesUpdated = true;
                            _tiles[x, y] = null;
                            break;
                        }
                        continue;
                    }
                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }
    }

    private void TryMoveUp()
    {
        for(int x=0;x<GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
            {
                if (_tiles[x, y] == null) continue;
                for (int y2 = 0; y2 < y; y2++)
                {
                    if (_tiles[x, y2] != null)
                    {
                        if (TileExistsBetween(x, y, x, y2))
                        {
                            continue;
                        }
                        if (_tiles[x, y2].Merge(_tiles[x, y]))
                        {
                            _tilesUpdated = true;
                            _tiles[x, y] = null;
                            break;
                        }
                        continue;
                    }
                    _tilesUpdated = true;
                    _tiles[x, y2] = _tiles[x, y];
                    _tiles[x, y] = null;
                    break;
                }
            }
        }
    }
}
