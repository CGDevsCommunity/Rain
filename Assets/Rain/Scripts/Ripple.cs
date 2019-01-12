using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Ripple : MonoBehaviour
{
    [SerializeField] private float _AnimTime;
    [SerializeField] private Pole[] _Poles;
    [SerializeField] private int _PoleCount = 10;
    private MeshRenderer _MeshRenderer;
    private Material _Material;

    private int _TimerPropertyId;
    private void Start()
    {
       
        _MeshRenderer = GetComponent<MeshRenderer>();
        _Material = _MeshRenderer.material;
        _TimerPropertyId = Shader.PropertyToID("_Timer");
        var poleTexGenerator = new PoleTextureGenerator();
        GeneratePoles();
        _Material.SetTexture("_PoleTexture", poleTexGenerator.GeneratePoleTexture(_Poles));
        StartCoroutine(RippleAnimationCoroutine());
    }

    private void GeneratePoles()
    {
        _Poles = new Pole[_PoleCount];
        for (int i = 0; i < _PoleCount; i++)
        {
            _Poles[i] = new Pole()
            {
                Position = new Vector2(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f)),
                Phase = Random.Range(0, 1f),
                Radius = Random.Range(0.005f, 0.02f)
            };
        }
    }
    private IEnumerator RippleAnimationCoroutine()
    {
        while (true)
        {
            float timer = 0;
            while (timer < _AnimTime)
            {
                _Material.SetFloat(_TimerPropertyId,  timer / _AnimTime);
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }
}
  
