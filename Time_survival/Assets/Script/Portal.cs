using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public Enemy parent;
    [HideInInspector]
    public float shrinkTiming = 2f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(Expand());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Expand()
    {
        if (parent.Type == Enemy.EnemyType.Boss)
        {
            transform.localScale = new Vector3(4, 5, 4);
            transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
        }
        while (true)
        {
            transform.localScale += new Vector3(0, 0.1f, 0);
            if (transform.localScale.y < 1)
            {
                yield return new WaitForSeconds(0.01f);
            }
            else
            {
                StartCoroutine(Shrink());
                yield break;
            }

        }
    }
    IEnumerator Shrink()
    {
        yield return new WaitForSeconds(shrinkTiming);
        while (true)
        {
            transform.localScale -= new Vector3(0, 0.1f, 0);
            yield return new WaitForSeconds(0.01f);
            if (transform.localScale.y < 0.1)
            {
                Destroy(gameObject);
                yield break;
            }

        }
    }
}
