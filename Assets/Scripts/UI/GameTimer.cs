using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void UpdateTimer(TimeSpan gameTime)
    {
        string format = "";
        if (gameTime.Hours > 0)
        {
            format = "hh\\:";
        }
        format += "mm\\:ss";
        _text.text = gameTime.ToString(format);
    }
}
