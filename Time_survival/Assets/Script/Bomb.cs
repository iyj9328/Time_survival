using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    public BossPattern boss;
    public float speed = 5f;
    public AudioClip missileSound;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(missileSound, transform.position, 8f);
        transform.rotation = Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position);
    }

    private void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 1, 0), speed * Time.deltaTime);
        
    }


    public void GetDamage()
    {
        GameManager.PlayerHealth += 10;
        BombDestroy();
    }
    public void BombDestroy()
    {
        boss.Obstacles.Remove(this);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.PlayerHealth -= 50;
            GameManager.Instance.StopCoroutine(GameManager.Instance.DamagedScreen());
            GameManager.Instance.StartCoroutine(GameManager.Instance.DamagedScreen());
            GetDamage();
        }
    }
}
