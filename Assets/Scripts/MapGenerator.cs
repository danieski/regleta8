using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    private List<List<Room>> rooms;

    [SerializeField] private int numColumns, numRows;
    [SerializeField] private RoomInfo[] roomsPool;
    private Dictionary<RoomInfo.DoorsConfig, List<RoomInfo>> roomsByDoors = new Dictionary<RoomInfo.DoorsConfig, List<RoomInfo>>();
    private List<RoomInfo> initRooms = new List<RoomInfo>();
    private List<RoomInfo> bossRooms = new List<RoomInfo>();

    private void Start()
    {
        for (int i = 0; i < roomsPool.Length; i++)
        {
            if (roomsPool[i].roomType == RoomInfo.RoomType.INIT)
            {
                initRooms.Add(roomsPool[i]);
                continue;
            }
            if (roomsPool[i].roomType == RoomInfo.RoomType.BOSS)
            {
                bossRooms.Add(roomsPool[i]);
                continue;
            }
            if (!roomsByDoors.ContainsKey(roomsPool[i].doorsConfig))
                roomsByDoors.Add(roomsPool[i].doorsConfig, new List<RoomInfo>());
            roomsByDoors[roomsPool[i].doorsConfig].Add(roomsPool[i]);
        }
    }

    public void GenMap(int layer)
    {
        // Inicializar matriz
        rooms = new List<List<Room>>(numRows);
        for (int i = 0; i < numRows; i++)
        {
            rooms[i] = new List<Room>(numColumns);
        }

        // Aleatorizar punto de inicio y orientación
        int initColumn = Random.Range(1, numColumns - 1);
        int initRow = Random.Range(1, numRows - 1);
        Room room = Instantiate(initRooms[Random.Range(0, initRooms.Count)].prefab).GetComponent<Room>();
        room.transform.Rotate(Vector3.up * 45 * Random.Range(0, 4));
        rooms[initRow][initColumn] = room;

        // Rellenar las habitaciones normales


        // Añadir habitación de boss
    }
}
