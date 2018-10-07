using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BulletCtrl : MonoBehaviour
{
    public string poolItemName = "bullet";  //지금은 오브젝트 풀링을 이름으로 찾아서 가는 방식으로 하고있어서 선언
    public GameObject attackEffect;
    public int damage = 1;
    public float speed = 3000f;  //총알이 나가는 속도(속도가 낮으면 레이져에 비해 타격속도가 너무 떨어져서 높게잡음)
    public Scene currentScene;

    public float timer = 0f;
    // Use this for initialization
    private void Awake()
    {
        
    }

    void Start()
    {
        currentScene = SceneManager.GetActiveScene();

    }

    private void OnEnable()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }

    private void Update()
    {
        if ( GetTimer() >= 1.3f && this.gameObject.activeInHierarchy)
        {
            SetTimer();
            if(currentScene.name == "Game")
            {
                GameManager.Instance.PushToPool(poolItemName, gameObject);
            }else if(currentScene.name == "Tutorial")
            {
                Tutorial.Instance.PushToPool(poolItemName, gameObject);
            }
            //Destroy(this.gameObject);
        }

    }

    float GetTimer()
    {
        return (timer += Time.deltaTime);
    }

    void SetTimer()
    {
        timer = 0f;
    }

    //총알이 각자 태그를 가진 오브젝트에 닿았을 때
    private void OnTriggerEnter(Collider other)
    {
        Vector3 thisPos = this.transform.position;


        if (other.gameObject.tag == "Enemy")
        {
            if (other.GetComponent<Enemy>().isDead == false)
            {
                other.GetComponent<Enemy>().GetDamage(damage);
                AttackEffect(thisPos);
            }
        }
        else if (other.gameObject.tag == "Portal")
        {
            if (other.GetComponent<Portal>().parent.GetComponent<Enemy>().Type != Enemy.EnemyType.Boss)
            {
                other.GetComponent<Portal>().parent.GetComponent<Enemy>().GetDamage(damage);
                Destroy(other.gameObject);
                AttackEffect(thisPos);
            }
        }
        else if (other.gameObject.tag == "Item")
        {
            other.GetComponent<Item>().GetDamage();
            AttackEffect(thisPos);

        }
        else if (other.gameObject.tag == "Bomb")
        {
            other.GetComponent<Bomb>().GetDamage();
            AttackEffect(thisPos);
        }
        else if (other.gameObject.tag == "MapObject")
        {
            var forObject = new Vector3(this.transform.position.x + 2, this.transform.position.y, this.transform.position.z - 2);
            AttackEffect(thisPos);
        }
        else if (other.gameObject.tag == "TutorialTarget")
        {
            other.GetComponent<TutorialTarget>().GetDamage();
            AttackEffect(thisPos);
        }
    }

    


    //총알을 맞췄을때 파티클이펙트 효과
    public void AttackEffect(Vector3 pos)
    {
        GameObject spark = Instantiate(attackEffect, pos, Quaternion.Euler(0, 135, 0));   //Hemishphere모양으로 파티클이 만들어지는데 이 방향을 컨트롤러가 바라보는 방향으로 튀게끔
        Destroy(spark, spark.GetComponent<ParticleSystem>().main.duration + 0.2f);
    }
}