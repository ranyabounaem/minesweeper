using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    private Vector2Int _pos;
    private bool _isMine;
    private bool _isFlagged;
    private bool _isUnlocked;


    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private GameObject _flag;
    [SerializeField]
    private TextMeshPro _mineNumber;
    [SerializeField]
    private GameObject _mine;

    public void Setup(Vector2Int pos, bool isMine)
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _pos = pos;
        _isMine = isMine;
        _isFlagged = false;
        _isUnlocked = false;
    }

    public void SetStatus(bool isMine)
    {
        _isMine = isMine;
    }

    public int ToggleFlagged()
    {
        if (!_isUnlocked)
        {
            _isFlagged = !_isFlagged;
            _flag.SetActive(_isFlagged);
            return _isFlagged ? 1 : -1;
        }
        return 0;
    }

    public bool UnlockTile(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
        _isUnlocked = true;

        if (_isMine)
        {
            _mine.SetActive(true);
            return true;
        }

        return false;
            
    }

    public void ShowMineNumber(int num)
    {
        _mineNumber.text = num.ToString();
        _mineNumber.gameObject.SetActive(true);
    }

    public bool GetFlagged()
    {
        return _isFlagged;
    }

    public Vector2Int GetPos()
    {
        return _pos;
    }

    public bool GetMine()
    {
        return _isMine;
    }

    public bool GetUnlocked()
    {
        return _isUnlocked;
    }

    public void Restart(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;

        _mineNumber.gameObject.SetActive(false);
        _mine.SetActive(false);
        _flag.SetActive(false);

        _isFlagged = false;
        _isUnlocked = false;
    }

}
