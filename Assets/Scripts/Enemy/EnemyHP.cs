using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public GameObject dmgText;
    private Transform tpCamera;

    // Start is called before the first frame update
    private void Start()
    {
        tpCamera = transform.parent.GetComponent<EnemyRobot>().tpCamera;
    }

    public void DamageShow(int dmg)
    {
        //每次收到伤害自动生成伤害跳字的实例
        GameObject text = Instantiate(dmgText, transform.position,transform.rotation,transform);
        text.GetComponent<Text>().text = "-" + dmg.ToString();
    }

    //Update is called once per frame
    void Update()
    {
        //画布始终朝向摄像机
        transform.rotation = tpCamera.rotation;
    }
}