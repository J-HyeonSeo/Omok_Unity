using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutPieceRequest
{
    public string roomId;
    public int row;
    public int col;

    public PutPieceRequest(string roomId, int row, int col)
    {
        this.roomId = roomId;
        this.row = row;
        this.col = col;
    }
}
