using UnityEngine;

namespace Player
{
    public class BulletPlayer : MonoBehaviour
    {
        [Header("Config Player Bullet")]
        [SerializeField] private float speed = 20f;
        [SerializeField] private float lifetime = 2f;
        private float spawnTime;
        private Transform target;
        [HideInInspector] public int dmg;

        void OnEnable()
        {
            spawnTime = Time.time;
        }

        void Update()
        {
            if (target != null)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                transform.forward = dir;
                transform.position += dir * speed * Time.deltaTime;
            }
            else transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (Time.time - spawnTime > lifetime)
                gameObject.SetActive(false);
        }

        public void SetTarget(Transform objetivo)
        {
            target = objetivo;
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Enemy"))
                col.GetComponent<Enemy>()?.RecibirDaño(dmg);
            gameObject.SetActive(false);
        }
    }
}
