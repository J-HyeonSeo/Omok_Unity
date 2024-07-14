using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OmokEventHandler : MonoBehaviour
{

    public GameObject blackPiece;
    public GameObject whitePiece;

    public GameObject transBlackPiece;
    public GameObject transWhitePiece;
    public Text blackPlayerNameText;
    public Text whitePlayerNameText;
    public Text blackPlayerTimeText;
    public Text whitePlayerTimeText;

    GameObject tempTransPiece;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    float xStep;
    float yStep;
    private readonly float eventArea = 0.2f;
    private readonly int N = 15;

    private void Start()
    {

        xStep = (maxX - minX) / (N - 1);
        yStep = (maxY - minY) / (N - 1);

        // 본인 이름 변경
        if (GameManager.Instance.piece == Piece.BLACK)
        {
            blackPlayerNameText.text = GameManager.Instance.playerName;
        } else
        {
            whitePlayerNameText.text = GameManager.Instance.playerName;
        }

        // PIECE 할당
        tempTransPiece = Instantiate(GameManager.Instance.piece == Piece.BLACK ? transBlackPiece : transWhitePiece);
        tempTransPiece.SetActive(false);

        StartCoroutine(PollingGameData());
    }

    // Update is called once per frame
    void Update()
    {

        // 마우스의 현재 화면 위치를 가져옵니다.
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        for (float x = minX; x <= maxX; x += xStep)
        {
            for (float y = minY; y <= maxY; y += yStep)
            {
                
                if (worldPosition.x > x - eventArea && worldPosition.x < x + eventArea
                    && worldPosition.y > y - eventArea && worldPosition.y < y + eventArea)
                {
                    tempTransPiece.transform.position = new Vector2(x, y);
                    tempTransPiece.SetActive(true);
                    return;
                }

            }
        }

        tempTransPiece.SetActive(false);

    }

    void OnMouseDown()
    {
        // 마우스의 현재 화면 위치를 가져옵니다.
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {

                float x = minX + i * xStep;
                float y = minY + j * yStep;

                int row = N - j - 1;
                int col = i;

                if (worldPosition.x > x - eventArea && worldPosition.x < x + eventArea
                    && worldPosition.y > y - eventArea && worldPosition.y < y + eventArea)
                {

                    Debug.Log("row => " + row + ", col => " + col);

                    return;
                }

            }
        }

    }

    // 1초마다 게임 데이터를 폴링함.
    IEnumerator PollingGameData()
    {
        RestConnector.Instance.GetRequest("/game/" + GameManager.Instance.roomId, (webRequest) =>
        {

            if (webRequest.responseCode == 200)
            {

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };

                Debug.Log(webRequest.downloadHandler.text);

                GameData gameData = JsonConvert.DeserializeObject<GameData>(webRequest.downloadHandler.text, settings);

                Debug.Log(gameData.roomId);
                Debug.Log(gameData.roomTitle);
                Debug.Log(gameData.nowState);
                Debug.Log(gameData.otherPlayerName);
                Debug.Log(gameData.turnedAt);
                Debug.Log(gameData.winnerPlayerId);
                Debug.Log(gameData.board);

            } else
            {

                ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                Debug.Log(response.message);
            }

        }, GameManager.Instance.accessToken);

        yield return new WaitForSeconds(1f);
    }

}
