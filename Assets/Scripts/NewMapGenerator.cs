using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewMapGenerator : MonoBehaviour
{
    public struct RoomConfig
    {
        public RoomConfig(RoomInfo.DoorsConfig doorsConfig, int numClockRotations)
        {
            this.doorsConfig = doorsConfig;
            this.numClockRotations = numClockRotations;
        }

        public RoomInfo.DoorsConfig doorsConfig;
        public int numClockRotations;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            RoomConfig other = (RoomConfig)obj;
            if (doorsConfig != other.doorsConfig || numClockRotations != other.numClockRotations)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return (doorsConfig.GetHashCode() + numClockRotations).GetHashCode();
        }
    }


    private List<List<Room>> rooms;
    private List<List<bool>> promises;

    [SerializeField] private int numColumns, numRows;
    [SerializeField] private float roomSize;
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
        GenMap(0);
    }

    public void GenMap(int layer)
    {
        // Inicializar matriz
        rooms = new List<List<Room>>(numRows);
        promises = new List<List<bool>>();
        for (int i = 0; i < numRows; i++)
        {
            rooms.Add(new List<Room>(numColumns));
            promises.Add(new List<bool>(numColumns));
            for (int j = 0; j < numColumns; j++)
            {
                rooms[i].Add(null);
                promises[i].Add(false);
            }
        }

        // Aleatorizar punto de inicio y orientación
        int initColumn = Random.Range(1, numColumns - 1);
        int initRow = Random.Range(1, numRows - 1);
        RoomInfo roomInfo = initRooms[Random.Range(0, initRooms.Count)];

        // Rellenar las habitaciones normales
        StepGen(initRow, initColumn, roomInfo);

        int distanceWithInit = 0;
        int iMostFarRoom = 0, jMostFarRoom = 0;
        // Añadir habitación de boss
        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = 0; j < rooms[i].Count; j++)
            {
                int distance = DistanceBetweenRooms(initRow, initColumn, i, j);
                if (distance > distanceWithInit && rooms[i][j] != null && rooms[i][j].roomInfo.doorsConfig == RoomInfo.DoorsConfig.ONE)
                {
                    distanceWithInit = distance;
                    iMostFarRoom = i;
                    jMostFarRoom = j;
                }
            }
        }
        int numClockRotations = rooms[iMostFarRoom][jMostFarRoom].numClockRotations;
        Destroy(rooms[iMostFarRoom][jMostFarRoom].gameObject);
        rooms[iMostFarRoom][jMostFarRoom] = null;
        StepGen(iMostFarRoom, jMostFarRoom, bossRooms[Random.Range(0, bossRooms.Count)], numClockRotations);
    }

    private Room StepGen(int row, int column, RoomInfo forceRoomInfo = null, int forceNumClockRotations = -1)
    {
        if (!CanInstantiateHere(row, column))
            return null;

        // Decidir qué sala instanciar
        List<RoomConfig> posibilities = GetAllowedRoomConfigs(row, column);
        if (rooms.Count == 0) throw new System.Exception("Cómo no va ha haber posibilidades?!");

        RoomInfo roomInfo = forceRoomInfo;
        int numClockRotations = forceNumClockRotations == -1 ? Random.Range(0, 4) : forceNumClockRotations;
        if (roomInfo == null)
        {
            RoomConfig selectedConfig = posibilities.Randomize().ToList()[Random.Range(0, posibilities.Count)];
            roomInfo = roomsByDoors[selectedConfig.doorsConfig][Random.Range(0, roomsByDoors[selectedConfig.doorsConfig].Count)];
            numClockRotations = selectedConfig.numClockRotations;
        }

        // Instanciar
        Room room = Instantiate(
            roomInfo.prefab,
            transform.position + new Vector3(column * roomSize, 0, -row * roomSize),
            Quaternion.Euler(90 * numClockRotations * Vector3.up)
        ).GetComponent<Room>();
        room.roomInfo = roomInfo;
        room.name = "Room ( " + column+" , "+row+" )";
        if (room.roomInfo.roomType == RoomInfo.RoomType.INIT)
        {
            room.name += " INIT";
            GameManager.instance.player.transform.position = room.transform.position + Vector3.up * 3.4f;
            room.EnterRoom();
        } else if (room.roomInfo.roomType == RoomInfo.RoomType.BOSS)
        {
            room.name += " BOSS";
        }
        room.numClockRotations = numClockRotations;
        rooms[row][column] = room;

        if (room.HasDoor(Room.Direction.LEFT))
            promises[row][column - 1] = true;
        if (room.HasDoor(Room.Direction.TOP))
            promises[row - 1][column] = true;
        if (room.HasDoor(Room.Direction.RIGHT))
            promises[row][column + 1] = true;
        if (room.HasDoor(Room.Direction.BOTTOM))
            promises[row + 1][column] = true;

        StepGen(row, column - 1);
        StepGen(row - 1, column);
        StepGen(row, column + 1);
        StepGen(row + 1, column);

        if (column - 1 >= 0)
            room.leftRoom = rooms[row][column - 1];
        if (row - 1 >= 0)
            room.topRoom = rooms[row - 1][column];
        if (column + 1 < rooms[row].Count)
            room.rightRoom = rooms[row][column + 1];
        if (row + 1 < rooms.Count)
            room.bottomRoom = rooms[row + 1][column];

        return room;
    }

    private bool CanInstantiateHere(int row, int column)
    {
        if (row < 0 || row >= numRows || column < 0 || column >= numColumns) // Si está fuera de los bordes del mapa
            return false;
        if (rooms[row][column] != null) // Si ya hay una habitación
            return false;

        if (column - 1 >= 0 && rooms[row][column - 1] != null && !rooms[row][column - 1].HasDoor(Room.Direction.RIGHT))
            return false;
        if (row - 1 >= 0 && rooms[row - 1][column] != null && !rooms[row - 1][column].HasDoor(Room.Direction.BOTTOM))
            return false;
        if (column + 1 < rooms[row].Count && rooms[row][column + 1] != null && !rooms[row][column + 1].HasDoor(Room.Direction.LEFT))
            return false;
        if (row + 1 < rooms.Count && rooms[row + 1][column] != null && !rooms[row + 1][column].HasDoor(Room.Direction.TOP))
            return false;
        return true;
    }

    private List<RoomConfig> GetAllowedRoomConfigs(int row, int column)
    {
        List<RoomConfig> posibilities = new List<RoomConfig> {
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 3),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 0),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 1),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 2),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 3),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3),
            new RoomConfig(RoomInfo.DoorsConfig.L, 0),
            new RoomConfig(RoomInfo.DoorsConfig.L, 1),
            new RoomConfig(RoomInfo.DoorsConfig.L, 2),
            new RoomConfig(RoomInfo.DoorsConfig.L, 3),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 3),
        };
        if (column - 1 >= 0 && (rooms[row][column - 1] != null || promises[row][column - 1])) // Izquierda
        {
            // Debe tener izquierda
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
        } else if (!CanInstantiateHere(row, column - 1))
        {
            // No puede tener izquierda
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 4));
        }
        if (row - 1 >= 0 && (rooms[row - 1][column] != null || promises[row - 1][column])) // Arriba
        {
            // Debe tener arriba
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!CanInstantiateHere(row - 1,column))
        {
            // No puede tener arriba
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
        }
        if (column + 1 < rooms[row].Count && (rooms[row][column + 1] != null || promises[row][column + 1])) // Derecha
        {
            // Debe tener derecha
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!CanInstantiateHere(row, column + 1))
        {
            // No puede tener derecha
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
        }
        if (row + 1 < rooms.Count && (rooms[row + 1][column] != null || promises[row + 1][column])) // Abajo
        {
            // Debe tener abajo
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!CanInstantiateHere(row + 1, column))
        {
            // No puede tener abajo
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ALL, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
        }
        return posibilities;
    }


    private int DistanceBetweenRooms(int row1, int column1, int row2, int column2)
    {
        return Mathf.Abs(row1 - row2) + Mathf.Abs(column1 - column2);
    }
}

