using UnityEngine;

public delegate void Vector2Callback();

[CreateAssetMenu(fileName = "InputManager", menuName = "ScriptableObjects/Input/InputManager")]
public class InputManager : ScriptableObject
{
    private InputControls _inputControls;

    public event Vector2Callback OnOpenTile;
    public event Vector2Callback OnAddFlag;

    public void Setup()
    {
        _inputControls = new InputControls();
        _inputControls.Enable();

        _inputControls.Game.OpenTile.performed += _ =>
        {
            OnOpenTile?.Invoke();
        };

        _inputControls.Game.AddFlag.performed += _ =>
        {
            OnAddFlag?.Invoke();
        };
    }

    public void Disable()
    {
        _inputControls.Game.Disable();
    }

    public void Enable()
    {
        _inputControls.Game.Enable();
    }
}
