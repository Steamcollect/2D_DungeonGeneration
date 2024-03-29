using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorData : MonoBehaviour
{
    public int indexInList;
    [HideInInspector] public Stockage stockage;
    SpriteRenderer graphics;

    private void Awake()
    {
        graphics = GetComponent<SpriteRenderer>();
    }

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
        if(roomReferences[1] != -1 ||roomReferences[3] != -1 ||roomReferences[5] != -1 ||roomReferences[7] != -1)
        {
            if (roomReferences[1] != -1 && stockage.currentsRoom[roomReferences[1]].proximityRoomIndex[5] != -1)
            {
                topLeftWall90.SetActive(true);
                topRightWall90.SetActive(true);
            }
            else
            {
                topDoor.SetActive(true);
            }
            if (roomReferences[3] != -1 && stockage.currentsRoom[roomReferences[3]].proximityRoomIndex[7] != -1)
            {
                topRightWall.SetActive(true);
                buttomRightWall.SetActive(true);
            }
            else
            {
                rightDoor.SetActive(true);
            }
            if (roomReferences[5] != -1 && stockage.currentsRoom[roomReferences[5]].proximityRoomIndex[1] != -1)
            {
                buttomRightWall90.SetActive(true);
                buttomLeftWall90.SetActive(true);
            }
            else
            {
                buttomDoor.SetActive(true);
            }
            if (roomReferences[7] != -1 && stockage.currentsRoom[roomReferences[7]].proximityRoomIndex[3] != -1)
            {
                buttomLeftWall.SetActive(true);
                topLeftWall.SetActive(true);
            }
            else
            {
                leftDoor.SetActive(true);
            }
        }

        graphics.enabled = false;

    }
}