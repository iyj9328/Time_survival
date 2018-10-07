using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PooledObject 
{

    public GameObject bullet = null;                //생성할 총알
    public string poolItemName = string.Empty;      //총알의 이름

    public int maxBullets = 0;                      //총알의 최대갯수

    [SerializeField]
    private List<GameObject> Magazine = new List<GameObject>();     //탄창

    //CreateBullets함수를 이용해 총알의 최대갯수만큼 탄창에 생성
    public void Initialize(Transform parent)
    {
        for (int i = 0; i < maxBullets; i++)
        {
            Magazine.Add(CreateBullets(parent));
        }
    }

    //총알을 Instantiate하여 Active상태를 false로 만들고 반환 
    private GameObject CreateBullets(Transform parent)
    {
        GameObject insertBullet = Object.Instantiate(bullet, parent) as GameObject;
        insertBullet.name = poolItemName;
        insertBullet.SetActive(false);

        return insertBullet;
    }

    //GameManager에서 호출되며, 미리 생성된 Pool에서 탄창(Magazine)의 첫 번째 인자에 위치한 총알을 반환하고 탄창에서 Remove
    public GameObject PopFromPool(Transform parent)
    {
        if (Magazine.Count == 0)
        {
            Magazine.Add(CreateBullets(parent));
        }
        GameObject bullet = Magazine[0];
        Magazine.RemoveAt(0);

        bullet.transform.SetParent(parent);
        return bullet;
    }

    //사용하고 난 총알을 매개변수로 반환받고 Active가 false인 상태로 다시 탄창(Magazine)에 저장
    public void PushToPool(GameObject bullet, Transform parent = null)
    {
        bullet.transform.SetParent(parent);
        Init(bullet);
        bullet.SetActive(false);
        Magazine.Add(bullet);
    }

    //총알의 Transform정보를 리셋
    public void Init(GameObject obj)
    {
        Vector3 obj_pos = new Vector3(0f, 1.7f, 0f);
        Quaternion obj_rot = Quaternion.identity;

        obj.transform.position = obj_pos;
        obj.transform.rotation = obj_rot;
    }
}