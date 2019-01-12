using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _Acceleration;
    void Update()
    {
        _Speed += _Acceleration * Time.deltaTime;
        transform.Translate(transform.up * Time.deltaTime * _Speed);
    }
}
