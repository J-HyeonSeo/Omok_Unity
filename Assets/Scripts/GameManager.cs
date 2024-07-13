using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;

    // ���ʿ� �ν��Ͻ� �Ҵ� �� �ߺ� �Ҵ�� ���� ����.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        GetRoomWaitRoomList();
    }

    // �ܺο��� ���ӸŴ����� ������ �� �ֵ��� ��. set�� ������� �ʾ� ���ο� GameManager�� �Ҵ� �� �� ���� ��.
    public static GameManager Instance
    {
        get { return instance; }
    }

    /*
     * ###################################################################
     * ##################         In-Game-Manage        ##################
     * ###################################################################
     */

    // ���ӿ��� ����� ���� ����

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string playerId;
    [HideInInspector]
    public Piece piece;
    [HideInInspector]
    public Piece[] board = new Piece[15 * 15];

    // �ܺο��� ����� ���� �ν��Ͻ� ����
    public Transform RoomContentArea; // �� ��� Content �θ� UI

    // ���Ӱ� ������� �ϴ� ������ ��ü ����
    public GameObject RoomContent; // �� ��� �� �ϳ��� ��ü�� �� ������


    // �游���
    public void CreateRoom()
    {

    }

    // ���� �ҷ����� ( 10�ʿ� �� ���� refresh )
    public void GetRoomWaitRoomList()
    {
        // �� ����� �� ���� UI ��������
        // ���� UI�� �ڽ� �ν��Ͻ� ���� �ϱ�.
        for (int i = RoomContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(RoomContentArea.GetChild(i).gameObject);
        }

        // ������ ���� ��� ��ȸ�ϱ�.
        RestConnector.Instance.GetRequest<RoomSimpleData[]>("/room/public", (response) =>
        {         
            Debug.Log(response);
        });

        // for������ ���鼭 ���Ӱ� UI ���� �߰��ϱ�.
    }

    //�� �����ϱ�
    public void EnterRoom()
    {

    }

    // ���� ������ ��������
    public void GetGameData()
    {

    }

    // ���� �� �α�
    public void PutPiece()
    {

    }

}
