using UnityEngine;

public class BulletPlayer : MonoBehaviour
{
    [Header("Config Player Bullet")]
    [SerializeField]private float speed = 20f;
    [SerializeField]private float lifetime = 2f;
    private float spawnTime;
    [HideInInspector]public int dmg;

    void OnEnable()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Time.time - spawnTime > lifetime)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>().RecibirDaño(dmg);
            gameObject.SetActive(false);

        } else{

            gameObject.SetActive(false);
            
        }   
        
    }
}