using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class GridStructure
{
    public int rowSize;
    public int colSize;
    public int mineCount;
}

public delegate void VoidCallback();
public delegate void IntCallback(int c);
public class Grid : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField]
    private bool _readFromFile;
    [SerializeField]
    private int _rowSize = 0;
    [SerializeField]
    private int _colSize = 0;
    [SerializeField]
    private int _mineCount = 0;

    [Header("References")]
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private InputManager _inputManager;
    [SerializeField]
    private GameObject _tilePrefab;
    [SerializeField]
    private Sprite _upTile;
    [SerializeField]
    private Sprite _downTile;

    private Tile[,] _tiles;

    public event VoidCallback OnLose;
    public event IntCallback OnFlag;

    private int _totalFlags = 0;
    private int _counter = 0;

    public static T ImportJson<T>(string path)
    {
        string __text = null;
        try
        {
            __text = File.ReadAllText(Application.dataPath + "/StreamingAssets/" + path);
        }
        catch
        {
            return default;
        }

        return JsonUtility.FromJson<T>(__text);
    }

    public void SetupGrid()
    {
        if (_readFromFile)
        {
            var __gridStructure = ImportJson<GridStructure>("JSON/GridConfiguration.json");
            if (__gridStructure != null)
            {
                _rowSize = __gridStructure.rowSize > 0 ? __gridStructure.rowSize : 9;
                _colSize = __gridStructure.colSize > 0 ? __gridStructure.colSize : 9;
                _mineCount = __gridStructure.mineCount > 0 ? __gridStructure.mineCount : 10;
                if (_mineCount > _rowSize * _colSize)
                {
                    _mineCount = _rowSize * _colSize - 1;
                }
            }
            else
            {
                _rowSize = 18;
                _colSize = 18;
                _mineCount = 10;
            }
        }

        _camera.orthographicSize = (Mathf.Max(_rowSize, _colSize) / 2) + 1;
        _tiles = new Tile[_rowSize, _colSize];

        var __flattenedCount = _rowSize * _colSize;

        if (_mineCount > __flattenedCount - 1)
        {
            _mineCount = __flattenedCount - 1;
        }

        var __mineIndices = new List<int>(_mineCount);

        for (var __i = 0; __i < _mineCount; __i++)
        {
            var __mineIndex = Random.Range(0, __flattenedCount);
            while (__mineIndices.Contains(__mineIndex))
            {
                __mineIndex = Random.Range(0, __flattenedCount);
            }
            __mineIndices.Add(__mineIndex);
        }

        _camera.transform.position = new Vector3(_colSize / 2, _rowSize / 2, _camera.transform.position.z);

        _inputManager.Setup();

        _inputManager.OnOpenTile += TileClick;

        _inputManager.OnAddFlag += AddFlag;

        for (var __i = 0; __i < _rowSize; __i++)
        {
            for (var __j = 0; __j < _colSize; __j++)
            {
                var __instantiatedTile = Instantiate(_tilePrefab, new Vector2(__j, __i), Quaternion.identity, transform);
                var __tileComponent = __instantiatedTile.GetComponent<Tile>();
                __tileComponent.Setup(new Vector2Int(__i, __j), false);
                _tiles[__i, __j] = __tileComponent;
            }
        }


        for (var __i = 0; __i < _mineCount; __i++)
        {
            var __row = __mineIndices[__i] / _rowSize;
            var __col = __mineIndices[__i] % _colSize;

            _tiles[__row, __col].SetStatus(true);
        }

        _counter = _mineCount;
        OnFlag?.Invoke(_counter);
    }

    private void Start()
    {
        SetupGrid();
    }

    private void TileClick()
    {
        var __mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Physics2D.Raycast(__mousePos, Vector2.zero).collider.TryGetComponent(out Tile __clickedTile))
        {
            if (__clickedTile.GetFlagged() || __clickedTile.GetUnlocked())
            {
                return;
            }
            var lost = __clickedTile.UnlockTile(_downTile);
            if (lost)
            {
                OnLose?.Invoke();
                _inputManager.Disable();
                Debug.Log("Lost");
            }

            if(!__clickedTile.GetFlagged() && !__clickedTile.GetMine())
            {
                var __mineCount = GetSurroundingMines(__clickedTile);

                if (__mineCount > 0)
                {
                    __clickedTile.ShowMineNumber(__mineCount);
                }
                else
                {
                    RecursiveUnlock(__clickedTile);
                }
            }
        }
    }

    private void RecursiveUnlock(Tile tile)
    {
        var __tiles = GetSurroundingTiles(tile);

        foreach (var __tile in __tiles)
        {
            if (!__tile.GetUnlocked() && !__tile.GetFlagged())
            {
                __tile.UnlockTile(_downTile);
                var __mineCount = GetSurroundingMines(__tile);

                if (__mineCount > 0)
                {
                    __tile.ShowMineNumber(__mineCount);
                }
                else
                {
                    RecursiveUnlock(__tile);
                }
            }
            
        }
    }

    private void AddFlag()
    {
        var __mousePos = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Physics2D.Raycast(__mousePos, Vector2.zero).collider.TryGetComponent(out Tile __clickedTile))
        {
            _totalFlags += __clickedTile.ToggleFlagged();
            _counter = Mathf.Clamp(_mineCount - _totalFlags, 0, _mineCount);
            OnFlag?.Invoke(_counter);
        }
    }

    public List<Tile> GetSurroundingTiles(Tile tile)
    {
        var __tiles = new List<Tile>();
        var __tilePos = tile.GetPos();
        Debug.Log(__tilePos);
        for (var __i = __tilePos.x - 1; __i < __tilePos.x + 2; __i++)
        {
            if (__i < 0)
            {
                continue;
            }

            if (__i > _rowSize - 1)
            {
                break;
            }

            for (var __j = __tilePos.y - 1; __j < __tilePos.y + 2; __j++)
            {
                if (__j < 0 || __j > _colSize - 1)
                {
                    continue;
                }

                if (__tilePos.y == __i && __tilePos.x == __j)
                {
                    continue;
                }
                __tiles.Add(_tiles[__i, __j]);
            }
        }

        return __tiles;
    }

    public int GetSurroundingMines(Tile tile)
    {
        var __minesCount = 0;

        var __tiles = GetSurroundingTiles(tile);
        foreach (var __tile in __tiles)
        {
            if (__tile.GetMine())
            {
                __minesCount++;
            }
        }
        return __minesCount;
    }

    public void Restart()
    {
        foreach (var __tile in _tiles)
        {
            __tile.Restart(_upTile);
        }
        _inputManager.Enable();
        _counter = _mineCount;
        _totalFlags = 0;
        OnFlag?.Invoke(_counter);
    }
}
