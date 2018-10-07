using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Image EnemyDot; // 미니맵에 표시할 점 프리팹
    public Portal portal; // 포탈 프리팹
    public int hp = 1; // 체력
    public float speed = 2; // 적의 스피드
    public AudioClip deathClip;
    public enum EnemyType // 적 타입 enum
    {
        Normal,
        Fast,
        Slow,
        Boss
    }
    
    public EnemyType Type; //타입
    Transform target;

    Rigidbody rb; //리지드바디 
    SkinnedMeshRenderer mr; //메시 렌더러
    Portal po; //포탈
    Image edot; //미니맵 점
    ParticleSystem ps; //파티클
    Animator anim;
    [HideInInspector]
    public bool isDead = false;
    [HideInInspector]
    public NavMeshAgent navAgent;

    string[] DeadMotion = new string[] { "OmolDead1", "OmolDead2", "OmolDead3", "OmolDead4" };


    // Awake
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        ps = GetComponentInChildren<ParticleSystem>();
        edot = Instantiate<Image>(EnemyDot);
        edot.rectTransform.SetParent(GameObject.FindWithTag("Minimap").transform);
        edot.rectTransform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        po = Instantiate<Portal>(portal, new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z), transform.rotation);
        po.parent = this;
        mr = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Start()
    {
        if (Type != EnemyType.Boss)
        {
            navAgent = GetComponent<NavMeshAgent>();
            target = GameObject.FindGameObjectWithTag("Player").transform;
            navAgent.SetDestination(target.position);
        }
        StartCoroutine(SetAlpha());
    }
    // 위치 업데이트와 미니맵 업데이트
    void Update() 
    {
        if (edot != null) {
            edot.rectTransform.localPosition = new Vector2(transform.position.x, transform.position.z);
            edot.rectTransform.localRotation = Quaternion.identity;
        }
    }

    //공격 받았을 때.
    public void GetDamage(int damage) 
    {
        Color HealthBarColor = SetColor(Type);


        hp -= damage;
        if (hp < 1 && isDead == false)
        {
            isDead = true;
            var isBoss = GetComponent<BossPattern>();
            if (isBoss != null)
            {
                isBoss.Dead();
            }

            switch (Type)
            {
                case EnemyType.Normal:
                    Time.timeScale = 1.0f;
                    break;
                case EnemyType.Fast:
                    Time.timeScale = 2.0f;
                    break;
                case EnemyType.Slow:
                    Time.timeScale = 0.5f;
                    break;
            }
            GameManager.Instance.ComboGauge = 100;
            GameManager.Score += 10 + GameManager.Combo;
            GameManager.Combo += 1;            
            GameManager.PlayerHealth += 30;
            if (GameManager.PlayerHealth > 500)
                GameManager.PlayerHealth = 500;
            GameManager.Instance.ChangeStatus(Type);

            GameManager.Instance.HealthBar.color = new Color(HealthBarColor.r, HealthBarColor.g, HealthBarColor.b,0.5f);
            StartCoroutine(EnemyDestroy(1.5f));
        } 
    }

    //적 제거
    IEnumerator EnemyDestroy(float interval)
    {
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
        if (Type != EnemyType.Boss)
        {
            navAgent.speed = 0;
        }
        GameManager.Instance.enemylist.Remove(this);
        if (GameManager.Instance.enemylist.Count == 0){
            if (GameManager.Instance.enemyleft == 0 && GameManager.Instance.WaveStatus == GameManager.WS.Normal) // 러쉬모드 시작
            {
                GameManager.Instance.WaveStatus = GameManager.WS.EnemyRush;
                GameManager.Instance.StartCoroutine(GameManager.Instance.RushMode());
            }
            else if (GameManager.Instance.WaveStatus == GameManager.WS.EnemyRush) // 보스모드 시작
            {
                GameManager.Instance.WaveStatus = GameManager.WS.Boss;
                GameManager.Instance.StartCoroutine(GameManager.Instance.BossMode());
            }
            else if (GameManager.Instance.WaveStatus == GameManager.WS.Boss) // 노말모드 복귀
            {
                GameManager.Instance.WaveStatus = GameManager.WS.Normal;
                GameManager.Instance.NewWave();
            }
        }

        if (po != null) Destroy(po.gameObject);
        if (edot != null) Destroy(edot.gameObject);

        anim.SetTrigger(DeadMotion[Random.Range(0,4)]);
        yield return new WaitForSeconds(interval);
        StartCoroutine(SetSink());
    }


    //파티클 생성 - 색상별 적 제거시 적 타입의 영향을 받아 속도가 변경되었음을 시각적으로 표시.
    public IEnumerator CreateParticle(EnemyType et)
    {
        if (ps != null && Type != EnemyType.Boss && isDead == false)
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




    //타입 지정 - GameManager에서 인스턴스를 리스트에 추가하며 함께 호출
    public void SetEnemyType(EnemyType et) 
    {
        Type = et;
        if(Type!=EnemyType.Boss)
            mr.material.color = SetColor(Type);
        edot.color = SetColor(Type);
    }


    //타입 별 색상 지정
    public Color SetColor(EnemyType et)
    {
        switch (et)
        {
            case EnemyType.Normal:
                return Color.yellow;
            case EnemyType.Fast:
                return Color.red;
            case EnemyType.Slow:
                return Color.blue;
            case EnemyType.Boss:
                return Color.yellow;
            default:
                return Color.black;
        }
    }

    //알파값 설정 - 초기 생성시 포탈에서 서서히 등장하는 듯한 효과 연출을 위한 코루틴.
    IEnumerator SetAlpha()
    {
        rb.useGravity = false;
        if (navAgent != null)
            navAgent.speed = 0;
        float alpha = 0f;
        if (GameManager.Instance.WaveStatus == GameManager.WS.EnemyRush)
        {
            po.shrinkTiming = 4f;
            yield return new WaitForSeconds(4f);
        }
        while (true)
        {
            alpha += 0.05f;
            mr.material.color = new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, alpha);
            if (alpha > 1)
            {
                if(navAgent!=null)
                    navAgent.speed = speed;
                rb.useGravity = true;
                if (Type != EnemyType.Boss)
                {
                    anim.SetTrigger("OmolGoForwad");
                }
                yield break;
            }
            else yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator SetSink()
    {
        GetComponent<Collider>().enabled = false;
        if (Type != EnemyType.Boss)
        {
            navAgent.enabled = false;
        }
        Vector3 sink = new Vector3(transform.position.x, -5, transform.position.z);
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, sink, transform.localScale.x * Time.deltaTime);
            if(transform.position.y < -5)
            {
                Destroy(gameObject);
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    //플레이어와 부딪혔을 때
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && isDead == false)
        {
            Debug.Log("OnTriggerEnter if");

            isDead = true;
            hp = 0;
            GameManager.PlayerHealth -= 50;
            GameManager.Instance.StopCoroutine(GameManager.Instance.DamagedScreen());
            GameManager.Instance.StartCoroutine(GameManager.Instance.DamagedScreen());
            StartCoroutine(EnemyDestroy(0));
        }
    } 


}
