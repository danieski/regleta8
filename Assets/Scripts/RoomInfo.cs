using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room")]
public class RoomInfo : ScriptableObject
{
    public enum RoomType { INIT, NORMAL, BOSS }
    
    public enum DoorsConfig { ONE, L, STRAIGHT, THREE, ALL }

    public RoomType roomType;
    public DoorsConfig doorsConfig;

    public GameObject prefab;
}
