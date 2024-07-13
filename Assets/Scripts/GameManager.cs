using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;

    // 최초에 인스턴스 할당 및 중복 할당시 제거 수행.
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

    // 외부에서 게임매니저에 접근할 수 있도록 함. set를 사용하지 않아 새로운 GameManager를 할당 할 수 없게 함.
    public static GameManager Instance
    {
        get { return instance; }
    }

    /*
     * ###################################################################
     * ##################         In-Game-Manage        ##################
     * ###################################################################
     */

    // 게임에서 사용할 변수 정의

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string playerId;
    [HideInInspector]
    public Piece piece;
    [HideInInspector]
    public Piece[] board = new Piece[15 * 15];

    // 외부에서 끌어올 실제 인스턴스 주입
    public Transform RoomContentArea; // 방 목록 Content 부모 UI

    // 새롭게 만들려고 하는 프리팹 객체 주입
    public GameObject RoomContent; // 방 목록 중 하나의 객체가 될 프리팹


    // 방만들기
    public void CreateRoom()
    {

    }

    // 방목록 불러오기 ( 10초에 한 번씩 refresh )
    public void GetRoomWaitRoomList()
    {
        // 방 목록이 될 메인 UI 가져오기
        // 메인 UI의 자식 인스턴스 제거 하기.
        for (int i = RoomContentArea.childCount - 1; i >= 0; i--)
        {
            Destroy(RoomContentArea.GetChild(i).gameObject);
        }

        // 서버에 대기방 목록 조회하기.
        RestConnector.Instance.GetRequest<RoomSimpleData[]>("/room/public", (response) =>
        {         
            Debug.Log(response);
        });

        // for문으로 돌면서 새롭게 UI 만들어서 추가하기.
    }

    //방 입장하기
    public void EnterRoom()
    {

    }

    // 게임 데이터 가져오기
    public void GetGameData()
    {

    }

    // 오목 돌 두기
    public void PutPiece()
    {

    }

}
