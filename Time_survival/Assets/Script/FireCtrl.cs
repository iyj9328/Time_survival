using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireCtrl : MonoBehaviour
{
    public float fireRate = 0.35f;
    public GameObject bullet;  //총알 오브젝트
    public Transform firePos;  //GunEnd의 위치정보
    public Camera fpsCam;      //Camera의 정보를 담은 변수선언
    public AudioClip shootClip;
    public int maxBullets = 15; //최대 총알 생성 갯수
    public bool canShot = true;
    public string bulletName = "Bullet";

    private float nextFire;

    private void Start()
    {
        if (Option.inputoption == Option.InputOption.NoPad)
        {
            StartCoroutine(AutoFire());
        }
    }

    private void Update()
    {
        //마우스 왼쪽 클릭시 총알을 생성하는 Fire()를 호출
        if (Option.inputoption == Option.InputOption.Pad)
        {
            if ((OVRInput.GetDown(OVRInput.RawButton.A) || Input.GetButtonDown("Fire1")) && canShot == true && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();

            }
        }
    }

    //카메라가 보는 방향으로 총알을 생성
    void Fire()
    {
        //if (bullet != null)
        //    Instantiate(bullet, firePos.position, fpsCam.transform.rotation);
        GameObject bullet;
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Tutorial")
        {
            bullet = Tutorial.Instance.PopFromPool(bulletName);
            bullet.transform.rotation = new Quaternion(fpsCam.transform.rotation.x, fpsCam.transform.rotation.y, fpsCam.transform.rotation.z, fpsCam.transform.rotation.w);
            bullet.SetActive(true);
        }
        else if (currentScene.name == "Game")
        {
            bullet = GameManager.Instance.PopFromPool(bulletName);
            bullet.transform.rotation = new Quaternion(fpsCam.transform.rotation.x, fpsCam.transform.rotation.y, fpsCam.transform.rotation.z, fpsCam.transform.rotation.w);
            bullet.SetActive(true);
        }
        Quaternion FireRot = Quaternion.Euler(fpsCam.transform.rotation.x, 180, fpsCam.transform.rotation.z);

        AudioSource.PlayClipAtPoint(shootClip, transform.position);
    }

    IEnumerator AutoFire()
    {
        yield return new WaitForSeconds(.5f);
        while (true)
        {
            if (canShot == true && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Fire();
                yield return new WaitForSeconds(.5f);

            }
            yield return new WaitForEndOfFrame();
        }
    }


    //Fire()할떄마다 rotation잡아준다고 생각했는데 그게 아닌가보네
    //왜 한 번만 하는거지???
    //아...! 총알이 SetActive(false)가 되어있는 상태에서는 참조해서 바꿀 수가 없구나;;
    //참조해서 바꾸려면 총알들이 프리팹으로 public에 들어가있어야되는데
    //그렇게되려면 게임이 시작됐을때 총알을 maxbullets만큼 만들어서 이걸 전부 다
    //public GameObject 쪽에 복사된 것들을 넣어줘야되겠네
    //------------------------------------------------
    //위에처럼 해도 안됨.... 왜지.....
    //배열에 들어간거는 SetActive(false)여도 위에꺼처럼 참조가 가능한건가?
    //아니면 Prefab으로 해서 넣어주는거 아니면 절대로 참조가 안되는건가??
    //IEnumerator로 만져야 될거같은데...
    
}