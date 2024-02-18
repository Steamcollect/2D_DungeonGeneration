using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    [HideInInspector]public Stockage stockage;
    SpriteRenderer graphics;

    private void Awake()
    {
        graphics = GetComponent<SpriteRenderer>();
    }

    public GameObject topLeftCorner, buttomLeftCorner, topRightCorner, buttomRightCorner;
    public GameObject topLeftWall, buttomLeftWall, topRightWall, buttomRightWall, topLeftWall90, buttomLeftWall90, topRightWall90, buttomRightWall90;
    public GameObject topDoor, rightDoor, buttomDoor, leftDoor;

    /*
     * 1 top left
     * 2 middle left
     * 3 top right
     * 4 middle right
     * 5 buttom right
     * 6 middle buttom
     * 7 buttom left
     * 8 middle left
    */

    public void SetRoomVisual(int[] roomReferences)
    {
        if (roomReferences[1] != -1)
        {
            if (stockage.currentsRoom[roomReferences[1]].type == RoomType.Corridor)
            {
                topLeftWall.SetActive(true);
                topRightWall.SetActive(true);
            }
            else
            {
                topLeftCorner.SetActive(true);
                topRightCorner.SetActive(true);
            }
        }
        else
        {
            topDoor.SetActive(true);
            topRightWall.SetActive(true);
            topLeftWall.SetActive(true);
        }

        if (roomReferences[3] != -1)
        {
            if (stockage.currentsRoom[roomReferences[3]].type == RoomType.Corridor)
            {
                buttomRightWall90.SetActive(true);
                topRightWall90.SetActive(true);
            }
            else
            {
                buttomRightCorner.SetActive(true);
                topRightCorner.SetActive(true);
            }
        }
        else
        {
            rightDoor.SetActive(true);
            buttomRightWall90.SetActive(true);
            topRightWall90.SetActive(true);
        }

        if (roomReferences[5] != -1)
        {
            if (stockage.currentsRoom[roomReferences[5]].type == RoomType.Corridor)
            {
                buttomLeftWall.SetActive(true);
                buttomRightWall.SetActive(true);
            }
            else
            {
                buttomLeftCorner.SetActive(true);
                buttomRightCorner.SetActive(true);
            }
        }
        else
        {
            buttomDoor.SetActive(true);
            buttomLeftWall.SetActive(true);
            buttomRightWall.SetActive(true);
        }

        if (roomReferences[7] != -1)
        {
            if(stockage.currentsRoom[roomReferences[7]].type == RoomType.Corridor)
            {
                buttomLeftWall90.SetActive(true);
                topLeftWall90.SetActive(true);
            }
            else
            {
                buttomLeftCorner.SetActive(true);
                buttomLeftCorner.SetActive(true);
            }
        }
        else
        {
            leftDoor.SetActive(true);
            topLeftWall90.SetActive(true);
            buttomLeftWall90.SetActive(true);
        }

        if (roomReferences[7] != -1 && stockage.currentsRoom[roomReferences[7]].type == RoomType.Room &&
           roomReferences[0] != -1 && stockage.currentsRoom[roomReferences[0]].type == RoomType.Room &&
           roomReferences[1] != -1 && stockage.currentsRoom[roomReferences[1]].type == RoomType.Room) topLeftCorner.SetActive(false);
        
        if (roomReferences[1] != -1 && stockage.currentsRoom[roomReferences[1]].type == RoomType.Room &&
           roomReferences[2] != -1 && stockage.currentsRoom[roomReferences[2]].type == RoomType.Room &&
           roomReferences[3] != -1 && stockage.currentsRoom[roomReferences[3]].type == RoomType.Room) topRightCorner.SetActive(false);

        if (roomReferences[3] != -1 && stockage.currentsRoom[roomReferences[3]].type == RoomType.Room &&
           roomReferences[4] != -1 && stockage.currentsRoom[roomReferences[4]].type == RoomType.Room &&
           roomReferences[5] != -1 && stockage.currentsRoom[roomReferences[5]].type == RoomType.Room) buttomRightCorner.SetActive(false);
        
        if (roomReferences[5] != -1 && stockage.currentsRoom[roomReferences[5]].type == RoomType.Room &&
           roomReferences[6] != -1 && stockage.currentsRoom[roomReferences[6]].type == RoomType.Room &&
           roomReferences[7] != -1 && stockage.currentsRoom[roomReferences[7]].type == RoomType.Room) buttomLeftCorner.SetActive(false);

        graphics.enabled = false;
    }
}