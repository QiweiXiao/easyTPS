using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponBase : MonoBehaviour
{
    public int weaponID;
    public Sprite icon;
    public Transform muzzle;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleParticlePrefab;
    public float bulletSpeed = 25f;
    public float shootDistance = 250f;
    public float shootWaitTime = 0.1f;
    public int bulletsNumInBag = 60;                //枪的备用弹夹的子弹数量
    public int bulletsNumMax = 200;                 //备用弹夹的子弹数量上限
    public int magizine = 30;                       //单个弹夹的子弹数量

    private bool isShootOne = true;                 //点射
    private int curBulletNum = 0;                   //当前弹夹子弹数量

    // Audio
    public AudioSource fireAudio;
    public AudioSource reloadAudio;

    public int GetCurBulletNum()
    {
        return curBulletNum;
    }

    public bool GetModeOne()
    {
        return isShootOne;
    }

    public void ChangeMode()
    {
        isShootOne = !isShootOne;
    }

    public float GetShootWaitTime()
    {
        return shootWaitTime;
    }

    // position 为射击点
    public void OpenFire(Vector3 position)
    {
        Vector3 dir = position - muzzle.position;
        dir.Normalize();
        GameObject bullet = Instantiate(bulletPrefab, muzzle.position + 0.6f * dir, Quaternion.LookRotation(dir) * Quaternion.AngleAxis(90, Vector3.right));
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * bulletSpeed;
        --curBulletNum;
        // Instantiate(muzzleParticlePrefab, muzzle.position, muzzle.rotation);
        muzzleParticlePrefab.Play();
        fireAudio.Play();
    }

    public void Reload()
    {
        if (bulletsNumInBag > magizine - curBulletNum)
        {
            bulletsNumInBag -= (magizine - curBulletNum);
            curBulletNum = magizine;
        }
        else
        {
            curBulletNum += bulletsNumInBag;
            bulletsNumInBag = 0;
        }
    }

    public void AddBulletsNum(int num)
    {
        if (bulletsNumInBag + num <= bulletsNumMax)
        {
            bulletsNumInBag += num;
        }
        else
        {
            bulletsNumInBag = bulletsNumMax;
        }
    }

    public void AddBulletsNum(WeaponBase weapon)
    {
        AddBulletsNum(weapon.bulletsNumInBag);
    }

    //public void UpdateWeaponParam(WeaponBase weapon)
    //{
    //    icon = weapon.icon;
    //    muzzle = weapon.muzzle;
    //    bulletPrefab = weapon.bulletPrefab;
    //    muzzleParticlePrefab = weapon.muzzleParticlePrefab;
    //    fireAudio = weapon.fireAudio;
    //    reloadAudio = weapon.reloadAudio;
    //    bulletSpeed = weapon.bulletSpeed;
    //    shootDistance = weapon.shootDistance;
    //    shootWaitTime = weapon.shootWaitTime;
    //    bulletsNumInBag = weapon.bulletsNumInBag;
    //    bulletsNumMax = weapon.bulletsNumMax;
    //    magizine = weapon.magizine;
    //}
}
