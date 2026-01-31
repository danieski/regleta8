using UnityEngine;

[CreateAssetMenu(fileName = "RoomInfo", menuName = "Scriptable Objects/RoomInfo")]
public class RoomInfo : ScriptableObject
{
    public enum RoomType { INIT, NORMAL, BOSS }
    
    public enum DoorsConfig { ONE, L, STRAIGHT, THREE, ALL }

    public RoomType roomType;
    public DoorsConfig doorsConfig;

    public GameObject prefab;
}
