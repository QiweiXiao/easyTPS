using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public float fov = 100f;
    public bool isPlayerInSight = false;
    public Vector3 lastSightPlayerPosition;

    private GameObject player;
    private PlayerRobot playerRobot;
    private SphereCollider col;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerRobot = player.GetComponent<PlayerRobot>();
        col = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            isPlayerInSight = false;
            Vector3 direction = player.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, direction);          // 0 <= angel <= 180
            if (angle < 0.5f * fov)
            {
                RaycastHit hit;
                if(Physics.Raycast(transform.position + 0.7f * transform.up, direction.normalized, out hit, col.radius))
                {
                    if (hit.transform.gameObject == player)
                    {
                        isPlayerInSight = true;
                        lastSightPlayerPosition = hit.point;
                    }
                }
            }
            // Destroy²»»á´¥·¢OnTriggerExit
            if (!playerRobot.IsAlive())
            {
                isPlayerInSight = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            isPlayerInSight = false;
        }
    }
}
