using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParticle : MonoBehaviour {


    // Use this for initialization
    void Start () {
        Color AlphaOne = GetComponent<MeshRenderer>().material.color;
        StartCoroutine(Flash());
        StartCoroutine(Delete());	
	}

    //생성된 큐브가 일정 시간 이후 반짝반짝거리도록 설정
    IEnumerator Flash()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            switch (GetComponent<MeshRenderer>().enabled)
            {
                case true:
                    GetComponent<MeshRenderer>().enabled = false; break;
                case false:
                    GetComponent<MeshRenderer>().enabled = true; break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    //큐브 삭제
    IEnumerator Delete()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        yield break;
    }
}
