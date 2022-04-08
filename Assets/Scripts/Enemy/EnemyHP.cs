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
        //ÿ���յ��˺��Զ������˺����ֵ�ʵ��
        GameObject text = Instantiate(dmgText, transform.position,transform.rotation,transform);
        text.GetComponent<Text>().text = "-" + dmg.ToString();
    }

    //Update is called once per frame
    void Update()
    {
        //����ʼ�ճ��������
        transform.rotation = tpCamera.rotation;
    }
}