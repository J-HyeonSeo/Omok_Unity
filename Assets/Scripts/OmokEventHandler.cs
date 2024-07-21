using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
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

    private void Awake()
    {

        xStep = (maxX - minX) / (N - 1);
        yStep = (maxY - minY) / (N - 1);

        // ���� �̸� ����
        if (GameManager.Instance.piece == Piece.BLACK)
        {
            blackPlayerNameText.text = GameManager.Instance.playerName;
        } else
        {
            whitePlayerNameText.text = GameManager.Instance.playerName;
        }

        // PIECE �Ҵ�
        tempTransPiece = Instantiate(GameManager.Instance.piece == Piece.BLACK ? transBlackPiece : transWhitePiece);
        tempTransPiece.SetActive(false);

        StartCoroutine(PollingGameDataAndProcess());
    }

    // ���� �� ������ �����ϴ� �̺�Ʈ�� ����
    void Update()
    {

        // ���콺�� ���� ȭ�� ��ġ�� �����ɴϴ�.
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if ((GameManager.Instance.state == State.BLACK && GameManager.Instance.piece == Piece.BLACK) ||
            (GameManager.Instance.state == State.WHITE && GameManager.Instance.piece == Piece.WHITE))
        {

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
    }

    void OnMouseDown()
    {
        // ���콺�� ���� ȭ�� ��ġ�� �����ɴϴ�.
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

                    GameManager.Instance.PutPiece(row, col);

                    return;
                }

            }
        }

    }

    // 1�ʸ��� ���� �����͸� �����ϰ�, ����� ���� ó���� ������.
    IEnumerator PollingGameDataAndProcess()
    {
        while (true)
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

                    if (gameData.winnerPlayerId != null && gameData.winnerPlayerId.Equals(GameManager.Instance.playerId))
                    {
                        // �¸� ��� ����

                        return;
                    }

                    if (gameData.winnerPlayerId != null && !gameData.winnerPlayerId.Equals(GameManager.Instance.playerId))
                    {
                        // �й� ��� ����

                        return;
                    }

                    // GameManager�� ���� ���� �Ҵ�.
                    GameManager.Instance.state = gameData.nowState;
                    GameManager.Instance.otherPlayerName = gameData.otherPlayerName;
                    GameManager.Instance.board = gameData.board;

                    // ���� ���� ������
                    RenderBoard();

                    //turnedAt�� ������ remainTime����.
                    DateTime turnedAt = GetDateTime(gameData.turnedAt);
                    GameManager.Instance.remainTime = 20 - (float)(DateTime.Now - turnedAt).TotalSeconds;

                }
                else
                {

                    ErrorResponse response = JsonConvert.DeserializeObject<ErrorResponse>(webRequest.downloadHandler.text);
                    Debug.Log(response.message);
                }

            }, GameManager.Instance.accessToken);

            yield return new WaitForSeconds(1f);
        }
    }

    // ���� Board ���¸� �����Ͽ� ���� ������ ����
    private void RenderBoard()
    {
        // Board���� Clear����
        Transform transform = gameObject.transform;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // GameManager�� Board �迭�� ����, ������ ����
        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                Piece piece = GameManager.Instance.board[GameManager.Instance.IX(x, y)];

                /*int row = N - j - 1;
                int col = i;*/

                Vector2 vector = new Vector2(minX + y * xStep, minY + (N - x - 1) * yStep);

                GameObject putPiece = piece == Piece.BLACK ? blackPiece : piece == Piece.WHITE ? whitePiece : null;

                if (putPiece == null) { continue; }

                GameObject obj = Instantiate(putPiece, gameObject.transform);
                obj.transform.position = vector;
            }
        }
    }

    private DateTime GetDateTime(int[] turnedAt)
    {
        int year = turnedAt[0];
        int month = turnedAt[1];
        int day = turnedAt[2];
        int hour = turnedAt[3];
        int minute = turnedAt[4];
        int second = turnedAt[5];
        int nanoOfSecond = turnedAt[6];

        DateTime dateTime = new DateTime(year, month, day, hour, minute, second);
        dateTime.AddTicks(nanoOfSecond / 100);

        return dateTime;
    }

}
