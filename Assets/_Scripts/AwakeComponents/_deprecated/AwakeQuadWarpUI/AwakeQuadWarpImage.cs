using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwakeQuadWarpImage : Image 
{
    [SerializeField]
    private Vector2[] _vertices = new[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f) };

    public Vector2[] Vertices
    {
        get { return _vertices; }
        set
        {
            _vertices = value;
            SetVerticesDirty();
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        UIVertex[] uiVertices = new UIVertex[4];
        for (int i = 0; i < 4; ++i)
        {
            uiVertices[i] = UIVertex.simpleVert;
            uiVertices[i].color = color;
            uiVertices[i].uv0 = _vertices[i];
            uiVertices[i].position = new Vector3(_vertices[i].x * rectTransform.rect.width, _vertices[i].y * rectTransform.rect.height);
        }

        vh.AddVert(uiVertices[0]);
        vh.AddVert(uiVertices[1]);
        vh.AddVert(uiVertices[2]);
        vh.AddVert(uiVertices[3]);

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}
