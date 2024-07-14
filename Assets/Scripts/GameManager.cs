using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;

    // 항상 MainScene의 객체가 메인이 되도록 셋팅.
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
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
    public string accessToken;
    [HideInInspector]
    public string otherPlayerName;
    [HideInInspector]
    public Piece piece;
    [HideInInspector]
    public Piece[] board = new Piece[15 * 15];
    [HideInInspector]
    public string roomId;

    // 외부에서 끌어올 실제 인스턴스 주입
    public Transform RoomContentArea; // 방 목록 Content 부모 UI
    public GameObject BackDrop; // 모달 BackDrop
    public InputField RoomTitleForCreateRoomField; // 방 이름 인풋필드(방생성)
    public InputField PlayerNameForCreateRoomField; // 플레이어명 인풋필드(방생성)
    public InputField PlayerNameForEnterRoomField; // 플레이어명 인풋필드(방입장)

    // 새롭게 만들려고 하는 프리팹 객체 주입
    public GameObject RoomContent; // 방 목록 중 하나의 객체가 될 프리팹

    // 모달창을 Open 모달의 자식인덱스를 입력받아 해당 하는 것을 Open시킴.
    public void OpenModal(int siblingIndex)
    {
        //BackDrop 활성화
        BackDrop.SetActive(true);

        //Modal 부모 객체 가져오기
        Transform mainModal = BackDrop.transform.GetChild(0);

        //현재 siblingIndex에 해당하는 모달만 켜고 나머지는 비활성화 처리.
        for (int i = 0; i < mainModal.childCount; i++)
        {
            if (i == siblingIndex)
            {
                mainModal.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                mainModal.GetChild(i).gameObject.SetActive(false);
            }
        }

    }

    // 모달창을 닫음.
    public void CloseModal()
    {
        BackDrop.SetActive(false);
    }

    // 방만들기
    public void CreateRoom()
    {
        // 방 제목, 플레이어명을 가져와야 함.
        string roomTitle = RoomTitleForCreateRoomField.text;
        playerName = PlayerNameForCreateRoomField.text;

        // Post 요청을 통해 roomId와 accessToken을 획득해야 함.
        string requestBody = JsonConvert.SerializeObject(new CreateRoomRequest(roomTitle, playerName));

        RestConnector.Instance.Request("/room/public", requestBody, "POST", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;

                // 인 게임 씬으로 넘어가기.
                SceneManager.LoadScene("InGame");

            } else
            {
                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }
        });

    }

    // 방 입장하기
    public void EnterRoom()
    {
        // roomId하고, 플레이어명을 가져와야 함.
        playerName = PlayerNameForEnterRoomField.text;

        // Patch 요청을 통해 accessToken을 획득해야 함.
        string requestBody = JsonConvert.SerializeObject(new EnterRoomRequest(roomId, playerName));

        RestConnector.Instance.Request("/room/public/enter", requestBody, "PATCH", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;

                // 인 게임 씬으로 넘어가기.
                SceneManager.LoadScene("InGame");

            }
            else
            {
                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }
        });
    }

    // 방 나가기
    public void ExitRoom()
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
        RestConnector.Instance.GetRequest("/room/public", (webRequest) =>
        {

            if (webRequest.responseCode == 200)
            {
                RoomSimpleData[] response = JsonConvert.DeserializeObject<RoomSimpleData[]>(webRequest.downloadHandler.text);

                // for문으로 돌면서 새롭게 UI 만들어서 추가하기.
                foreach (RoomSimpleData data in response)
                {
                    GameObject instanceContent = Instantiate(RoomContent, RoomContentArea);
                    GameObject titleObject = instanceContent.transform.GetChild(0).gameObject;

                    // 제목 할당.
                    Text roomTitleText = titleObject.GetComponent<Text>();
                    roomTitleText.text = data.roomTitle;

                    // 버튼 이벤트 할당.
                    Button button = instanceContent.GetComponent<Button>();
                    button.onClick.AddListener(() =>
                    {
                        roomId = data.roomId;
                        OpenModal(1);
                    });

                }
            }
        });

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
