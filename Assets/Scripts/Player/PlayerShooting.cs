using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting essentials")]
    [SerializeField]private Transform firePoint;
    public BulletPool bulletPool;
    [SerializeField]private float fireRate = 0.2f;
    [SerializeField]private int bulletDmg = 10;
    private float lastShotTime = 0f;
    bool canAttack = true;

    [Header("Animator")]
    public Animator anim;

    void Update()
    {
        if(canAttack)
        {
            if (Input.GetButton("Fire1") && Time.time - lastShotTime > fireRate)
            {
                anim.SetTrigger("Shoot");
                canAttack = false;
                Invoke("ResetAttck", fireRate);
            }
        }
        
    }

    public void Shoot()
    {
        lastShotTime = Time.time;
        GameObject bullet = bulletPool.GetBullet(bulletDmg);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);
    }

    void ResetAttck()
    {
        canAttack = true;
    }
}