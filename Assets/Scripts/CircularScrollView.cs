using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

public class CircularScrollView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
  public enum Direction
  {
    VERTICAL,
    HORIZONTAL
  };

  public RectTransform rectTransform;

  [SerializeField, Range(-1, 1)]
  private float angle = 1;
  [SerializeField]
  private int arcHeight;
  [SerializeField]
  private float spaceing = 2;

  public Direction direction;

  public List<DragObject> dragObjectArray;

  private float circularR;
  private Vector2 canvasMaxPos;
  private Vector2 lowerBoundPos;
  private Vector2 uperBoundPos;
  private Vector2 circularPostion;

  // 
  private Vector3 startInputPos;
  private Vector3 lastInputPos;
  private Vector3 deltaInputPos;
  private Vector3 upPostion;
  private Vector3 downPostion;
  private Vector3 centerPostion;

  //
  private Vector2 dragObjectBound;

  //
  private Action onEndDragToDo;
  private Action onBeingDragToDo;

  private void Start()
  {
    if (ReferenceEquals(rectTransform, null))
      rectTransform = GetComponent<RectTransform>();

    InitialPosition();
  }

  public DragObject[] SetDragObjectArray
  {
    set
    {
      dragObjectArray = new List<DragObject>(value);
    }
  }

  // 初始化所有物件的位置
  public void InitialPosition()
  {
    GetPostion();
    GetCircular(upPostion, centerPostion, downPostion);

    RectTransform rectTransform = dragObjectArray[0].GetComponent<RectTransform>();
    dragObjectBound = new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);

    float space;

    for (int i = 0; i < dragObjectArray.Count; i++)
    {
      switch (direction)
      {
        case Direction.VERTICAL:
          space = uperBoundPos.y - i * (dragObjectBound.y * 2 + spaceing);

          dragObjectArray[i].transform.localPosition = new Vector3(0, space, 0);

          // X軸偏移
          OffsetX(dragObjectArray[i]);
          break;

        case Direction.HORIZONTAL:
          space = lowerBoundPos.x + i * (dragObjectBound.x * 2 + spaceing);

          dragObjectArray[i].transform.localPosition = new Vector3(space, 0, 0);

          // Y軸偏移
          OffsetY(dragObjectArray[i]);
          break;
      }
    }
  }

  private void GetPostion()
  {
    canvasMaxPos = new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);  // 取得畫面最大邊界(假設當前在畫面中心點)
    uperBoundPos = new Vector2(canvasMaxPos.x, canvasMaxPos.y);
    lowerBoundPos = new Vector2(-canvasMaxPos.x, -canvasMaxPos.y);

    switch (direction)
    {
      case Direction.VERTICAL:
        if (angle >= 1)
        {
          upPostion = new Vector3(lowerBoundPos.x, uperBoundPos.y);
          downPostion = new Vector3(lowerBoundPos.x, lowerBoundPos.y);
          centerPostion = new Vector3(lowerBoundPos.x + arcHeight, 0);
        }
        else
        {
          upPostion = new Vector3(uperBoundPos.x, uperBoundPos.y);
          downPostion = new Vector3(uperBoundPos.x, lowerBoundPos.y);
          centerPostion = new Vector3(uperBoundPos.x - arcHeight, 0);
        }
        break;

      case Direction.HORIZONTAL:
        if (angle >= 1)
        {
          upPostion = new Vector3(lowerBoundPos.x, lowerBoundPos.y);
          downPostion = new Vector3(uperBoundPos.x, lowerBoundPos.y);
          centerPostion = new Vector3(0, lowerBoundPos.y + arcHeight);
        }
        else
        {
          //upPostion = new Vector3(uperBoundPos.x, uperBoundPos.y);
          //downPostion = new Vector3(uperBoundPos.x, lowerBoundPos.y);
          //centerPostion = new Vector3(uperBoundPos.x - arcHeight, 0);
        }
        break;
    }
  }

  // 取得圓形
  private void GetCircular(Vector2 p1, Vector2 p2, Vector2 p3)
  {
    float a = 2 * (p2.x - p1.x);
    float b = 2 * (p2.y - p1.y);
    float c = p2.x * p2.x + p2.y * p2.y - p1.x * p1.x - p1.y * p1.y;
    float d = 2 * (p3.x - p2.x);
    float e = 2 * (p3.y - p2.y);
    float f = p3.x * p3.x + p3.y * p3.y - p2.x * p2.x - p2.y * p2.y;
    float x = (b * f - e * c) / (b * d - e * a);
    float y = (d * c - a * f) / (b * d - e * a);

    circularR = (float)Math.Sqrt((double)((x - p1.x) * (x - p1.x) + (y - p1.y) * (y - p1.y)));  // 圓半徑
    circularPostion = new Vector2(x, y);  // 圓心
  }

  // X軸偏移
  private void OffsetX(DragObject dragObject)
  {
    Vector3 nowPos = dragObject.transform.localPosition;

    double x = angle * Math.Sqrt(Math.Pow(circularR, 2) - Math.Pow((nowPos.y - circularPostion.y), 2));

    if (double.IsNaN(x))
      x = 0;

    x += circularPostion.x;

    dragObject.transform.localPosition = new Vector3((float)x, nowPos.y, nowPos.z);
  }

  // Y軸偏移
  private void OffsetY(DragObject dragObject)
  {
    Vector3 nowPos = dragObject.transform.localPosition;

    double y = angle * Math.Sqrt(Math.Pow(circularR, 2) - Math.Pow((nowPos.x - circularPostion.x), 2));

    if (double.IsNaN(y))
      y = 0;

    y += circularPostion.y;

    dragObject.transform.localPosition = new Vector3(nowPos.x, (float)y, nowPos.z);
  }

  // 更新所有物件位置
  private void UpdateObjectPostion(Vector3 pos)
  {
    for (int i = 0; i < dragObjectArray.Count; i++)
    {
      switch (direction)
      {
        case Direction.VERTICAL:
          dragObjectArray[i].transform.localPosition += new Vector3(0, pos.y, 0);
          OffsetX(dragObjectArray[i]);
          break;

        case Direction.HORIZONTAL:
          dragObjectArray[i].transform.localPosition += new Vector3(pos.x, 0, 0);
          OffsetY(dragObjectArray[i]);
          break;
      }
    }
  }

  // 檢查物件是否超過邊界
  private void CheckObjectPostion(Vector3 pos)
  {
    DragObject dragObject = null;

    // 取得檢查物件
    switch (direction)
    {
      case Direction.VERTICAL:
        if (pos.y > 0)
          dragObject = dragObjectArray[0];  // 向上滑動，檢察第一個
        else if (pos.y < 0)
          dragObject = dragObjectArray[dragObjectArray.Count - 1];  // 向下滑動，檢查最後一個
        else
          return;
        break;

      case Direction.HORIZONTAL:
        if (pos.x > 0)
          dragObject = dragObjectArray[dragObjectArray.Count - 1];  // 向右滑動，檢查最後一個
        else if (pos.x < 0)
          dragObject = dragObjectArray[0];  // 向左滑動，檢察第一個
        else
          return;
        break;
    }

    Vector3 checkPos = dragObject.transform.localPosition;

    // 檢查物件是否超過邊界
    switch (direction)
    {
      case Direction.VERTICAL:
        // 向上滑動，檢查上邊界
        if (pos.y > 0 && checkPos.y - dragObjectBound.y > uperBoundPos.y)
        {
          Vector3 down = dragObjectArray[dragObjectArray.Count - 1].transform.localPosition;

          dragObject.transform.localPosition = new Vector3(checkPos.x,
            down.y - dragObjectBound.y * 2 - spaceing, checkPos.z);

          OffsetX(dragObject);

          // 第一個物件移至最後一個
          dragObjectArray.RemoveAt(0);
          dragObjectArray.Add(dragObject);
        }
        // 向下滑動，檢查下邊界
        else if (pos.y < 0 && checkPos.y + dragObjectBound.y < lowerBoundPos.y)
        {
          Vector3 up = dragObjectArray[0].transform.localPosition;

          dragObject.transform.localPosition = new Vector3(checkPos.x,
            up.y + dragObjectBound.y * 2 + spaceing, checkPos.z);

          OffsetX(dragObject);

          // 最後物件移至一個
          dragObjectArray.RemoveAt(dragObjectArray.Count - 1);
          dragObjectArray.Insert(0, dragObject);
        }
        break;

      case Direction.HORIZONTAL:
        if (pos.x > 0 && checkPos.x - dragObjectBound.x > uperBoundPos.x)
        {
          Vector3 right = dragObjectArray[0].transform.localPosition;

          dragObject.transform.localPosition = new Vector3(
            right.x - dragObjectBound.x * 2 - spaceing, checkPos.y, checkPos.z);

          OffsetY(dragObject);

          // 最後物件移至一個
          dragObjectArray.RemoveAt(dragObjectArray.Count - 1);
          dragObjectArray.Insert(0, dragObject);
        }
        else if (pos.x < 0 && checkPos.x + dragObjectBound.x < lowerBoundPos.x)
        {
          Vector3 left = dragObjectArray[dragObjectArray.Count - 1].transform.localPosition;

          dragObject.transform.localPosition = new Vector3(
            left.x + dragObjectBound.x * 2 + spaceing, checkPos.y, checkPos.z);

          OffsetY(dragObject);

          // 第一個物件移至最後一個
          dragObjectArray.RemoveAt(0);
          dragObjectArray.Add(dragObject);
        }
        break;
    }
  }

  public Action SetOnEndDragToDo { set { onEndDragToDo += value; } }

  public Action RemoveOnEDragToDo { set { onEndDragToDo -= value; } }

  public Action SetOnBeginDragToDo { set { onBeingDragToDo += value; } }

  public Action RemoveOnBeginDragToDo { set { onBeingDragToDo -= value; } }

  #region 觸控事件

  public void OnBeginDrag(PointerEventData eventData)
  {
#if !UNITY_EDITOR
    startInputPos = Input.GetTouch(0).position;
#else
    startInputPos = Input.mousePosition;
#endif

    lastInputPos = startInputPos;

    if (!ReferenceEquals(onBeingDragToDo, null))
      onBeingDragToDo();
  }

  public void OnEndDrag(PointerEventData eventData)
  {
    if (!ReferenceEquals(onEndDragToDo, null))
      onEndDragToDo();
  }

  public void OnDrag(PointerEventData eventData)
  {
    Vector3 currentInputPos;
#if !UNITY_EDITOR
    currentInputPos = Input.GetTouch(0).position;
#else
    currentInputPos = Input.mousePosition;
#endif

    deltaInputPos = currentInputPos - lastInputPos;
    lastInputPos = currentInputPos;

    UpdateObjectPostion(deltaInputPos);
    CheckObjectPostion(deltaInputPos);
  }

  #endregion
}
