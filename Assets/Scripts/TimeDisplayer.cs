using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplayer : MonoBehaviour
{

    public Piece piece;

    [HideInInspector]
    public Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        GameManager.Instance.remainTime -= Time.deltaTime;
        float remainTime = GameManager.Instance.remainTime;

        if (GameManager.Instance.state == State.BLACK && piece == Piece.BLACK)
        {
            text.text = "Time: " + remainTime.ToString("F2");
            return;
        }

        if (GameManager.Instance.state == State.WHITE && piece == Piece.WHITE)
        {
            text.text = "Time: " + remainTime.ToString("F2");
            return;
        }

        text.text = "-";

    }
}
