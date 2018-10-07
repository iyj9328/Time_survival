using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRay : MonoBehaviour {

    public int Damage = 1;
    public float fireRate = 0.25f;
    public float weaponRange = 500f;
    public float hitForce = 100f;
    public Transform gunEnd;
    public float ShootDelay = 0.05f;

    private Camera fpsCam;
    private LineRenderer line;
    private float nextFire;
    // Use this for initialization
    void Start()
    {
        line = GetComponent<LineRenderer>();
        fpsCam = GetComponentInChildren<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(ShotEffect());
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            line.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {
                Enemy other = hit.collider.GetComponent<Enemy>();
                Portal po = hit.collider.GetComponent<Portal>();
                Item it = hit.collider.GetComponent<Item>();
                Bomb bomb = hit.collider.GetComponent<Bomb>();
                if (other != null)
                {
                    other.GetDamage(Damage);
                }
                if (po != null)
                {
                    po.parent.GetComponent<Enemy>().GetDamage(Damage);
                    Destroy(po.gameObject);
                }
                if (it != null)
                {
                    it.GetDamage();
                }
                if (bomb != null)
                {
                    bomb.GetDamage();
                }
                line.SetPosition(1, hit.point);
            }
            else
            {
                line.SetPosition(1, fpsCam.transform.forward * weaponRange);
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        line.enabled = true;
        yield return new WaitForSeconds(ShootDelay);
        line.enabled = false;
    }
}
