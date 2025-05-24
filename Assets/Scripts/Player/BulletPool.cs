using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [Header("Pool essentials")]
    [SerializeField]private GameObject bulletPrefab;
    [SerializeField]private int poolSize = 20;
    [SerializeField]private Transform bulletsParent;

    private List<GameObject> bullets = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab, bulletsParent);
            obj.SetActive(false);
            bullets.Add(obj);
        }
    }

    public GameObject GetBullet(int givenDmg)
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                bullet.GetComponent<BulletPlayer>().dmg = givenDmg;
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(bulletPrefab, bulletsParent);
        newBullet.GetComponent<BulletPlayer>().dmg = givenDmg;
        newBullet.SetActive(false);
        bullets.Add(newBullet);
        return newBullet;
    }
}