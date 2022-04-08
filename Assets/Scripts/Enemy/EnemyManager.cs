using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> enemys;
    public int enemysNum = 5;
    // ���ɲ�����
    public float xMax = 13f;
    public float xMin = -10f;
    public float zMax = 33f;
    public float zMin = 19f;
    public float y = -6f;

    private Transform tpCamera;

    private void Start()
    {
        tpCamera = GameObject.Find("PlayerManager").GetComponent<PlayerManager>().GetCamera().transform;
    }

    // ȷ��Player������
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            for (int i = 0; i < enemysNum; ++i)
            {
                int index = Random.Range(0, enemys.Count);
                Vector3 position = new Vector3(Random.Range(xMin, xMax), y, Random.Range(zMin, zMax));
                GameObject enemy = Instantiate(enemys[index], position, Quaternion.identity);
                enemy.GetComponent<EnemyRobot>().tpCamera = tpCamera;
            }
        }
    }
}
