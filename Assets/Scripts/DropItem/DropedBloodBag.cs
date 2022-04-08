using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedBloodBag : MonoBehaviour
{
    public int healingAmount = 20;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // »ØÑª
            other.GetComponent<PlayerRobot>().AddHP(healingAmount);
            Destroy(gameObject, 0.5f);
        }
    }
}
