using UnityEngine;
using DG.Tweening;

public class Room : MonoBehaviour
{
    public enum Direction { LEFT, TOP, RIGHT, BOTTOM };

    public RoomInfo roomInfo;
    public int numClockRotations;

    public Room leftRoom;
    public Room topRoom;
    public Room rightRoom;
    public Room bottomRoom;

    public GameObject[] doors;
    public GameObject[] enemies;

    private int deadEnemies = 0;
    private bool Completed { get { return deadEnemies >= enemies.Length; }  }

    public void EnterRoom()
    {
        GameManager.instance.mainCamera.transform.DOMove(transform.position, 0.5f).SetEase(Ease.InOutFlash);
        if (Completed) return;
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(true);
        }
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }
    }

    public void ExitRoom(Direction direction)
    {
        Room room = leftRoom;
        switch (direction)
        {
            case Direction.LEFT:
                room = leftRoom;
                break;
            case Direction.TOP:
                room = topRoom;
                break;
            case Direction.RIGHT:
                room = rightRoom;
                break;
            case Direction.BOTTOM:
                room = bottomRoom;
                break;
                
        }
        room.EnterRoom();
    }

    public void EnemyDead()
    {
        deadEnemies++;
        if (Completed)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].SetActive(false);
            }
        }
    }
}
