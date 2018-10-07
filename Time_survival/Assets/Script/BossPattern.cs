using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPattern : MonoBehaviour {


    public Bomb bomb;
    Animator anim;
    Enemy me;
    Image HealthBar;
    Image HealthBarChild;
    Text BossTxt;
    public List<Bomb> Obstacles;
    public GameObject CubeParticle;

    public int level;
    int trigger;
    float maxhp;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        me = GetComponent<Enemy>();
        maxhp = (float)me.hp;
        BossTxt = GameObject.FindGameObjectWithTag("BossText").GetComponent<Text>();
        BossTxt.text = "TARGET";
        HealthBar = GameObject.FindGameObjectWithTag("BossHealthBar").GetComponent<Image>();
        HealthBarChild = GameObject.FindGameObjectWithTag("BossInnerHealthBar").GetComponent<Image>();
        Debug.Log(HealthBarChild.name);
        HealthBar.color = new Color(HealthBar.color.r, HealthBar.color.g, HealthBar.color.b, 0.25f);
        StartCoroutine(Fire());
    }


	void Update () {
        //HealthBar.rectTransform.sizeDelta = new Vector2(16, 280 - (280 - 280 * me.hp / maxhp));
        HealthBarChild.fillAmount = (float) (me.hp / maxhp);
    }


    IEnumerator Fire() 
    {

        switch (level)
        {
            case 1:
                yield return new WaitForSeconds(2f);

                while (true)
                {
                    if (me.isDead == true) yield break;
                    Boss1Attack();
                    yield return new WaitForSeconds(2f);
                }
            case 2:
                trigger = 0;
                while (true)
                {
                    StartCoroutine(Boss2Attack(-34.6f, 0, 4.5f));
                    yield return new WaitForSeconds(4f);
                    StartCoroutine(Boss2Attack(-22.4f, 0, -7.5f));
                    yield return new WaitForSeconds(4f);
                    StartCoroutine(Boss2Attack(-18.9f, 0, 5.5f));
                    yield return new WaitForSeconds(4f);
                }

            case 3:
                while (true)
                {
                    yield return new WaitForSeconds(3f);
                    if (me.isDead == true) yield break;
                    Boss3Attack(Random.Range(0,3));
                    yield return new WaitForSeconds(3f);
                    if (me.isDead == true) yield break;
                    StartCoroutine(Boss3Attack2());
                }
            default:
                break;
        }
    }

    public void Dead()
    {
        Debug.Log("DEAD");
        int Count = Obstacles.Count;
        for (var i = 0; i < Count; i++)
            Obstacles[0].BombDestroy();
        BossTxt.text = "";
        HealthBar.color = new Color(HealthBar.color.r, HealthBar.color.g, HealthBar.color.b, 0);
        HealthBarChild.fillAmount = 0;
    }


    void Boss1Attack() // 보스 1 패턴
    {
        Vector3 NewPosition;
        Bomb ins;
        anim.SetTrigger("OmolAttack2");
        NewPosition = new Vector3(transform.position.x + Random.Range(-4, 4), transform.position.y + Random.Range(2, 4), transform.position.z + 1);
        ins = Instantiate<Bomb>(bomb, NewPosition, Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up));
        ins.boss = this;
        Obstacles.Add(ins);
    }



    IEnumerator Boss2Attack(float xpos, float ypos, float zpos) // 보스 2 패턴
    {
        if (me.isDead == true) yield break;
        if (trigger == 0)
        {
            trigger = 1;
            anim.SetTrigger("OmolJump");
        }
        else
        {
            anim.SetTrigger("OmolRun");
        }
        Vector3 NewPosition;
        Bomb ins;
        //MovePos = new Vector3(-34.6f, 0, 4.5f);
        Vector3 MovePos = new Vector3(xpos, ypos, zpos);
        transform.rotation = Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up);
        while (true)
        {
            if (me.isDead == true) yield break;
            transform.position = Vector3.MoveTowards(transform.position, MovePos, 30 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            if (Vector3.Distance(transform.position, MovePos) < 1) break;
        }
        transform.rotation = Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up);
        anim.SetTrigger("OmolDmgFront2");
        me.GetComponentInChildren<ParticleSystem>().Play();
        Vector3 PieceShot = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        Vector3 PieceScale = new Vector3(2, 2, 2); 
        for (int i = 0; i < 5; i++)
        {
            var piece = Instantiate<GameObject>(CubeParticle, PieceShot, Quaternion.Euler(0, 72f * i, 45));
            piece.transform.localScale = PieceScale;
            piece.GetComponent<Rigidbody>().AddForce(piece.transform.up * -10);
            piece.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        NewPosition = new Vector3(transform.position.x + Random.Range(-4, 4), transform.position.y + Random.Range(2, 4), transform.position.z + 1);
        ins = Instantiate<Bomb>(bomb, NewPosition, Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up));
        ins.boss = this;
        Obstacles.Add(ins);
        yield return new WaitForSeconds(1);
        me.GetComponentInChildren<ParticleSystem>().Stop();
    }


    void Boss3Attack(int pattern) // 보스 3 패턴 1
    {
        anim.SetTrigger("OmolAttack2");
        Vector3 NewPosition;
        Bomb ins;
        switch (pattern) {
            case 0: // 십자 모양
                for (var i = 0; i < 3; i++) {
                    int j = 0;
                    if (i != 1) j = 1;
                    for (; j < 3; j++){
                        if(transform.position.x == 0)
                            NewPosition = new Vector3(transform.position.x + 2 - (i * 2), transform.position.y + 2 + (j * 2), transform.position.z + 1);
                        else
                            NewPosition = new Vector3(transform.position.x + 1, transform.position.y + 2 + (j * 2), transform.position.z + 2 - (i * 2));
                        ins = Instantiate<Bomb>(bomb, NewPosition, Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up));
                        ins.boss = this;
                        Obstacles.Add(ins);
                        if (i != 1) break;
                    }
                }
                break;
            case 1: // 사다리꼴 
               for(var i = 0; i < 3; i++)
                {
                    for(var j = 0; j < 2; j++)
                    {
                        int minus = (j == 0) ? 1 : -1;
                        if (transform.position.x == 0)
                            NewPosition = new Vector3(transform.position.x + (minus *  (i % 3 + 1)), transform.position.y + 6 - (i * 2), transform.position.z + 1);
                        else
                            NewPosition = new Vector3(transform.position.x + 1, transform.position.y + 6 - (i * 2), transform.position.z + (minus * (i % 3 + 1)));
                        ins = Instantiate<Bomb>(bomb, NewPosition, Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up));
                        ins.boss = this;
                        Obstacles.Add(ins);
                    }
                } 
                break;
            case 2: // 육각형
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 2; j++)
                    {
                        int minus = (j == 0) ? 1 : -1;
                        if (transform.position.x == 0)
                            NewPosition = new Vector3(transform.position.x + (minus * 2 * ((i % 2) + 1)), transform.position.y + 6 - (i * 2), transform.position.z + 1);
                        else
                            NewPosition = new Vector3(transform.position.x + 1 , transform.position.y + 6 - (i * 2), transform.position.z + (minus * 2 * ((i % 2) + 1)));
                        ins = Instantiate<Bomb>(bomb, NewPosition, Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up));
                        ins.boss = this;
                        Obstacles.Add(ins);
                    }
                }
                break;
            default:
                break;
        }
    }

    IEnumerator Boss3Attack2()
    {
        int aftertrigger = 0;
        List<Vector3> newPos = new List<Vector3>();
        var portal_exit = Instantiate<Portal>(me.portal, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
        portal_exit.parent = this.GetComponent<Enemy>();
        newPos.Add(new Vector3(0, 0, 25));
        newPos.Add(new Vector3(0, 0, -25));
        newPos.Add(new Vector3(25, 0, 0));
        newPos.Add(new Vector3(-25, 0, 0));
        for (var i = Random.Range(0, 4); ; i = Random.Range(0, 4))
        {
            if (i != trigger)
            {
                aftertrigger = i;
                break;
            }
        }
        trigger = aftertrigger;
        anim.SetTrigger("OmolAttack1");
        yield return new WaitForSeconds(1f);

        transform.position = newPos[trigger];
        transform.rotation = Quaternion.LookRotation(GameManager.Instance.Player.gameObject.transform.position - transform.position, Vector3.up);
        var portal_in = Instantiate<Portal>(me.portal, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.rotation);
        portal_in.parent = this.GetComponent<Enemy>();
        newPos.Clear();
        yield return null;
    }
}
