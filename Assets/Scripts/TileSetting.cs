using UnityEngine;
[CreateAssetMenu(fileName="TileSetting" ,menuName="WeMade2048/TileSetting" ,order=0)]
public class TileSetting :ScriptableObject
{
    public float AnimationTime = 0.3f;
    public AnimationCurve AnimationCurve;
    public TileColor[] TileColors;
}
