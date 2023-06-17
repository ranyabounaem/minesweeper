using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private LossUI _lossPanel;

    [SerializeField]
    private CounterUI _counter;

    private void Awake()
    {
        _grid.OnLose += ShowLossPanel;
        _grid.OnFlag += UpdateCounter;

        _lossPanel.Setup(_grid);
    }

    private void ShowLossPanel()
    {
        _lossPanel.gameObject.SetActive(true);
    }

    private void UpdateCounter(int c)
    {
        _counter.UpdateCounter(c);
    }

}
