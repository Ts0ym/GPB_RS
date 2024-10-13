using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeBlending : MonoBehaviour
{
    public RectTransform edgeLeft;
    public RectTransform edgeRight;
    public RectTransform edgeTop;
    public RectTransform edgeBottom;
    
    public float Left { get { return edgeLeft.sizeDelta.x; } set { edgeLeft.sizeDelta = new Vector2(Mathf.Round(value), edgeLeft.sizeDelta.y); } }
    public float Right { get { return edgeRight.sizeDelta.x; } set { edgeRight.sizeDelta = new Vector2(Mathf.Round(value), edgeRight.sizeDelta.y); } }
    public float Top { get { return edgeTop.sizeDelta.y; } set { edgeTop.sizeDelta = new Vector2(edgeTop.sizeDelta.x, Mathf.Round(value)); } }
    public float Bottom { get { return edgeBottom.sizeDelta.y; } set { edgeBottom.sizeDelta = new Vector2(edgeBottom.sizeDelta.x, Mathf.Round(value)); } }

    public void SetEdges(float left, float right, float top, float bottom)
    {
        edgeLeft.sizeDelta = new Vector2(left, edgeLeft.sizeDelta.y);
        edgeRight.sizeDelta = new Vector2(right, edgeRight.sizeDelta.y);
        edgeTop.sizeDelta = new Vector2(edgeTop.sizeDelta.x, top);
        edgeBottom.sizeDelta = new Vector2(edgeBottom.sizeDelta.x, bottom);
    }
}
