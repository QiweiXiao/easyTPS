using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropGuns : MonoBehaviour
{
    
    public List<GameObject> weapons;
    public int weaponsNum = 20;
    // ����������
    public float xMax = 15f;
    public float xMin = -20f;
    public float zMax = 10f;
    public float zMin = -15f;
    public float y = -0.4f;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 position;
        // ÿ����������һ��
        foreach (GameObject weapon in weapons)
        {
            position = new Vector3(Random.Range(xMin,xMax),y, Random.Range(zMin, zMax));
            Instantiate(weapon, position, Quaternion.identity);
        }

        // Ȼ�����
        int index = 0;
        for (int i = 0; i < weaponsNum - weapons.Count; ++i)
        {
            index = Random.Range(0, weapons.Count);
            position = new Vector3(Random.Range(xMin, xMax), y, Random.Range(zMin, zMax));
            Instantiate(weapons[index], position, Quaternion.identity);
        }
    }

}
