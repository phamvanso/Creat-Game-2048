using UnityEngine;

public class GameState : MonoBehaviour
{
    public int[,] tileValue = new int[TileManager.GridSize,TileManager.GridSize];
    public int score;
    public int moveCount;
}
