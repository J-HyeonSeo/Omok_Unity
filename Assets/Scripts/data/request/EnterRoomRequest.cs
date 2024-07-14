using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoomRequest
{
    public string roomId;
    public string playerName;

    public EnterRoomRequest(string roomId, string playerName)
    {
        this.roomId = roomId;
        this.playerName = playerName;
    }
}
