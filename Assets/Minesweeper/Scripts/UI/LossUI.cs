using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LossUI : MonoBehaviour
{
    [SerializeField]
    private Button _restartButton;

    public void Setup(Grid grid)
    {
        _restartButton.onClick.AddListener(() =>
        {
            grid.Restart();
            gameObject.SetActive(false);
        });
    }
}