/** Elijo celda aleatoria para instanciar la primera celda (nunca en borde)
 *  
 *  ------------- Función recursiva -------------
 *  Compruebo si podría llegar a haber una habitación en mi celda
 *      Si no podría entonces CORTO.
 *  Compruebo si ya hay una habitación a mi izquierda
 *      Si la hay compruebo si tiene una puerta hacia mí
 *          Si la tiene descarto las opciones que no tienen una puerta en la izquierda.
 *          Si no la tiene entonces CORTO.
 *      Si no la hay compruebo si a mi izquerda podría llegar a haber una habitación
 *          Si no podría descarto las opciones con puerta en la izquierda.
 *  Estas comprobaciones igual para el resto de lados.
 *  Una vez he descartado todas las opciones que no podrían ser elijo una.
 *  Instancio una habitación en mi celda.
 *  Llamo a esta función para la celda de mi izquierda.
 *  Llamo a esta función para la celda de encima.
 *  Llamo a esta función para la celda de mi derecha.
 *  Llamo a esta función para la celda de abajo.
 *  ---------------------------------------------
 *  
 *  Qué significa que no podría llegar a haber una habitación en una celda?
 *  1. Que esté fuera de los bordes del mapa.
 *  2. Que ya haya una habitación en esa celda.
 *  3. Que haya una habitación en alguna celda colindante que no tenga una puerta hacia esa celda.
 *  
 */