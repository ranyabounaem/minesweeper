using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CounterUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _counter;

    public void UpdateCounter(int value)
    {
        _counter.text = "Mines left: " + value;
    }
}
