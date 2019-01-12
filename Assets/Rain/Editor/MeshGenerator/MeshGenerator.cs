using System.Collections;
using System.Collections.Generic;
using TriangleNet;
using TriangleNet.Geometry;
using UnityEditor;
using UnityEngine;

public class MeshGenerator : ScriptableObject
{
    [SerializeField] private string _MeshPath;
    [SerializeField] private int _MeshSideVerticesCount;
    [SerializeField] private float _Size = 1;
    
    public void GenerateAndSaveMesh(MeshType meshType)
    {
        Mesh mesh = null;
        switch (meshType)
        {
            case MeshType.Quad:
                mesh = GenerateQuadMesh(_MeshSideVerticesCount, _Size);
                break;
            case MeshType.Circle:
                mesh = GenerateCircleMesh(_MeshSideVerticesCount, _Size);
                break;
            case MeshType.ParabolicCircle:
                mesh = GenerateParabolicCircleMesh(_MeshSideVerticesCount, _Size);
                break;
        }

        var verts = mesh.vertices;
        var uvs = new Vector2[verts.Length];
        for (int j = 0; j < verts.Length; j++)
        {
            uvs[j] = ((Vector2)verts[j] + _Size * Vector2.one)/ (2 * _Size);
        }

        mesh.uv = uvs;
        AssetDatabase.CreateAsset(mesh, _MeshPath);
        AssetDatabase.SaveAssets();
    }
    private Mesh GenerateQuadMesh(int sideVertsCount, float scale)
    {
        Polygon polygon = new Polygon(); 
        for (int i = 0; i < sideVertsCount; i++)
        {
            for (int j = 0; j < sideVertsCount; j++)
            {
                var x = (float) (i + 1) / sideVertsCount;
                var y = (float) (j + 1) / sideVertsCount;
                var pos = new Vector2(x, y);
                polygon.Add((pos - Vector2.one * 0.5f) * scale);
            }
        }
        var mesh = ((TriangleNetMesh) polygon.Triangulate()).GenerateUnityMesh();

        return mesh;
    }
    
    private Mesh GenerateCircleMesh(int sideVertsCount, float scale)
    {
        Polygon polygon = new Polygon();
       
        float radiusStep = _Size / sideVertsCount;

        int  i = 0;
        List<Vector2> contour = new List<Vector2>();
        for (float radius = radiusStep; radius <= _Size; radius += radiusStep)
        {
            contour.Clear();
            float roundStep = Mathf.PI * 2 / (i + 1);
            float phase = 0;
            for (float angle = phase; angle <= Mathf.PI * 2 + phase; angle += roundStep)
            {
                var pos = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)) + Vector2.one * 0.5f;
                contour.Add((pos - Vector2.one * 0.5f) * scale);
            }
            polygon.Add(contour);
            i++;
        }
        var mesh = ((TriangleNetMesh) polygon.Triangulate()).GenerateUnityMesh();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
   
        return mesh;
    }
    private Mesh GenerateParabolicCircleMesh(int sideVertsCount, float scale)
    {
        Polygon polygon = new Polygon();
       
        float radiusStep = _Size / sideVertsCount;
        int  j = 0;
        var vertsZ = new float[sideVertsCount * (sideVertsCount+ 1)];
       
        for (float radius = radiusStep; radius <= _Size; radius += radiusStep)
        {
            float roundStep = Mathf.PI * 2 / sideVertsCount;
            
            for (float angle = 0; angle <= Mathf.PI * 2; angle += roundStep)
            {
                vertsZ[j] = (_Size - radius) * (_Size - radius) / _Size;
                
                var pos = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle)) + Vector2.one * 0.5f;
                polygon.Add((pos - Vector2.one * 0.5f) * scale);
                j++;
            }
        }
        var mesh = ((TriangleNetMesh) polygon.Triangulate()).GenerateUnityMesh();
        var verts = mesh.vertices;
        for (int k = 0; k < verts.Length; k++)
        {
            verts[k].z = vertsZ[k];
        }

        mesh.vertices = verts;
        return mesh;
    }
    public enum MeshType
    {
        Quad,
        Circle,
        ParabolicCircle
    }
}
[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorInspector : Editor
{
    private MeshGenerator _Generator;
    private void OnEnable()
    {
        _Generator = target as MeshGenerator;
    }

    public override void OnInspectorGUI()
    {      
        base.OnInspectorGUI();
  
        if (GUILayout.Button("Generate Quad Mesh"))
        {
            _Generator.GenerateAndSaveMesh(MeshGenerator.MeshType.Quad);
        }
        if (GUILayout.Button("Generate Circle Mesh"))
        {
            _Generator.GenerateAndSaveMesh(MeshGenerator.MeshType.Circle);
        }
        if (GUILayout.Button("Generate Parabolic Circle Mesh"))
        {
            _Generator.GenerateAndSaveMesh(MeshGenerator.MeshType.ParabolicCircle);
        }
    }
}