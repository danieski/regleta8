using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MapGenerator : MonoBehaviour
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
        for (int i = 0; i < numRows; i++)
        {
            rooms.Add(new List<Room>(numColumns));
            for (int j = 0; j < numColumns; j++)
            {
                rooms[i].Add(null);
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
        Destroy(rooms[iMostFarRoom][jMostFarRoom]);
        rooms[iMostFarRoom][jMostFarRoom] = null;
        StepGen(iMostFarRoom, jMostFarRoom, bossRooms[Random.Range(0, bossRooms.Count)], numClockRotations);
    }

    private bool StepGen(int row, int column, RoomInfo forceRoomInfo = null, int forceNumClockRotations = -1)
    {
        /*  Condiciones de salida
         *  Si estoy fuera de mapa
         *  Si ya hay una habitación en la coordenada
         *  Si hay alguna sala colindante que no tiene una puerta hacia mí
         */
        if (row < 0 || row >= numRows || column < 0 || column >= numColumns)
            return false;
        if (rooms[row][column] != null)
            return false;
        List<RoomConfig> posibilities = GetAllowedDoorsConfigs(row, column, true);
        if (posibilities == null || posibilities.Count == 0)
            return false;


        // Decidir qué sala instanciar
        RoomInfo roomInfo = forceRoomInfo;
        int numClockRotations = forceNumClockRotations == -1 ? Random.Range(0, 4) : forceNumClockRotations;
        if (roomInfo == null)
        {
            RoomConfig selectedConfig = posibilities.Randomize().ToList()[Random.Range(0,posibilities.Count)];
            roomInfo = roomsByDoors[selectedConfig.doorsConfig][Random.Range(0, roomsByDoors[selectedConfig.doorsConfig].Count)];
            numClockRotations = selectedConfig.numClockRotations;
        }
        Debug.Log("Instanciando una room " + roomInfo.doorsConfig + " girado " + numClockRotations + " veces");
        // Instanciar
        Room room = Instantiate(
            roomInfo.prefab,
            transform.position + new Vector3(column * roomSize, 0, -row * roomSize),
            Quaternion.Euler(90 * numClockRotations * Vector3.up)
        ).GetComponent<Room>();
        room.roomInfo = roomInfo;
        room.numClockRotations = numClockRotations;
        rooms[row][column] = room;

        bool ok = false;
        bool leftFailed = false;
        bool topFailed = false;
        bool rightFailed = false;
        bool bottomFailed = false;
        // Izquierda
        if (column > 0)
        {
            Debug.Log("Tiremos para la izquierda");
            ok = StepGen(row, column - 1);
            if (rooms[row][column-1] != null)
            {
                room.leftRoom = rooms[row][column - 1];
                rooms[row][column - 1].rightRoom = room;
            } else if (RoomHasLeft(room))
            {
                leftFailed = true;
            }
        }
        // Arriba
        if (row > 0)
        {
            Debug.Log("Tiremos para arriba");
            ok = StepGen(row - 1, column);
            if (ok)
            {
                room.leftRoom = rooms[row - 1][column];
                rooms[row - 1][column].rightRoom = room;
            } else if (RoomHasUp(room))
            {
                topFailed = true;
            }
        }
        // Derecha
        if (column < numColumns - 1)
        {
            Debug.Log("Tiremos para la derecha");
            ok = StepGen(row, column + 1);
            if (ok)
            {
                room.leftRoom = rooms[row][column + 1];
                rooms[row][column + 1].rightRoom = room;
            } else if (RoomHasRight(room))
            {
                rightFailed = true;
            }

        }
        // Abajo
        if (row < numRows - 1)
        {
            Debug.Log("Tiremos para abajo");
            StepGen(row + 1, column);
            if (ok)
            {
                room.leftRoom = rooms[row + 1][column];
                rooms[row + 1][column].rightRoom = room;
            } else if (RoomHasDown(room))
            {
                bottomFailed = true;
            }
        }

        if (leftFailed || topFailed || rightFailed || bottomFailed)
        {
            Debug.Log("Failed: left-" + leftFailed + " top-" + topFailed + " right-" + rightFailed + " bottom-" + bottomFailed);
        }

        return true;
    }

    private List<RoomConfig> GetAllowedDoorsConfigs(int row, int column, bool precheck = false)
    {
        Room room;
        // Izquierda
        bool needLeft = false;
        bool canLeft = column > 0;
        if (canLeft && precheck)
        {
            List<RoomConfig> checkposibilities = GetAllowedDoorsConfigs(row, column-1);
            if (checkposibilities == null || checkposibilities.Count == 0)
                canLeft = false;
        }
        if (canLeft)
        {
            room = rooms[row][column - 1];
            if (room != null) {
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.ONE && room.numClockRotations != 3)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.L && room.numClockRotations != 2 && room.numClockRotations != 3)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.STRAIGHT && room.numClockRotations != 1 && room.numClockRotations != 3)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.THREE && room.numClockRotations == 1)
                    return null;
                needLeft = true;
            }
        }
        // Arriba
        bool needUp = false;
        bool canUp = row > 0;
        if (canUp && precheck)
        {
            List<RoomConfig> checkposibilities = GetAllowedDoorsConfigs(row - 1, column);
            if (checkposibilities == null || checkposibilities.Count == 0)
                canUp = false;
        }
        if (canUp)
        {
            room = rooms[row - 1][column];
            if (room != null) {
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.ONE && room.numClockRotations != 0)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.L && room.numClockRotations != 0 && room.numClockRotations != 3)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.STRAIGHT && room.numClockRotations != 0 && room.numClockRotations != 2)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.THREE && room.numClockRotations == 2)
                    return null;
                needUp = true;
            }
        }
        // Derecha
        bool needRight = false;
        bool canRight = column < numColumns - 1;
        if (canRight && precheck)
        {
            List<RoomConfig> checkposibilities = GetAllowedDoorsConfigs(row, column + 1);
            if (checkposibilities == null || checkposibilities.Count == 0)
                canRight = false;
        }
        if (canRight)
        {
            room = rooms[row][column + 1];
            if (room != null) {
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.ONE && room.numClockRotations != 1)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.L && room.numClockRotations != 0 && room.numClockRotations != 1)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.STRAIGHT && room.numClockRotations != 1 && room.numClockRotations != 3)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.THREE && room.numClockRotations == 3)
                    return null;
                needRight = true;
            }
        }
        // Abajo
        bool needDown = false;
        bool canDown = row < numRows - 1;
        if (canDown && precheck)
        {
            List<RoomConfig> checkposibilities = GetAllowedDoorsConfigs(row + 1, column);
            if (checkposibilities == null || checkposibilities.Count == 0)
                canDown = false;
        }
        if (canDown)
        {
            room = rooms[row + 1][column];
            if (room != null) {
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.ONE && room.numClockRotations != 2)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.L && room.numClockRotations != 1 && room.numClockRotations != 2)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.STRAIGHT && room.numClockRotations != 0 && room.numClockRotations != 2)
                    return null;
                if (room.roomInfo.doorsConfig == RoomInfo.DoorsConfig.THREE && room.numClockRotations == 0)
                    return null;
                needDown = true;
            }
        }

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
        if (needLeft)
        {
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
        } else if (!canLeft)
        {
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
        if (needUp)
        {
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!canUp)
        {
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
        if (needRight)
        {
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 2));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!canRight)
        {
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
        if (needDown)
        {
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.THREE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.L, 3));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 0));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 1));
            posibilities.Remove(new RoomConfig(RoomInfo.DoorsConfig.ONE, 3));
        }
        else if (!canDown)
        {
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
    private bool RoomHasLeft(Room room)
    {
        return new List<RoomConfig>{
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 3),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 0),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 2),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 3),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3),
            new RoomConfig(RoomInfo.DoorsConfig.L, 2),
            new RoomConfig(RoomInfo.DoorsConfig.L, 3),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 3)
        }.Contains(new RoomConfig(room.roomInfo.doorsConfig, room.numClockRotations));
    }
    private bool RoomHasUp(Room room)
    {
        return new List<RoomConfig>{
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 3),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 0),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 1),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 3),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2),
            new RoomConfig(RoomInfo.DoorsConfig.L, 0),
            new RoomConfig(RoomInfo.DoorsConfig.L, 3),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 0)
        }.Contains(new RoomConfig(room.roomInfo.doorsConfig, room.numClockRotations));
    }
    private bool RoomHasRight(Room room)
    {
        return new List<RoomConfig>{
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 3),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 0),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 1),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 2),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 1),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 3),
            new RoomConfig(RoomInfo.DoorsConfig.L, 0),
            new RoomConfig(RoomInfo.DoorsConfig.L, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 1)
        }.Contains(new RoomConfig(room.roomInfo.doorsConfig, room.numClockRotations));
    }
    private bool RoomHasDown(Room room)
    {
        return new List<RoomConfig>{
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 0),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 1),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ALL, 3),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 1),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 2),
            new RoomConfig(RoomInfo.DoorsConfig.THREE, 3),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 0),
            new RoomConfig(RoomInfo.DoorsConfig.STRAIGHT, 2),
            new RoomConfig(RoomInfo.DoorsConfig.L, 1),
            new RoomConfig(RoomInfo.DoorsConfig.L, 2),
            new RoomConfig(RoomInfo.DoorsConfig.ONE, 2)
        }.Contains(new RoomConfig(room.roomInfo.doorsConfig, room.numClockRotations));
    }
}
