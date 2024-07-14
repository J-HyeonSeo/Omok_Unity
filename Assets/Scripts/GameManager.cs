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

    // �׻� MainScene�� ��ü�� ������ �ǵ��� ����.
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
    public string accessToken;
    [HideInInspector]
    public string otherPlayerName;
    [HideInInspector]
    public Piece piece;
    [HideInInspector]
    public Piece[] board = new Piece[15 * 15];
    [HideInInspector]
    public string roomId;

    // �ܺο��� ����� ���� �ν��Ͻ� ����
    public Transform RoomContentArea; // �� ��� Content �θ� UI
    public GameObject BackDrop; // ��� BackDrop
    public InputField RoomTitleForCreateRoomField; // �� �̸� ��ǲ�ʵ�(�����)
    public InputField PlayerNameForCreateRoomField; // �÷��̾�� ��ǲ�ʵ�(�����)
    public InputField PlayerNameForEnterRoomField; // �÷��̾�� ��ǲ�ʵ�(������)

    // ���Ӱ� ������� �ϴ� ������ ��ü ����
    public GameObject RoomContent; // �� ��� �� �ϳ��� ��ü�� �� ������

    // ���â�� Open ����� �ڽ��ε����� �Է¹޾� �ش� �ϴ� ���� Open��Ŵ.
    public void OpenModal(int siblingIndex)
    {
        //BackDrop Ȱ��ȭ
        BackDrop.SetActive(true);

        //Modal �θ� ��ü ��������
        Transform mainModal = BackDrop.transform.GetChild(0);

        //���� siblingIndex�� �ش��ϴ� ��޸� �Ѱ� �������� ��Ȱ��ȭ ó��.
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

    // ���â�� ����.
    public void CloseModal()
    {
        BackDrop.SetActive(false);
    }

    // �游���
    public void CreateRoom()
    {
        // �� ����, �÷��̾���� �����;� ��.
        string roomTitle = RoomTitleForCreateRoomField.text;
        playerName = PlayerNameForCreateRoomField.text;

        // Post ��û�� ���� roomId�� accessToken�� ȹ���ؾ� ��.
        string requestBody = JsonConvert.SerializeObject(new CreateRoomRequest(roomTitle, playerName));

        RestConnector.Instance.Request("/room/public", requestBody, "POST", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;

                // �� ���� ������ �Ѿ��.
                SceneManager.LoadScene("InGame");

            } else
            {
                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }
        });

    }

    // �� �����ϱ�
    public void EnterRoom()
    {
        // roomId�ϰ�, �÷��̾���� �����;� ��.
        playerName = PlayerNameForEnterRoomField.text;

        // Patch ��û�� ���� accessToken�� ȹ���ؾ� ��.
        string requestBody = JsonConvert.SerializeObject(new EnterRoomRequest(roomId, playerName));

        RestConnector.Instance.Request("/room/public/enter", requestBody, "PATCH", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;

                // �� ���� ������ �Ѿ��.
                SceneManager.LoadScene("InGame");

            }
            else
            {
                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }
        });
    }

    // �� ������
    public void ExitRoom()
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
        RestConnector.Instance.GetRequest("/room/public", (webRequest) =>
        {

            if (webRequest.responseCode == 200)
            {
                RoomSimpleData[] response = JsonConvert.DeserializeObject<RoomSimpleData[]>(webRequest.downloadHandler.text);

                // for������ ���鼭 ���Ӱ� UI ���� �߰��ϱ�.
                foreach (RoomSimpleData data in response)
                {
                    GameObject instanceContent = Instantiate(RoomContent, RoomContentArea);
                    GameObject titleObject = instanceContent.transform.GetChild(0).gameObject;

                    // ���� �Ҵ�.
                    Text roomTitleText = titleObject.GetComponent<Text>();
                    roomTitleText.text = data.roomTitle;

                    // ��ư �̺�Ʈ �Ҵ�.
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

    // ���� ������ ��������
    public void GetGameData()
    {

    }

    // ���� �� �α�
    public void PutPiece()
    {

    }

}
