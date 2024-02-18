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
    public GameObject bossRoom;
}

[System.Serializable]
public class Room
{
    public Vector2 pos;
    public RoomType type;
    public RoomParticularity particularity;
    public GameObject go;

    public int[] proximityRoomIndex;

    public Room(Vector2 pos, RoomType type, RoomParticularity particularity, GameObject go)
    {
        this.pos = pos;
        this.type = type;
        this.particularity = particularity;
        this.go = go;
        proximityRoomIndex = new int[8];
    }
}
public enum RoomType { Room, Corridor}
public enum RoomParticularity { Treasure, Boss, Secret, basic, Key}