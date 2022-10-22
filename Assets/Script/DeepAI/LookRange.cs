using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRange : MonoBehaviour
{
    [SerializeField, Header("½d³ò")]
    private float fovRange = 6f;
    [SerializeField, Header("R")]
    private float R;
    [SerializeField, Header("G")]
    private float G=255;
    [SerializeField, Header("B")]
    private float B;
    [SerializeField, Header("A")]
    private float A=1;
    [SerializeField, Header("LayerMask")]
    private LayerMask layerMask;
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(R,G,B,A);
        Gizmos.DrawWireSphere(transform.position, fovRange);
    }
    private void Start()
    {
        
    }
    private void Awake()
    {
        //print($"layerMask:{layerMask.value}");
    }
}
