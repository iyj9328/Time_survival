using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour {

    public List<TutorialTarget> enemylist; 
    public Text NoticeText;
    public Text TutoText;
    public Text EnemyLeft;
    public List<PooledObject> ObjectPool = new List<PooledObject>();    //PooledObject를 리스트(배열)로 관리하기 위한 변수 선언
    public Text timeScaleText;
    public Image HealthBar;
    public Image FadeImage;
    public int page = 0;
    public int count = 0;
    public static Tutorial Instance;
    public TutorialTarget tt;
    public Image Map;
    public GameObject Player; //플레이어
    // Use this for initialization
    void Awake () {
        Instance = this;
        for (int i = 0; i < ObjectPool.Count; ++i)
        {
            ObjectPool[i].Initialize(transform);
        }
    }

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

	
	// Update is called once per frame
	void Update () {
        Map.transform.localRotation = Quaternion.Euler(0, 0, Camera.main.transform.eulerAngles.y);
    }

    public bool PushToPool(string itemName, GameObject bullet, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null)
            return false;

        pool.PushToPool(bullet, parent == null ? transform : parent);
        return true;
    }

    public GameObject PopFromPool(string itemName, Transform parent = null)
    {
        PooledObject pool = GetPoolItem(itemName);
        if (pool == null) return null;

        return pool.PopFromPool(transform);
    }

    PooledObject GetPoolItem(string itemName)
    {
        string bulletName = "Bullet";
        for (int i = 0; i < ObjectPool.Count; i++)
        {
            if(ObjectPool[i].poolItemName.Equals(bulletName) || ObjectPool[i].poolItemName.Equals(itemName))
            {
                return ObjectPool[i];
            }
        }

        Debug.LogWarning("There is no matched pool list");

        return null;
    }

    public void nextTuto()
    {
        TutorialTarget ins = null;
        switch (page)
        {
            case 0:
                break;
            case 1:
                TutoText.text = "파란 타겟\n시간이 2분의 1로 느려집니다.";
                for (var i = 0; i < 3; i++)
                {
                    ins = Instantiate<TutorialTarget>(tt, new Vector3(-10 + (i * 10), 0.3f, 15 + (i * 4)), Quaternion.Euler(0,180,0));
                    ins.type = 1;
                    ins.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.blue;
                    ins.speed = 3;
                    enemylist.Add(ins);
                    ins.GetComponent<Animator>().SetTrigger("OmolGoForwad");
                    ins.StartCoroutine(ins.Move());
                }
                break;
            case 2:
                TutoText.text = "빨간 타겟\n시간이 2배로 빨라집니다.";
                for (var i = 0; i < 3; i++)
                {
                    ins = Instantiate<TutorialTarget>(tt, new Vector3(-10 + (i * 10), 0.3f, 15 + (i * 4)), Quaternion.Euler(0, 180, 0));
                    ins.type = 2;
                    ins.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.red;
                    ins.speed = 3;
                    enemylist.Add(ins);
                    ins.GetComponent<Animator>().SetTrigger("OmolGoForwad");
                    ins.StartCoroutine(ins.Move());
                }
                break;
            case 3:
                TutoText.text = "노란 타겟\n시간이 정상으로 회복됩니다.";
                for (var i = 0; i < 3; i++)
                {
                    ins = Instantiate<TutorialTarget>(tt, new Vector3(-10 + (i * 10), 0.3f, 15 + (i * 4)), Quaternion.Euler(0, 180, 0));
                    ins.type = 0;
                    ins.GetComponentInChildren<SkinnedMeshRenderer>().material.color = Color.yellow;
                    ins.speed = 3;
                    enemylist.Add(ins);
                    ins.GetComponent<Animator>().SetTrigger("OmolGoForwad");
                    ins.StartCoroutine(ins.Move());
                }
                break;
            case 4:
                StartCoroutine(FadeAndNext());
                TutoText.rectTransform.localPosition = new Vector3(0, 0, 40);
                TutoText.text = "모든 과정이 끝났습니다. \n전투를 시작합니다.";
                break;

        }
    }

    public void ChangeStatus(int et) // For Test
    {
        Debug.Log("ON");
        int EnemyCount = enemylist.Count;
        for (var i = 0; i < EnemyCount; i++)
        {
            enemylist[i].StopCoroutine(enemylist[i].CreateParticle(et));
            enemylist[i].StartCoroutine(enemylist[i].CreateParticle(et));
        }
    }

    IEnumerator FadeAndNext()
    {
        float alpha = 0;
        while (true)
        {
            alpha += 0.01f;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            if (alpha > 1)
            {
                yield return new WaitForSeconds(1.5f);
                SceneManager.LoadScene("Game");
            }
            yield return new WaitForEndOfFrame();
            
        }
    }

    IEnumerator FadeOut()           //천천히 설명화면이 사라지면서 튜토리얼 모드가 실행 됨.
    {
        float alpha = 1;
        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
        Player.GetComponent<FireCtrl>().canShot = false;
        TutoText.rectTransform.localPosition = new Vector3(0, 0, 40);
        TutoText.text = "본격적인 전투에 앞서\n트레이닝을 실시합니다.";
        yield return new WaitForSeconds(3f);
        TutoText.text = "";
        while (true)
        {
            alpha -= 0.01f;
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            if (alpha < 0)
            {
                Debug.Log("TEST");
                Player.GetComponent<FireCtrl>().canShot = true;
                TutoText.rectTransform.localPosition = new Vector3(0, 260, 40);
                TutoText.text = "고개를 움직여\n4방향을 공격해보세요.";
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
