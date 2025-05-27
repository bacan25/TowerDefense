using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class BulletPool : MonoBehaviour
    {
        [Header("Pool essentials")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private int poolSize = 20;
        [SerializeField] private Transform bulletsParent;

        private List<GameObject> bullets = new List<GameObject>();

        void Start()
        {
            for (int i = 0; i < poolSize; i++)
            {
                var obj = Instantiate(bulletPrefab, bulletsParent);
                obj.SetActive(false);
                bullets.Add(obj);
            }
        }

        public GameObject GetBullet(int givenDmg)
        {
            foreach (var bullet in bullets)
            {
                if (!bullet.activeInHierarchy)
                {
                    var bp = bullet.GetComponent<BulletPlayer>();
                    bp.dmg = givenDmg;
                    return bullet;
                }
            }

            var newBullet = Instantiate(bulletPrefab, bulletsParent);
            var newBp = newBullet.GetComponent<BulletPlayer>();
            newBp.dmg = givenDmg;
            newBullet.SetActive(false);
            bullets.Add(newBullet);
            return newBullet;
        }
    }
}
