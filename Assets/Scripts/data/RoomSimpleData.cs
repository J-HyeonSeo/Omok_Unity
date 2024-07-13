using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSimpleData
{
    public string roomId;
    public string roomTitle;

    public RoomSimpleData() { }
    public RoomSimpleData(string roomId, string roomTitle)
    {
        this.roomId = roomId;
        this.roomTitle = roomTitle;
    }
}
