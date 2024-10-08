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
        if (GameManager.Instance.isGameDone)
        {
            return;
        }

        GameManager.Instance.remainTime -= Time.deltaTime;
        float remainTime = Mathf.Max(GameManager.Instance.remainTime, 0f);

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
