using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public ParticleSystem hitParticlePrefab;
    public ParticleSystem hitBloodPrefab;
    public int power = 40;                  // 应该写在枪械类上

    //public float speed = 1f;

    private void Start()
    {
        Invoke("DestroyProjectile", 10f);
    }

    //private void FixedUpdate()
    //{
    //    transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
    //}

    private void OnCollisionEnter(Collision other)
    {
        Vector3 hitNormal = other.contacts[0].normal;
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerRobot>().GetDamage(power);
            ParticleSystem preb = Instantiate(hitBloodPrefab, other.transform.position, Quaternion.Euler(hitNormal.x, hitNormal.y, hitNormal.z));
            Destroy(preb.gameObject, 1f);
        }
        else if (other.gameObject.tag == "Enemy")
        {
            EnemyRobot enemy = other.gameObject.GetComponent<EnemyRobot>();
            enemy.GetDamage(power);
            enemy.IsDamaged = true;
            ParticleSystem preb = Instantiate(hitBloodPrefab, other.transform.position, Quaternion.Euler(hitNormal.x, hitNormal.y, hitNormal.z));
            Destroy(preb.gameObject, 1f);
        }
        else
        {
            ParticleSystem preb = Instantiate(hitParticlePrefab, other.transform.position, Quaternion.Euler(hitNormal.x, hitNormal.y, hitNormal.z));
            Destroy(preb.gameObject, 1f);
        }
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
