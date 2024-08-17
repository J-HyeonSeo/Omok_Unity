using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    private static GameManager instance = null;

    // �׻� MainScene�� ��ü�� ������ �ǵ��� ����.
    void Awake()
    {
        // ���� �ν��Ͻ��� ����ִٸ�..
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else // ���� �ν��Ͻ��� ������� �ʴٸ�, (���Ӿ����� ���ξ����� �Ѿ�� ���)
        {
            // ���� �ڱ��ڽ� �ν��Ͻ��� ������.
            Destroy(instance.gameObject); 

            // ���� �Ѿ�� ���ξ��� ���ӿ�����Ʈ�� ���� �ν��Ͻ��� ������.
            instance = this;

            // ���� �Ѿ�� ���ξ��� ���ӸŴ��� �ν��Ͻ��� �� ��ȯ���� �������� �ʵ��� ��.
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(GetRoomWaitRoomList());
        InitializeBoard();
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
    public State state;
    [HideInInspector]
    public Piece[] board = new Piece[15 * 15];
    [HideInInspector]
    public string roomId;
    [HideInInspector]
    public float remainTime;
    [HideInInspector]
    public bool isGameDone = false;

    // �ܺο��� ����� ���� �ν��Ͻ� ����
    public Transform roomContentArea; // �� ��� Content �θ� UI
    public GameObject backDrop; // ��� BackDrop
    public InputField roomTitleForCreateRoomField; // �� �̸� ��ǲ�ʵ�(�����)
    public InputField playerNameForCreateRoomField; // �÷��̾�� ��ǲ�ʵ�(�����)
    public InputField playerNameForEnterRoomField; // �÷��̾�� ��ǲ�ʵ�(������)

    // ���Ӱ� ������� �ϴ� ������ ��ü ����
    public GameObject roomContent; // �� ��� �� �ϳ��� ��ü�� �� ������

    // ���â�� Open ����� �ڽ��ε����� �Է¹޾� �ش� �ϴ� ���� Open��Ŵ.
    public void OpenModal(int siblingIndex)
    {
        //BackDrop Ȱ��ȭ
        backDrop.SetActive(true);

        //Modal �θ� ��ü ��������
        Transform mainModal = backDrop.transform.GetChild(0);

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
        backDrop.SetActive(false);
    }

    // �游���
    public void CreateRoom()
    {
        // �� ����, �÷��̾���� �����;� ��.
        string roomTitle = roomTitleForCreateRoomField.text;
        playerName = playerNameForCreateRoomField.text;

        // Post ��û�� ���� roomId�� accessToken�� ȹ���ؾ� ��.
        string requestBody = JsonConvert.SerializeObject(new CreateRoomRequest(roomTitle, playerName));

        RestConnector.Instance.Request("/room/public", requestBody, "POST", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;
                playerId = response.playerId;

                // �� �÷��̾�� ����
                piece = Piece.BLACK;
                InitializeBoard();

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
        playerName = playerNameForEnterRoomField.text;

        // Patch ��û�� ���� accessToken�� ȹ���ؾ� ��.
        string requestBody = JsonConvert.SerializeObject(new EnterRoomRequest(roomId, playerName));

        RestConnector.Instance.Request("/room/public/enter", requestBody, "PATCH", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                RoomCreateAndEnterResponse response = JsonConvert.DeserializeObject<RoomCreateAndEnterResponse>(webRequest.downloadHandler.text);
                roomId = response.roomId;
                accessToken = response.accessToken;
                playerId = response.playerId;

                // ȭ��Ʈ �÷��̾�� ����
                piece = Piece.WHITE;
                InitializeBoard();

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

    // ���� ȭ������ ���ư���.
    public void GoToMain()
    {
        SceneManager.LoadScene("Main");
    }

    // ���� �ҷ����� ( 10�ʿ� �� ���� refresh )
    IEnumerator GetRoomWaitRoomList()
    {
        while (true)
        {
            // �� ����� �� ���� UI ��������
            // ���� UI�� �ڽ� �ν��Ͻ� ���� �ϱ�.
            for (int i = roomContentArea.childCount - 1; i >= 0; i--)
            {
                Destroy(roomContentArea.GetChild(i).gameObject);
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
                        GameObject instanceContent = Instantiate(roomContent, roomContentArea);
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

            yield return new WaitForSeconds(3f);
        }

    }

    // ���� �� �α�
    public void PutPiece(int x, int y)
    {
        string requestBody = JsonConvert.SerializeObject(new PutPieceRequest(roomId, x, y));

        RestConnector.Instance.Request("/game", requestBody, "PATCH", (webRequest) =>
        {
            if (webRequest.responseCode == 200)
            {

                Debug.Log("����!");

            }
            else
            {
                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }
        }, accessToken);
    }

    // ���� �ʱ�ȭ
    private void InitializeBoard()
    {
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                board[IX(x, y)] = Piece.NONE;
            }
        }
    }

    public int IX(int x, int y)
    {
        return x * 15 + y;
    }

}
