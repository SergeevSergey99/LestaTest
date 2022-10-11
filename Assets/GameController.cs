using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    private GameObject[,] field = new GameObject[5, 5];

    public GameObject prefab;
    public GameObject panel;
    public GameObject winPanel;

    private void Awake()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                field[x, y] = transform.GetChild(x + y * 5).gameObject;
            }
        }

        GenerateField();
    }

    public bool isCellAvailable(int x, int y)
    {
        if (x is < 0 or > 4 || y is < 0 or > 4) return false;
        return field[x, y].transform.childCount == 0;
    }

    public GameObject GetCell(int x, int y)
    {
        return field[x, y];
    }

    void SetBlock(int x, int y, int v)
    {
        Instantiate(prefab, field[x, y].transform).GetComponent<Block>().init(v, x, y, this);
    }

    bool isFieldWin()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (field[i * 2, y].transform.childCount != 1 ||
                    field[i * 2, y].transform.GetChild(0).GetComponent<Block>().value != i + 1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void CheckField()
    {
        if(isFieldWin()) winPanel.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ReGenerateField()
    {
        for (int y = 0; y < 5; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (field[x, y].transform.childCount != 0) Destroy(field[x, y].transform.GetChild(0).gameObject);
            }
        }

        StartCoroutine(NextFrame());

    }

    IEnumerator NextFrame()
    {
        yield return new WaitForFixedUpdate();
        GenerateField();
    }
    

    void GenerateField()
    {
        SetBlock(1, 0, -1);
        SetBlock(3, 0, -1);
        SetBlock(1, 2, -1);
        SetBlock(3, 2, -1);
        SetBlock(1, 4, -1);
        SetBlock(3, 4, -1);

        for (int j = 1; j <= 3; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                int x = Random.Range(0, 5);
                int y = Random.Range(0, 5);
                if (field[x, y].transform.childCount == 0) SetBlock(x, y, j);
                else i--;
            }
        }
        if(isFieldWin()) ReGenerateField();
    }
}