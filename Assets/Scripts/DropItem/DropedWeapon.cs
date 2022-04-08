using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropedWeapon : MonoBehaviour
{
    public WeaponBase weapon;


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            // 询问是否捡起
            HUD.GetInstance().UpdateQuery(weapon.gameObject, true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                other.GetComponent<PlayerRobot>().AddWeapon(weapon);
                HUD.GetInstance().UpdateQuery(weapon.gameObject, false);
                Destroy(gameObject);        //待改为 gameObject.SetActive(false)；
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            // 关闭询问
            HUD.GetInstance().UpdateQuery(weapon.gameObject, false);
        }
    }

}
