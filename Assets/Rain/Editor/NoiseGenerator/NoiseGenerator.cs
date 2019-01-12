using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NoiseGenerator : ScriptableObject
{
    [SerializeField] private string _NoisePath;
    [SerializeField] private Vector2Int _NoiseResolution;
    [SerializeField] private float _NoiseScale;
    [SerializeField] private bool _IsTextureTransparent;

    private Vector2 _Offset;

    public void GenerateNoiseTexture()
    {
        var texture = new Texture2D(_NoiseResolution.x, _NoiseResolution.y);
        RandomiseNoise(texture);
        File.WriteAllBytes(_NoisePath, texture.EncodeToPNG());
        AssetDatabase.SaveAssets();
    }
    private void RandomiseNoise(Texture2D texture)
    {
        _Offset = new Vector2(Random.Range(0, 10f), Random.Range(0, 10f));
        for (int x = 0; x < _NoiseResolution.x; x++)
        {
            for (int y = 0; y < _NoiseResolution.y; y++)
            {
                float sample = Mathf.PerlinNoise(
                    _Offset.x + x / _NoiseScale / _NoiseResolution.x,
                    _Offset.y + y / _NoiseScale / _NoiseResolution.y);
                texture.SetPixel(x, y,
                    _IsTextureTransparent ? new Color(1, 1, 1, sample) : new Color(sample, sample, sample));
            }
        }
        texture.Apply();
    }
}

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorInspector : Editor
{
    private NoiseGenerator _Generator;
    private void OnEnable()
    {
        _Generator = target as NoiseGenerator;
    }

    public override void OnInspectorGUI()
    {      
        base.OnInspectorGUI();
  
        if (GUILayout.Button("Generate NoiseTexture"))
        {
            _Generator.GenerateNoiseTexture();
        }
    }
}