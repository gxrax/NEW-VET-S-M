using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "Game/Tool")]
public class ToolData : ScriptableObject
{
    public string toolName;
    public string curesDisease;
    public int price;
    public Sprite icon;

    public bool isUnlocked;
}
