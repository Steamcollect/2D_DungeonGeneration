using System.Collections.Generic;
using UnityEngine;

public class Stockage : MonoBehaviour
{
    public int maxRooms;
    public List<Room> currentsRoom;

    [Header("GO references")]
    public GameObject startRoom;
    public GameObject corridor;
    public GameObject room;
}

[System.Serializable]
public class Room
{
    public Vector2 pos;
    public RoomType type;
    public GameObject go;

    public int[] proximityRoomIndex;

    public Room(Vector2 pos, RoomType type, GameObject go)
    {
        this.pos = pos;
        this.type = type;
        this.go = go;
        proximityRoomIndex = new int[8];
    }
}
public enum RoomType { Room, Corridor}