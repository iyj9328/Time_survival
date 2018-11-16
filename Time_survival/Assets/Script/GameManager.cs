using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static int Score; //점수
    public static int PlayerHealth = 500; //플레이어 체력
    public static int Combo = 0; // 콤보
    public static int Wave = 1;
    public static GameManager Instance; // 매니저의 인스턴스 아이디를 기록할 변수.
    public List<PooledObject> ObjectPool = new List<PooledObject>();    //PooledObject를 리스트(배열)로 관리하기 위한 변수 선언
    public int enemyleft = 10; // 남은 적 생성 갯수
    public float Interval; // 적 생성 간격
    public Enemy enemy; //생성할 적
    public Enemy boss; //생성할 적
    public Item item;   //생성할 아이템
    public GameObject GetHpParticle;    //플레이어가 Item을 맞췄을때 HP가 오르는 것을 보여주기 위한 파티클
    public GameObject Player; //플레이어
    public List<Enemy> enemylist; //생성한 적들을 일괄적으로 관리하기 위한 리스트
    public Text ScoreText; //점수 텍스트
    public Text timeScaleText; //타임스케일 텍스트
    public Text ComboText; //콤보 텍스트
    public Text WaveText; //웨이브 텍스트
    public Text NoticeText; // 웨이브 안내 텍스트
    public Text EnemyLeftText; //앞으로 생성될 적 갯수
    public Text ResultText; // 클리어시 결과를 보여줄 텍스트
    public Image HealthBar; // 체력바
    public Image StatusImage; //상태 표시 이미지
    public Image ComboBar;    //콤보 게이지
    public Image FadeImage;
    public Image Map;         //미니맵
    public AudioClip Warning;   //빅웨이브 시에 경고음
    public AudioClip Gameover;  //게임오버 당했을 때(졌을 때) BGM
    public AudioClip NormalBgm; //평상 시에 나오는 BGM
    public AudioClip BossBgm;   //Boss 몹 등장 시에 나오는 BGM
    public AudioClip MissionComplete;       //게임을 클리어했을 때 나오는 BGM

    private GameObject GetHpIns;        //HP회복 파티클 GameObject
    private AudioSource modeName;       

    [HideInInspector]
    public bool CanGenerate = true; //적 생성 가능 여부
    [HideInInspector]
    public bool OnGetHpCheck; //회복 파티클이 활성화 되어있는지의 여부
    [HideInInspector]
    public enum WS
    {
        Normal,
        EnemyRush,
        Boss,
        GameOver,
        GameClear
    }
    [HideInInspector]
    public WS WaveStatus;
    [HideInInspector]
    public int ComboGauge = 0;

    int MaxCombo = 0;

    //Object Pooling 방식으로 총알을 사전에 객체화 시켜서 생성해줌.
    private void Awake()
    {
        for(int i = 0; i < ObjectPool.Count; ++i)
        {
            ObjectPool[i].Initialize(transform);
        }
    }
    // Start
    void Start()
    {
        AddBgmAudio(NormalBgm);
        GetHpInstance();
        WaveStatus = WS.Normal;
        Instance = this;
        StatusImage.color = new Color(StatusImage.color.r, StatusImage.color.g, StatusImage.color.b, 0);
        StartCoroutine(FadeOut());
        StartCoroutine(NormalMode());
        StartCoroutine(PlayerHealthControl());
        StartCoroutine(ComboGaugeControl());
        StartCoroutine(ItemGenerate());
    }


    // Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GlobalAttack();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayerHealth = 10;
        }
        if (WaveStatus != WS.GameOver && PlayerHealth < 1) // 체력 소모 체크.
        {
            modeName.Pause();
            StopAllCoroutines();
            NoticeText.text = "GAME OVER";
            AudioSource.PlayClipAtPoint(Gameover, transform.position);
            StartCoroutine(GameEnd());
        }
        if (OnGetHpCheck != false)      //회복 Item을 명중 시켰을 시에 Effect
        {
            StartCoroutine(OnGetHp());
        }
        if (WaveStatus != WS.GameOver && WaveStatus != WS.GameClear && WaveStatus != WS.Normal && WaveStatus != WS.EnemyRush)
        {
            modeName.Pause();
            modeName.clip = BossBgm;
            modeName.PlayDelayed(3f);
        }
        if(WaveStatus != WS.GameOver && WaveStatus != WS.GameClear && WaveStatus != WS.Boss)
        {
            modeName.Pause();
            modeName.clip = NormalBgm;
            modeName.Play();
        }
        
    }


    //OnGUI 
    private void OnGUI()
    {
        timeScaleText.text = Time.timeScale.ToString();
        ScoreText.text = "SCORE : " + Score.ToString();
        if (Combo == 0)
            ComboText.text = "";
        else
            ComboText.text = Combo.ToString() + " COMBO";
        //HealthBar.rectTransform.sizeDelta = new Vector2(500 - (500 - 500 * PlayerHealth / 500), 16);
        HealthBar.fillAmount = (float)PlayerHealth / 500.0f;
        ComboBar.rectTransform.sizeDelta = new Vector2(150 - (150 - 150 * ComboGauge / 100), 5);
        Map.transform.localRotation = Quaternion.Euler(0, 0, Camera.main.transform.eulerAngles.y);

    }

    //BGM을 바꿔주기위한 함수
    void AddBgmAudio(AudioClip BgmName)
    {
        transform.gameObject.AddComponent<AudioSource>();
        modeName = GetComponent<AudioSource>();
        modeName.clip = BgmName;
        modeName.loop = true;
        modeName.Play();
    }

    //사용하고 난 GameObject(총알)를 다시 Pool에 반환시켜준다
    public bool PushToPool(string itemName, GameObject bullet, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null) return false;
        
        pool.PushToPool(bullet, parent == null ? transform : parent);
        return true;
    }

    //생성되어있는 객체인 GameObject(총알)를 이름으로 찾아 GetPoolItem 함수를 이용하여 Pool에서 꺼내온다.
    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null) return null;
        
        return pool.PopFromPool(transform);
    }
    PooledObject GetPoolItem(string itemName)
    {
        for(int i = 0; i < ObjectPool.Count; i++)
        {
            if (ObjectPool[i].poolItemName.Equals(itemName))
            {
                return ObjectPool[i];
            }
        }

        Debug.LogWarning("There is no matched pool list");

        return null;
    }

    //아이템 생성
    private IEnumerator ItemGenerate()
    {
        while (true)
        {
            float Range1 = RandomMinus() * Random.Range(-40, 40);
            float Range2 = RandomMinus() * Random.Range(-7, 7);
            Vector3 CreatePositionX = new Vector3(Range2, 0.65f, Range1);
            Vector3 CreatePositionZ = new Vector3(Range1, 0.65f, Range2);
            Vector3 PositionSelect = (RandomMinus() == 1) ? CreatePositionX : CreatePositionZ;
            var insItem = Instantiate<Item>(item, PositionSelect, Quaternion.LookRotation(Player.gameObject.transform.position - PositionSelect, Vector3.up));
            yield return new WaitForSeconds(Random.Range(13f, 20f));
        }

    }

    void GetHpInstance()
    {
        GameObject GetHp = Instantiate(GetHpParticle, Player.transform);       //회복 파티클의 인스턴스를 플레이어의 자식으로 생성
        GetHp.SetActive(false);                              //Item을 맞출때만 활성화 할 수 있게 우선은 false상태로 둠
        GetHpIns = GetHp;
    }

    IEnumerator OnGetHp()
    {
        GetHpIns.SetActive(true);
        yield return new WaitForSeconds(GetHpIns.GetComponent<ParticleSystem>().main.duration + 0.2f);
        OnGetHpCheck = false;
        GetHpIns.SetActive(false);
    }

    //웨이브 타입 1 : 적 생성 코루틴
    IEnumerator NormalMode()
    {
        yield return new WaitForSeconds(2f);
        NoticeText.text = "";
        while (true)
        {
            if (enemyleft > 0 && CanGenerate == true)
            {
                enemyleft -= 1;
                EnemyLeftText.text = enemyleft.ToString();
                EnemyGenerate();
                yield return new WaitForSeconds(Interval);
            }
            else
            {
                yield break;
            }
        }
    }

    //적 생성 (랜덤 포지션)
    public void EnemyGenerate()
    {
        float Range1 = RandomMinus() * Random.Range(35, 40);
        float Range2 = RandomMinus() * Random.Range(-7, 7);
        Vector3 CreatePositionX = new Vector3(Range2, 0.65f, Range1);
        Vector3 CreatePositionZ = new Vector3(Range1, 0.65f, Range2);
        Vector3 PositionSelect = (RandomMinus() == 1) ? CreatePositionX : CreatePositionZ;
        var ins = Instantiate<Enemy>(enemy, PositionSelect, Quaternion.LookRotation(Player.gameObject.transform.position - PositionSelect, Vector3.up));
        enemylist.Add(ins);
        ins.SetEnemyType((Enemy.EnemyType)Random.Range(0, 3));
    }
    //적 생성 (지정 포지션과 지정 타입)
    public void EnemyGenerate(float xpos, float ypos, float zpos, Enemy.EnemyType et)
    {
        Vector3 Position = new Vector3(xpos, ypos, zpos);
        var ins = Instantiate<Enemy>(enemy, Position, Quaternion.LookRotation(Player.gameObject.transform.position - Position, Vector3.up));
        enemylist.Add(ins);
        ins.SetEnemyType(et);
    }

    /*void BulletInstantiate()  //오브젝트풀 방식 일단 보류
    {
        for (int i=0; i < maxBullets; i++)
        {
            
        }
    }*/

    //웨이브 타입 2 : 러쉬 모드 진입
    public IEnumerator RushMode()
    {
        HealthBar.color = new Color(255, 255, 0, 0.5f);
        Time.timeScale = 1;
        NoticeText.text = "ENEMY RUSH!";
        AudioSource.PlayClipAtPoint(Warning, transform.position);
        int count = 9;
        float alpha = 1f;
        while (true)
        {
            if (count == 0)
            {
                RushGenerate();
                NoticeText.text = "";
                break;
            }
            if (count % 2 == 0)
            {
                alpha -= 0.1f;
                if (alpha < 0)
                    count -= 1;
            }
            else
            {
                alpha += 0.1f;
                if (alpha > 1)
                    count -= 1;
            }
            NoticeText.color = new Color(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.02f);
        }
        yield break;
    }



    // 웨이브 별 러쉬 구성 
    void RushGenerate()
    {
        EnemyLeftText.text = "RUSH";

        switch (Wave)
        {
            case 1:
                for (var i = -1; i < 2; i++)
                {
                    EnemyGenerate(i * 2, 0, 43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(i * 2, 0, -43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(-43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                }
                break;
            case 2:
                for (var i = -2; i < 2; i++)
                {
                    EnemyGenerate(i * 2, 0, 43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(i * 2, 0, -43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(-43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                }
                break;
            case 3:
                for (var i = -3; i < 2; i++)
                {
                    EnemyGenerate(i * 2, 0, 43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(i * 2, 0, -43, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                    EnemyGenerate(-43, 0, i * 2, (Enemy.EnemyType)Random.Range(0, 3));
                }
                break;
            default:
                break;
        }

    }

    //웨이브 타입 3 : 보스 모드 진입
    public IEnumerator BossMode()
    {
        Debug.Log("BOSS");
        HealthBar.color = new Color(255, 255, 0, 0.5f);
        Time.timeScale = 1;
        NoticeText.text = "WARNING!";
        AudioSource.PlayClipAtPoint(Warning, transform.position);
        int count = 9;
        float alpha = 1f;
        while (true)
        {
            if (count == 0)
            {
                BossGenerate();
                NoticeText.text = "";
                yield break;
            }
            if (count % 2 == 0)
            {
                alpha -= 0.1f;
                if (alpha < 0)
                    count -= 1;
            }
            else
            {
                alpha += 0.1f;
                if (alpha > 1)
                    count -= 1;
            }
            NoticeText.color = new Color(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.02f);
        }
    }

    // 웨이브 별 보스
    void BossGenerate()
    {
        Enemy ins = null;
        EnemyLeftText.text = "BOSS";
        switch (Wave)
        {
            case 1:
                Time.timeScale = 1.0f;
                ins = Instantiate<Enemy>(boss, new Vector3(0, 0, 25), Quaternion.LookRotation(Player.gameObject.transform.position - new Vector3(0, 0, 25), Vector3.up));
                ins.SetEnemyType(Enemy.EnemyType.Boss);
                ins.hp = 30;
                ins.GetComponent<BossPattern>().level = 1;
                enemylist.Add(ins);
                break;
            case 2:
                Time.timeScale = 1.0f;
                ins = Instantiate<Enemy>(boss, new Vector3(-36.3f, 14.2f, -22.7f), Quaternion.LookRotation(Player.gameObject.transform.position - new Vector3(0, 0, 25), Vector3.up));
                ins.SetEnemyType(Enemy.EnemyType.Boss);
                ins.hp = 40;
                ins.GetComponent<BossPattern>().level = 2;
                enemylist.Add(ins);
                break;
            case 3:
                Time.timeScale = 1.0f;
                ins = Instantiate<Enemy>(boss, new Vector3(0, 0, 25), Quaternion.LookRotation(Player.gameObject.transform.position - new Vector3(0, 0, 25), Vector3.up));
                ins.SetEnemyType(Enemy.EnemyType.Boss);
                ins.hp = 50;
                ins.GetComponent<BossPattern>().level = 3;
                enemylist.Add(ins);
                break;
            default:
                break;
        }
        ;

    }

    // 새 웨이브 설정 및 시작
    public void NewWave()
    {
        Wave += 1;
        WaveText.text = Wave.ToString();
        NoticeText.text = "WAVE " + Wave.ToString();
        switch (Wave)
        {
            case 2:
                enemyleft = 20;
                Interval = 2.5f;
                StartCoroutine(NormalMode());
                break;
            case 3:
                enemyleft = 25;
                Interval = 2f;
                StartCoroutine(NormalMode());
                break;
            case 4:
                WaveStatus = WS.GameClear;
                modeName.Pause();
                modeName.clip = MissionComplete;
                modeName.Play();
                NoticeText.text = "MISSION\nCOMPLETE";
                NoticeText.rectTransform.localPosition = new Vector3(0, 140, 0);
                ResultText.text = string.Format("Score : {0}\nMax Combo : {1}",Score,MaxCombo);
                StartCoroutine(GameEnd());

                break;
        }
    }


    // 랜덤하게 1과 -1 출력
    int RandomMinus()
    {
        int p = Random.Range(0, 2);
        return (p == 0) ? 1 : -1;
    }


    //리스트의 모든 적들의 상태 변경
    public void ChangeStatus(Enemy.EnemyType et) // For Test
    {
        int EnemyCount = enemylist.Count;
        for (var i = 0; i < EnemyCount; i++)
        {
            enemylist[i].StopCoroutine(enemylist[i].CreateParticle(et));
            enemylist[i].StartCoroutine(enemylist[i].CreateParticle(et));
        }
    }


    //리스트의 모든 적들에게 -1 공격 (테스트용)
    void GlobalAttack()
    {
        int EnemyCount = enemylist.Count;
        for (var i = 0; i < EnemyCount; i++)
        {
            Debug.Log("i : " + i + " / " + enemylist[0].GetInstanceID());
            enemylist[0].GetDamage(1);
        }
    }

    //플레이어의 체력을 지속적으로 소모시키는 코루틴
    IEnumerator PlayerHealthControl()
    {
        while (true)
        {
            PlayerHealth -= 1;
            yield return new WaitForSeconds(0.1f);
        }
    }


    //공격 받은 후 화면이 빨간색이 됨.
    public IEnumerator DamagedScreen()
    {
        float alpha = 1;
        while (true)
        {
            alpha -= 0.1f;
            StatusImage.color = new Color(StatusImage.color.r, StatusImage.color.g, StatusImage.color.b, alpha);
            if (alpha < 0) yield break;
            yield return new WaitForSeconds(0.05f);
        }

    }

    //콤보 게이지 컨트롤
    public IEnumerator ComboGaugeControl()
    {
        while (true)
        {
            if (ComboGauge == 0) {
                if (Combo > MaxCombo)
                    MaxCombo = Combo;
                Combo = 0;
            }
            else ComboGauge -= 1;
            yield return new WaitForSeconds(0.03f);
        }
    }

    //게임오버와 게임 클리어
    public IEnumerator GameEnd()
    {
        Player.GetComponent<FireCtrl>().canShot = false;
        int EnemyCount = enemylist.Count;
        for (var i = 0; i < EnemyCount; i++)
        {
            if (enemylist[i].Type == Enemy.EnemyType.Boss)
            {
                enemylist[i].GetComponent<BossPattern>().StopAllCoroutines();
            }
            else
            {
                enemylist[i].StopAllCoroutines();
                enemylist[i].navAgent.speed = 0;
                enemylist[i].navAgent.enabled = false;
            }
        }

        NoticeText.color = Color.white;
        if(WaveStatus!=WS.GameClear)
            WaveStatus = WS.GameOver;
        float alpha = 0;
        while (true)
        {
            alpha += 0.05f;

            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            if (alpha > 1) break;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Title");
    }

    IEnumerator FadeOut()
    {
            float alpha = 1;
            while (true)
            {
                alpha -= 0.01f;
                FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
                if (alpha < 0)
                    yield break;
                yield return new WaitForEndOfFrame();
            }
    }

}
