using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBase : MonoBehaviour
{
    public int maxhp = 100;
    public int hp = 100;
    public bool IsAlive()
    {
        return hp > 0;
    }
    public virtual void GetDamage(int dmg)
    {
        if (!IsAlive())
        {
            return;
        }
        hp -= dmg;
    }
    public virtual void Die()
    {
        Destroy(gameObject, 1f);

        // ÖØÉú
    }
    public virtual void OpenFire() { }

    public virtual void AddHP(int addhp)
    {
        hp = hp + addhp >= maxhp ? maxhp : hp + addhp;
    }
}
