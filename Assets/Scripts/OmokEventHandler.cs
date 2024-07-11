using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OmokEventHandler : MonoBehaviour
{

    public GameObject blackPiece;
    public GameObject whitePiece;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    int N = 15;

    private void Start()
    {

        float xStep = (maxX - minX) / N;
        float yStep = (maxY - minY) / N;

        for (float x = minX; x <= maxX; x += xStep)
        {
            for (float y = minY; y <= maxY; y += yStep)
            {     
                Vector2 nowVector = new Vector2(x, y);

                Instantiate(blackPiece, nowVector, Quaternion.identity);
            }
        }
            
    }

    // Update is called once per frame
    void Update()
    {

        // 마우스의 현재 화면 위치를 가져옵니다.
        Vector2 screenPosition = Input.mousePosition;
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        Debug.Log(worldPosition);

    }

}
