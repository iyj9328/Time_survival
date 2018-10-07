using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Image itemDot;
    Rigidbody rb;
    ParticleSystem ps;
    Image idot;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<ParticleSystem>();
        idot = Instantiate<Image>(itemDot);
        idot.rectTransform.SetParent(GameObject.FindWithTag("Minimap").transform);
        idot.color = Color.green;
        idot.rectTransform.localPosition = new Vector2(transform.position.x, transform.position.z);
        idot.rectTransform.localRotation = Quaternion.identity;
        idot.rectTransform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }

    public void GetDamage()
    {
        GameManager.Instance.OnGetHpCheck = true;
        GameManager.PlayerHealth += 50;
        if (GameManager.PlayerHealth > 500)
            GameManager.PlayerHealth = 500;
        Destroy(idot.gameObject);
        Destroy(gameObject);
    }
}
