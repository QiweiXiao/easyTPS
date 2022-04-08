using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public ParticleSystem hitParticlePrefab;
    public float dizzyTime = 3f;
    public float radius = 3f;
    public float skillSpeed = 70f;

    //public float speed = 1f;

    private void Start()
    {
        Invoke("DestroyProjectile", 10f);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * skillSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Vector3 hitNormal = other.contacts[0].normal;
        ParticleSystem preb = Instantiate(hitParticlePrefab, other.transform.position, Quaternion.identity);           // Quaternion.Euler(hitNormal.x, hitNormal.y, hitNormal.z)
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        for (int i = 0; i < colliders.Length; i++)
        {
            EnemyRobot enemy = colliders[i].GetComponent<EnemyRobot>();
            enemy.GetDamage(50);
            enemy.Dizzy(dizzyTime);
        }
        Destroy(preb.gameObject, 1.5f);
        DestroyProjectile();
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}