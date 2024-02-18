using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonCreator : MonoBehaviour
{
    public Vector2 minMaxVerticalRoomPos;
    public Vector2 minMaxHorizontalRoomPos;

    public float delayBetweenAction;

    Stockage stockage;

    private void Awake()
    {
        stockage = GetComponent<Stockage>();
    }

    private void Start()
    {
        SetNewDungeon();
    }

    public void SetNewDungeon()
    {
        ResetAllRoom();

        // Create starting room
        GameObject tmpGO = Instantiate(stockage.startRoom, new Vector2(0, 0), Quaternion.identity);
        tmpGO.transform.SetParent(transform);
        stockage.currentsRoom.Add(new Room(new Vector2(0, 0), RoomType.Room, tmpGO));

        // Generate the rest of the dungeon
        StartCoroutine(GenerateRoom());
    }

    IEnumerator GenerateRoom()
    {
        #region Room Creation
        // Generate room
        for (int i = 0; i < stockage.maxRooms; i++)
        {
            CreateRoom();
            yield return new WaitForSeconds(delayBetweenAction);
        }
        #endregion

        #region Corridor creation
        // Order the room list with the closest to the nearest room from the start
        stockage.currentsRoom = stockage.currentsRoom.OrderBy(element => Vector2.Distance(element.pos, Vector2.zero)).ToList();

        // Get the current count of room
        int currentRoomCount = stockage.currentsRoom.Count;
        // generate corridor
        for (int i = 0; i < currentRoomCount; i++)
        {
            Vector2 currentPos = stockage.currentsRoom[i].pos;

            while (currentPos != Vector2.zero)
            {
                // verif if there is no proximity room closest from the start
                if (!HaveNearestProximityRoom(currentPos))
                {
                    // Create new corridor position
                    if (currentPos.x != 0 && currentPos.y != 0)
                    {
                        int tmp = Random.Range(0, 2);
                        if (tmp == 0)
                        {
                            SetVerticalPos();
                        }
                        else
                        {
                            SetHorizontalPos();
                        }
                    }
                    else if (currentPos.x != 0)
                    {
                        SetVerticalPos();
                    }
                    else if (currentPos.y != 0)
                    {
                        SetHorizontalPos();
                    }
                    void SetVerticalPos()
                    {
                        if (currentPos.x < 0) currentPos.x += 1;
                        else currentPos.x -= 1;
                    }
                    void SetHorizontalPos()
                    {
                        if (currentPos.y < 0) currentPos.y += 1;
                        else currentPos.y -= 1;
                    }

                    // verif if the place already exist
                    int roomFound = FindRoomWithAxialPos(currentPos);
                    if (roomFound == -1)
                    {
                        GameObject tmpGO = Instantiate(stockage.corridor, currentPos, Quaternion.identity);
                        tmpGO.transform.SetParent(transform);
                        tmpGO.GetComponent<CorridorData>().indexInList = stockage.currentsRoom.Count;
                        stockage.currentsRoom.Add(new Room(currentPos, RoomType.Corridor, tmpGO));

                        yield return new WaitForSeconds(delayBetweenAction);
                    }
                    else if (roomFound >= stockage.maxRooms + 1)
                    {
                        // Set the current pos to "0" to stop the "while"
                        currentPos = Vector2.zero;
                    }

                }
                // Set the current pos to "0" to stop the "while"
                else currentPos = Vector2.zero;
            }
        }
        #endregion

        #region Dungeon visual
        for (int i = 0; i < stockage.currentsRoom.Count; i++)
        {
            Vector2 pos = stockage.currentsRoom[i].pos;

            // Get every proximity room around the current room
            int[] tmp = new int[8];
            tmp[0] = FindRoomWithAxialPos(new Vector2(pos.x - 1, pos.y + 1));
            tmp[1] = FindRoomWithAxialPos(new Vector2(pos.x, pos.y + 1));
            tmp[2] = FindRoomWithAxialPos(new Vector2(pos.x + 1, pos.y + 1));
            tmp[3] = FindRoomWithAxialPos(new Vector2(pos.x + 1, pos.y));
            tmp[4] = FindRoomWithAxialPos(new Vector2(pos.x + 1, pos.y - 1));
            tmp[5] = FindRoomWithAxialPos(new Vector2(pos.x, pos.y - 1));
            tmp[6] = FindRoomWithAxialPos(new Vector2(pos.x - 1, pos.y - 1));
            tmp[7] = FindRoomWithAxialPos(new Vector2(pos.x - 1, pos.y));

            stockage.currentsRoom[i].proximityRoomIndex = tmp;
        }

        // Set unnecessary path
        for (int i = 0; i < stockage.currentsRoom.Count; i++)
        {
            if (stockage.currentsRoom[i].type == RoomType.Corridor) CheckExcessCorridor(i);
        }
        // Remove alone path
        for (int i = 0; i < stockage.currentsRoom.Count; i++)
        {
            if (stockage.currentsRoom[i].type == RoomType.Corridor) RemoveExcessCorridor(i);
        }

        // Set the visual
        for (int i = 0; i < stockage.currentsRoom.Count; i++)
        {
            if (stockage.currentsRoom[i].type == RoomType.Room)
            {
                RoomData current = stockage.currentsRoom[i].go.GetComponent<RoomData>();
                current.stockage = stockage;
                current.SetRoomVisual(stockage.currentsRoom[i].proximityRoomIndex);

            }
            else if (stockage.currentsRoom[i].type == RoomType.Corridor)
            {
                CorridorData current = stockage.currentsRoom[i].go.GetComponent<CorridorData>();
                current.stockage = stockage;
                current.SetRoomVisual(stockage.currentsRoom[i].proximityRoomIndex);
            }
            yield return new WaitForSeconds(delayBetweenAction);
        }
        #endregion
    }
    void CreateRoom()
    {
        Vector2 tmpPos =new Vector2(
            (int)Random.Range(minMaxHorizontalRoomPos.x, minMaxHorizontalRoomPos.y),
            (int)Random.Range(minMaxVerticalRoomPos.x, minMaxVerticalRoomPos.y));
        if (FindRoomWithAxialPos(tmpPos) != -1)
        {
            CreateRoom();
            return;
        }
        else
        {
            GameObject tmpGO = Instantiate(stockage.room, tmpPos, Quaternion.identity);
            tmpGO.transform.SetParent(transform);
            tmpGO.GetComponent<RoomData>().indexInList = stockage.currentsRoom.Count;
            stockage.currentsRoom.Add(new Room(tmpPos, RoomType.Room, tmpGO));
        }
    }

    void CheckExcessCorridor(int i)
    {
        if (stockage.currentsRoom[i].proximityRoomIndex[3] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[3]].type == RoomType.Corridor
            && stockage.currentsRoom[i].proximityRoomIndex[1] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[3]].proximityRoomIndex[1] != -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[3]].proximityRoomIndex[7] = -1;
            stockage.currentsRoom[i].proximityRoomIndex[3] = -1;
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[5] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[5]].type == RoomType.Corridor
            && stockage.currentsRoom[i].proximityRoomIndex[3] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[5]].proximityRoomIndex[3] != -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[5]].proximityRoomIndex[1] = -1;
            stockage.currentsRoom[i].proximityRoomIndex[5] = -1;
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[7] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[7]].type == RoomType.Corridor
            && stockage.currentsRoom[i].proximityRoomIndex[5] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[7]].proximityRoomIndex[5] != -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[7]].proximityRoomIndex[3] = -1;
            stockage.currentsRoom[i].proximityRoomIndex[7] = -1;
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[1] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[1]].type == RoomType.Corridor
            && stockage.currentsRoom[i].proximityRoomIndex[7] != -1 && stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[1]].proximityRoomIndex[7] != -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[1]].proximityRoomIndex[5] = -1;
            stockage.currentsRoom[i].proximityRoomIndex[1] = -1;
        }
    }
    void RemoveExcessCorridor(int i)
    {
        if (stockage.currentsRoom[i].proximityRoomIndex[1] != -1 && stockage.currentsRoom[i].proximityRoomIndex[3] == -1 && stockage.currentsRoom[i].proximityRoomIndex[5] == -1 && stockage.currentsRoom[i].proximityRoomIndex[7] == -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[1]].proximityRoomIndex[5] = -1;
            ClearProximityRoom();
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[1] == -1 && stockage.currentsRoom[i].proximityRoomIndex[3] != -1 && stockage.currentsRoom[i].proximityRoomIndex[5] == -1 && stockage.currentsRoom[i].proximityRoomIndex[7] == -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[3]].proximityRoomIndex[7] = -1;
            ClearProximityRoom();
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[1] == -1 && stockage.currentsRoom[i].proximityRoomIndex[3] == -1 && stockage.currentsRoom[i].proximityRoomIndex[5] != -1 && stockage.currentsRoom[i].proximityRoomIndex[7] == -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[5]].proximityRoomIndex[1] = -1;
            ClearProximityRoom();
        }
        if (stockage.currentsRoom[i].proximityRoomIndex[1] == -1 && stockage.currentsRoom[i].proximityRoomIndex[3] == -1 && stockage.currentsRoom[i].proximityRoomIndex[5] == -1 && stockage.currentsRoom[i].proximityRoomIndex[7] != -1)
        {
            stockage.currentsRoom[stockage.currentsRoom[i].proximityRoomIndex[7]].proximityRoomIndex[3] = -1;
            ClearProximityRoom();
        }

        void ClearProximityRoom()
        {
            for (int y = 0; y < 8; y++)
            {
                stockage.currentsRoom[i].proximityRoomIndex[y] = -1;
            }
        }
    }

    bool HaveNearestProximityRoom(Vector2 pos)
    {
        int left = FindRoomWithAxialPos(new Vector2(pos.x - 1, pos.y));
        if (left != -1 && stockage.currentsRoom[left].pos == new Vector2(pos.x - 1, pos.y)
            && Vector2.Distance(new Vector2(pos.x - 1, pos.y), Vector2.zero) < Vector2.Distance(pos, Vector2.zero))
            return true;
        
        int right = FindRoomWithAxialPos(new Vector2(pos.x + 1, pos.y));
        if (right != -1 && stockage.currentsRoom[right].pos == new Vector2(pos.x + 1, pos.y)
            && Vector2.Distance(new Vector2(pos.x + 1, pos.y), Vector2.zero) < Vector2.Distance(pos, Vector2.zero))
            return true;
        
        int top = FindRoomWithAxialPos(new Vector2(pos.x, pos.y + 1));
        if (top != -1 && stockage.currentsRoom[top].pos == new Vector2(pos.x, pos.y + 1)
            && Vector2.Distance(new Vector2(pos.x, pos.y + 1), Vector2.zero) < Vector2.Distance(pos, Vector2.zero))
            return true;
        
        int buttom = FindRoomWithAxialPos(new Vector2(pos.x, pos.y - 1));
        if (buttom != -1 && stockage.currentsRoom[buttom].pos == new Vector2(pos.x, pos.y - 1)
            && Vector2.Distance(new Vector2(pos.x, pos.y - 1), Vector2.zero) < Vector2.Distance(pos, Vector2.zero))
            return true;

        return false;
    }
    int FindRoomWithAxialPos(Vector2 pos)
    {
        for (int i = 0; i < stockage.currentsRoom.Count; i++)
        {
            if (pos == stockage.currentsRoom[i].pos) return i;
        }

        return -1;
    }

    void ResetAllRoom()
    {
        foreach (Room current in stockage.currentsRoom)
        {
            Destroy(current.go);
        }
        stockage.currentsRoom.Clear();
    }
}