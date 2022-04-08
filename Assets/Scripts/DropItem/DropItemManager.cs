using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItemManager : MonoBehaviour
{
    public List<GameObject> weapons;
    public GameObject bloodBag;
    private int weaponsNum;
    void Awake()
    {
        weaponsNum = weapons.Count;
    }

    public int WeaponsNum
    {
        get
        {
            return weaponsNum;
        }
    }

}
