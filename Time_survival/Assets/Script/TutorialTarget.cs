using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialTarget : MonoBehaviour {


    /*
     * Enemy와 겹치는 면이 매우 많으므로 추후 통합예정.
    */


    public int type = 0; // 0 : 노말 1 : 스피드다운 2 : 스피드업
    public float speed = 0;
    public Image EnemyDot;
    ParticleSystem ps; //파티클
    Image edot;
    bool isDead = false;
    string[] DeadMotion = new string[] { "OmolDead1", "OmolDead2", "OmolDead3", "OmolDead4" };


    // Use this for initialization
    void Start () {
        ps = GetComponentInChildren<ParticleSystem>();
        Tutorial.Instance.count += 1;
        Tutorial.Instance.EnemyLeft.text = Tutorial.Instance.count.ToString();
        edot = Instantiate<Image>(EnemyDot);
        edot.rectTransform.SetParent(GameObject.FindWithTag("Minimap").transform);
        edot.rectTransform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        if (type == 0) GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.yellow;
        edot.color = GetComponentInChildren<SkinnedMeshRenderer>().material.color;
    }

    void Update()
    {
        if (edot != null)
        {
            edot.rectTransform.localPosition = new Vector2(transform.position.x, transform.position.z);
            edot.rectTransform.localRotation = Quaternion.identity;
        }
    }
	
    public void GetDamage()
    {
        if (isDead == false)
        {
            isDead = true;
            Tutorial.Instance.count -= 1;
            GetComponent<Animator>().SetTrigger(DeadMotion[Random.Range(0,4)]);
            Destroy(edot.gameObject);
            if (type == 0)
            {
                Time.timeScale = 1;
                Tutorial.Instance.HealthBar.color = Color.yellow;
            }
            else if (type == 1)
            {
                Time.timeScale = 0.5f;
                Tutorial.Instance.HealthBar.color = Color.blue;
            }
            else if (type == 2)
            {
                Time.timeScale = 2f;
                Tutorial.Instance.HealthBar.color = Color.red;
            }
            Tutorial.Instance.timeScaleText.text = Time.timeScale.ToString();
            Debug.Log("HIT : " + this.GetInstanceID());
            Tutorial.Instance.EnemyLeft.text = Tutorial.Instance.count.ToString();
            if (Tutorial.Instance.count == 0)
            {
                Tutorial.Instance.page += 1;
                Tutorial.Instance.nextTuto();
            }
            Tutorial.Instance.ChangeStatus(type);
            Tutorial.Instance.enemylist.Remove(this);
            speed = 0;
            StartCoroutine(SetSink());
        }
    }

    public IEnumerator Move()
    {
        while (true)
        {
            while (true)
            {
                Vector3 newPos = new Vector3(speed, 0, 0) * Time.deltaTime;
                transform.position = transform.position + newPos;
                if (transform.position.x >= 11 || transform.position.x <= -11) break;
                yield return new WaitForEndOfFrame();
            }
            speed = -speed;
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator CreateParticle(int et)
    {
        if (ps != null && isDead == false)
        {
            var pschild = ps.transform.Find("Twinkle").GetComponent<ParticleSystem>();
            var main = ps.main;
            var mainchild = pschild.main;
            main.startColor = new ParticleSystem.MinMaxGradient(SetColor(et), Color.white);
            mainchild.startColor = new ParticleSystem.MinMaxGradient(SetColor(et), Color.white);
            ps.Play();
            yield return new WaitForSeconds(2f);
            ps.Stop();
            yield break;
        }
    }

    public Color SetColor(int et)
    {
        switch (et)
        {
            case 0:
                return Color.yellow;
            case 1:
                return Color.blue;
            case 2:
                return Color.red;
            default:
                return Color.black;
        }
    }

    IEnumerator SetSink()
    {
        yield return new WaitForSeconds(1.5f);
        GetComponent<Collider>().enabled = false;
        Vector3 sink = new Vector3(transform.position.x, -5, transform.position.z);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, sink, transform.localScale.x * Time.deltaTime);
            if (transform.position.y < -5)
            {
                Destroy(this.gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
