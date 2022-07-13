using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value;
    public int X;
    public int Y;
    private GameController _gc;

    public void init(int v, int x, int y, GameController gc)
    {
        value = v;
        X = x;
        Y = y;
        _gc = gc;
        switch (v)
        {
            case -1: GetComponent<Image>().color = Color.gray;
                break;
            case 1: GetComponent<Image>().color = Color.red;
                break;
            case 2: GetComponent<Image>().color = Color.yellow;
                break;
            case 3: GetComponent<Image>().color = new Color(239/256f,114/256f,21/256f);
                break;
        }
    }

    private float range = 100;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (value <= 0) return;
        startDraged = false;
    }

    private bool isVertical;
    private bool startDraged;
    public void OnDrag(PointerEventData eventData)
    {
        
        if (value <= 0) return;
        if (!startDraged)
        {
            if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y)) isVertical = false;
            else isVertical = true;
            startDraged = true;
        }

        if (transform.localPosition.y + eventData.delta.y < 0 && !(Y < 4 && _gc.isCellAvailable(X, Y + 1))) return;
        if (transform.localPosition.y + eventData.delta.y > 0 && !(Y > 0 && _gc.isCellAvailable(X, Y - 1))) return;
        if (transform.localPosition.x + eventData.delta.x < 0 && !(X > 0 && _gc.isCellAvailable(X - 1, Y))) return;
        if (transform.localPosition.x + eventData.delta.x > 0 && !(X < 4 && _gc.isCellAvailable(X + 1, Y))) return;
        

        if(((Vector2)transform.localPosition + eventData.delta).magnitude < range) 
            transform.localPosition += new Vector3(isVertical? 0: eventData.delta.x, isVertical? eventData.delta.y:0,0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (value <= 0) return;
        if(transform.localPosition.magnitude < range/8) transform.localPosition = Vector3.zero;
        else
        {
            if(transform.localPosition.x > 0) updateCell(X+1,Y);
            else if(transform.localPosition.x < 0) updateCell(X - 1, Y);
            else if(transform.localPosition.y > 0) updateCell(X, Y - 1);
            else if(transform.localPosition.y < 0) updateCell(X, Y + 1);
            _gc.panel.SetActive(true);
            StartCoroutine(MoveToZero());
        }
    }

    void updateCell(int x, int y)
    {
        X = x;
        Y = y;
        transform.SetParent(_gc.GetCell(X, Y).transform);
    }

    IEnumerator MoveToZero()
    {
        var t = new WaitForSeconds(0.01f);

        var EPS = 0.1f;
        while (transform.localPosition.magnitude > EPS)
        {
            yield return t;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.3f);
        }

        transform.localPosition = Vector3.zero;
        
        _gc.panel.SetActive(false);
        _gc.CheckField();
    }
}
