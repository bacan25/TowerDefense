using UnityEngine;
using Player;


public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting essentials")]
    [SerializeField]private Transform firePoint;
    public BulletPool bulletPool;
    [SerializeField]private float fireRate;
    [SerializeField]private int bulletDmg = 10;
    private float lastShotTime = 0f;
    bool canAttack = true;

    [Header("Animator")]
    public Animator anim;

    public AudioSource sfx;
    
    [Header("Secondary Fire")]
    private SecondaryFireSystem secondaryFire;
    
    void Start()
    {
        // Buscar o crear sistema de disparo secundario
        secondaryFire = GetComponent<SecondaryFireSystem>();
        if (secondaryFire == null)
        {
            secondaryFire = gameObject.AddComponent<SecondaryFireSystem>();
        }
    }
    
    void Update()
    {
        // Manejar input del disparo secundario
        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            SecondaryShootButtonPressed();
        }
    }

    public void ShootButtonPressed()
    {
        if(canAttack)
        {
            if (Time.time - lastShotTime > fireRate)
            {
                anim.SetTrigger("Shoot");
                canAttack = false;
                sfx.Play();
                Invoke("ResetAttck", fireRate);
            }
        }
    }

    //Esto se llama a través de la animación
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
    
    public void SecondaryShootButtonPressed()
    {
        if (secondaryFire != null && secondaryFire.EstaDisponible())
        {
            secondaryFire.IntentarDisparoSecundario();
        }
    }
}
