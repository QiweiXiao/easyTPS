using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    public Image weaponIcon;
    public Text bulletNumText;
    public Text hpNumText;
    public Text shootMode;
    public Text dropItem;
    public Slider hpSlider;

    private static HUD instance;
    private string semi = "SEMI";           // SAFE：保险；SEMI：点射/半自动；
    private string auto = "AUTO";           // AUTO：连射/全自动
    private void Awake()
    {
        instance = this;
    }

    public static HUD GetInstance()
    {
        return instance;
    }

    public void UpdateWeaponUI(Sprite icon, int bulletNum, int resNum, bool mode)
    {
        weaponIcon.sprite = icon;
        bulletNumText.text = bulletNum.ToString() + "/" + resNum.ToString();
        shootMode.text = mode ? "Mode:" + semi : "Mode:" + auto;
    }

    public void UpdateHpUI(int hpNum)
    {
        hpNumText.text = hpNum.ToString();
        hpSlider.value = hpNum;
    }

    public void UpdateQuery(GameObject obj, bool isShow)
    {
        if (isShow)
        {
            dropItem.text ="Press E and get " + obj.name + "!";
        }
        else
        {
            dropItem.text = "";
        }
    }

}
