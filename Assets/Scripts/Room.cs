using UnityEngine;
using DG.Tweening;
using System.Collections;

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

    public IEnumerator EnterRoom()
    {
        GameManager.instance.mainCamera.transform.DOMove(transform.position + Vector3.up * GameManager.instance.mainCamera.transform.position.y, 0.5f).SetEase(Ease.InOutFlash);
        if (!Completed)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].SetActive(true);
            }
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(true);
            }
        }
    }

    public void ExitRoom(string direction)
    {
        Room room = leftRoom;
        switch (direction)
        {
            case "LEFT":
                room = leftRoom;
                break;
            case "TOP":
                room = topRoom;
                break;
            case "RIGHT":
                room = rightRoom;
                break;
            case "BOTTOM":
                room = bottomRoom;
                break;
                
        }
        room.StartCoroutine(room.EnterRoom());
    }

    public void EnemyDead()
    {
        deadEnemies++;
        if (Completed)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].GetComponent<MeshRenderer>().enabled = false;
                doors[i].GetComponent<Collider>().isTrigger = true;
            }
        }
    }
}
