using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GunEnd의 위치를 시각적으로 보여주기위한 스크립트
public class GunEndGizmo : MonoBehaviour
{
    public Color gizColor = Color.red;  //GunEnd의 색
    public float gizRadius = 0.1f;      //GunEnd의 지름

    private void OnDrawGizmos()
    {
        Gizmos.color = gizColor;
        Gizmos.DrawSphere(transform.position, gizRadius);
    }
}
