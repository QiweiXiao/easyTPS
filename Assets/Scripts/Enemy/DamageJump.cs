using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageJump : MonoBehaviour
{
    //�����ٶ�
    public float upSpeed = 0.5f;
    //����ʱ��
    public float upTime = 0.8f;
    //��ʱ��
    private float upTimer = 0f;
    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        while(upTimer < upTime)
        {
            upTimer += Time.deltaTime;
            //�������Ϲ�������С����͸�� 
            transform.Translate(Vector3.up * upSpeed * Time.deltaTime);
            text.fontSize--;
            text.color = new Color(0.5f, 0, 0, 1 - upTimer/ upTime);
            yield return null;
        }
        Destroy(gameObject);
    }

}
