using TMPro;
using UnityEngine;

public class MoveCounter : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void UpdateCount(int moveCount)
    {
        bool showDisplayPlural=moveCount != 1;
        _text.text = $"{moveCount} {(showDisplayPlural ? "moves" : "move")}";
    }
}
