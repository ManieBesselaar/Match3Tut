using UnityEngine;
[CreateAssetMenu(fileName = "ShapePattern", menuName = "Pattern")]
public class Pattern : ScriptableObject
{
    public string Name;
    public Vector2Int[] Positions;
    public int Width;
    public int Height;
}
