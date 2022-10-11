using System;
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
            case -1:
                GetComponent<Image>().color = Color.gray;
                break;
            case 1:
                GetComponent<Image>().color = Color.red;
                break;
            case 2:
                GetComponent<Image>().color = Color.yellow;
                break;
            case 3:
                GetComponent<Image>().color = new Color(239 / 256f, 114 / 256f, 21 / 256f);
                break;
        }
    }

    private float range = 160;

    private bool isVertical;
    private bool setDraged;
    private float EPS = 0.03f;
    private Vector2 delta;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (value <= 0) return;
        setDraged = false;
        delta = Camera.main.ScreenToWorldPoint(eventData.position) - transform.position;
    }

    Vector2 newPos(PointerEventData eventData)
    {
        return new Vector2(
            Camera.main.ScreenToWorldPoint(eventData.position).x - delta.x,
            Camera.main.ScreenToWorldPoint(eventData.position).y - delta.y
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        var pos = newPos(eventData);
        if (value <= 0) return;
        if (!setDraged
//            || !isVertical && transform.localPosition.x < range / 80
//            || isVertical && transform.localPosition.y < range / 80
           )
        {
            isVertical = !(Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y));
            setDraged = true;
        }

        if (isVertical && transform.InverseTransformPoint(pos).y + eventData.delta.y < 0 && !_gc.isCellAvailable(X, Y + 1)) return;
        if (isVertical && transform.InverseTransformPoint(pos).y + eventData.delta.y > 0 && !_gc.isCellAvailable(X, Y - 1)) return;
        if (!isVertical && transform.InverseTransformPoint(pos).x + eventData.delta.x < 0 && !_gc.isCellAvailable(X - 1, Y)) return;
        if (!isVertical && transform.InverseTransformPoint(pos).x + eventData.delta.x > 0 && !_gc.isCellAvailable(X + 1, Y)) return;

        if (((Vector2) transform.InverseTransformPoint(pos)).magnitude <= range)
        {
            if (transform.localPosition.magnitude > (range / 20 * 19)) CheckAndUpdatePos();
            transform.position = new Vector3(
                !isVertical ? pos.x : transform.position.x,
                isVertical ? pos.y : transform.position.y,
                transform.position.z);
            
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (value <= 0) return;
        if (transform.localPosition.magnitude > range / 8)
        {
            CheckAndUpdatePos();
            _gc.panel.SetActive(true);
        }

        StartCoroutine(MoveToZero());
    }

    void CheckAndUpdatePos()
    {
        if (transform.localPosition.x > 0) updateCell(X + 1, Y);
        else if (transform.localPosition.x < 0) updateCell(X - 1, Y);
        else if (transform.localPosition.y > 0) updateCell(X, Y - 1);
        else if (transform.localPosition.y < 0) updateCell(X, Y + 1);
    }

    void updateCell(int x, int y)
    {
        if (_gc.isCellAvailable(x, y))
        {
            X = x;
            Y = y;
            transform.SetParent(_gc.GetCell(X, Y).transform);
        }
        //setDraged = false;
    }

    IEnumerator MoveToZero()
    {
        var t = new WaitForSeconds(0.01f);

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