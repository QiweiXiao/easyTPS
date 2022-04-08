using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyRobot : RobotBase
{
    public int moreHP = 300;
    public Transform tpCamera;
    private int maxHP;
    public int damage = 8;

    //组件
    private NavMeshAgent agent;
    private Animator anim;
    private EnemySight eyeSight;
    private EnemySight weaponSight;
    private GameObject player;
    private PlayerRobot playerRobot;


    public bool pause = false;              //游戏暂停
    public AudioSource footAudio;
    public AudioSource getDamageAudio;
    private bool isDamaged = false;         // 当前是否受击判断
    private int isCoroutine = 0;            // 受击协程等待中
    public float unActTime = 1f;
    public Slider hp_Slider;                //血条
    private EnemyHP damageHP;
    // public WeaponBase curWeapon;

    //Patrolling
    public float patrolSpeed = 3f;
    public float patrolWaitTime = 1f;
    private float patrolTimer = 0f;
    public Transform wayPoints;
    private int wayPointIndex = 0;
    private int pointsNum;


    //Attacking
    public bool isGunner = true;            // 远、近程怪
    public float shootWaitTime = 0.3f;
    private float shootTimer = 0f;
    public float cutWaitTime = 2f;
    private float cutTimer = 0f;
    public float errorAngle = 2f;           // 允许射击的误差角度

    //Chasing
    public float chaseSpeed = 5f;
    public float chaseWaitTime = 5f;
    private float chaseTimer = 0f;
    public float rotationSpeed = 5f;

    // 死后掉落物品
    private DropItemManager dropItemManager;
    public int minBloodBagsNum = 0;
    public int maxBloodBagsNum = 2;
    public int minWeaponsNum = 0;
    public int maxWeaponsNum = 2;


    // Start is called before the first frame update
    void Start()
    {
        hp += moreHP;
        maxHP = hp;
        hp_Slider.value = 100;
        damageHP = transform.Find("Canvas").GetComponent<EnemyHP>();

        agent = GetComponent<NavMeshAgent>();
        pointsNum = wayPoints.childCount;
        wayPointIndex = Random.Range(0, pointsNum);
        agent.SetDestination(wayPoints.GetChild(wayPointIndex).position);

        anim = GetComponent<Animator>();
        anim.SetBool("isAlive", true);
        eyeSight = transform.Find("EyeSight").GetComponent<EnemySight>();
        weaponSight = transform.Find("WeaponSight").GetComponent<EnemySight>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRobot = player.GetComponent<PlayerRobot>();
        dropItemManager = GameObject.Find("DropItemManager").GetComponent<DropItemManager>();
        footAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // 受击反馈
        if (IsDamaged)
        {
            anim.SetTrigger("isDamaging");
            IsDamaged = false;
            //anim.SetBool("isDamage", true);
            _ = StartCoroutine("UnAct", unActTime);
            return;
        }
        if (isCoroutine > 0)
        {
            return;
        }

        // Audio
        if (pause || !IsAlive())
        {
            footAudio.Pause();
            return;
        }
        if (agent.speed < 0.05f)
        {
            footAudio.Pause();
        }
        else
        {
            footAudio.Play();
        }

        // 寻路、攻击
        //if (!playerRobot.IsAlive())             //玩家阵亡
        //{
        //    Patrolling();
        //}
        if (weaponSight.isPlayerInSight)   // 攻击范围内，攻击
        {
            Attacking();
        }
        else if (eyeSight.isPlayerInSight)      // 视野范围内，追击
        {
            Chasing();
            anim.SetFloat("speed", agent.speed / chaseSpeed);
        }
        else
        {
            Patrolling();
            anim.SetFloat("speed", agent.speed / patrolSpeed);
        }
    }

    public bool IsDamaged
    {
        get
        {
            return isDamaged;
        }
        set
        {
            isDamaged = value;
        }
    }

    IEnumerator UnAct(float time)
    {
        ++isCoroutine;
        agent.isStopped = true;
        yield return new WaitForSeconds(time);
        --isCoroutine;
        if (isCoroutine == 0) {
            agent.isStopped = false;
        }
    }

    private void Patrolling()
    {
        agent.isStopped = false;
        agent.speed = patrolSpeed;
        if(agent.remainingDistance < agent.stoppingDistance)
        {
            patrolTimer += Time.deltaTime;
            agent.speed = 0f;
            if(patrolTimer > patrolWaitTime)
            {
                wayPointIndex += Random.Range(1, pointsNum);    // 左闭右开
                wayPointIndex %= pointsNum;
                patrolTimer = 0f;
            }
        }
        else
        {
            // 避免停顿中可能去追击或者做其他事，造成的下次停顿时间缩短的情况
            patrolTimer = 0f;
        }
        agent.SetDestination(wayPoints.GetChild(wayPointIndex).position);
    }

    private void Attacking()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        //以指定速度转向玩家
        Vector3 lookPos = weaponSight.lastSightPlayerPosition;
        lookPos.y = transform.position.y;
        Vector3 targetDir = lookPos - transform.position;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, Time.deltaTime * rotationSpeed, 0);
        transform.rotation = Quaternion.LookRotation(newDir);
        if (isGunner)
        {
            if(Vector3.Angle(targetDir, transform.forward) < errorAngle)
            {
                shootTimer += Time.deltaTime;
                if(shootTimer > shootWaitTime)
                {
                    anim.SetBool("isShoot", true);
                    shootTimer = 0f;
                    OpenFire(lookPos);
                    // 射线碰撞检测,不发射子弹，只检测玩家
                    Ray camRay = new Ray(transform.position + transform.up * 0.2f, newDir);        //往上抬一些
                    RaycastHit hit;
                    if (Physics.Raycast(camRay, out hit, 300f, LayerMask.GetMask("Player")))
                    {
                        hit.collider.GetComponent<PlayerRobot>().GetDamage(damage);
                    }
                }
            }
        }
        else
        {
            if (Vector3.Angle(targetDir, transform.forward) < errorAngle)
            {
                cutTimer += Time.deltaTime;
                if (cutTimer > cutWaitTime)
                {
                    anim.SetTrigger("attack");
                    cutTimer = 0f;
                    // 直接去血
                    playerRobot.GetDamage(damage);
                }
            }
        }
    }

    private void Chasing()
    {
        agent.isStopped = false;
        agent.destination = eyeSight.lastSightPlayerPosition;
        agent.speed = chaseSpeed;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            //在原地旋转一会儿发现敌人
            transform.rotation = transform.rotation * Quaternion.AngleAxis(Time.deltaTime * rotationSpeed, transform.up);
            chaseTimer += Time.deltaTime;
            if (chaseTimer > chaseWaitTime)
            {
                chaseTimer = 0f;
                agent.speed = 0f;
                agent.isStopped = true;
            }
        }
        else
        {
            chaseTimer = 0f;
        }
    }

    public override void OpenFire() {
        base.OpenFire();
        // curWeapon.OpenFire(transform.forward);
    }

    public void OpenFire(Vector3 target)
    {
        base.OpenFire();
        // curWeapon.OpenFire(target - transform.position);
    }

    public override void GetDamage(int dmg)
    {
        if (!IsAlive())
        {
            return;
        }
        base.GetDamage(dmg);
        getDamageAudio.Play();
        //血条减少
        hp_Slider.value -= dmg * 100f / maxHP;
        //生成伤害跳字
        damageHP.DamageShow(dmg);
        if (!IsAlive())
        {
            anim.SetBool("isAlive", false);
            agent.isStopped = true;
            //播放死亡动画
            anim.SetTrigger("die");
            Die();
            DropItems();
        }
    }

    // 死亡掉落血包和物品
    private void DropItems()
    {
        // 然后随机
        Vector3 randpos = Vector3.zero;
        int weaponsNum = Random.Range(minWeaponsNum, maxWeaponsNum + 1);
        int bagsNum = Random.Range(minBloodBagsNum, maxBloodBagsNum + 1);
        for (int i = 0; i < weaponsNum; ++i)
        {
            int index = Random.Range(0, dropItemManager.WeaponsNum);
            randpos.x = Random.Range(-1, 1);
            randpos.z = Random.Range(-1, 1);
            Instantiate(dropItemManager.weapons[index], transform.position + randpos, Quaternion.identity);
        }
        for (int i = 0; i < bagsNum; ++i)
        {
            randpos.x = Random.Range(-1, 1);
            randpos.z = Random.Range(-1, 1);
            Instantiate(dropItemManager.bloodBag, transform.position + randpos, Quaternion.identity);
        }
    }

    // PlayerSkill调用
    public void Dizzy(float dizzyTime)
    {
        anim.SetTrigger("isDizzy");
        _ = StartCoroutine("Dizzying", dizzyTime);
    }

    IEnumerator Dizzying(float dizzyTime)
    {
        ++isCoroutine;
        agent.isStopped = true;
        yield return new WaitForSeconds(dizzyTime);
        --isCoroutine;
        if (isCoroutine == 0)
        {
            agent.isStopped = false;
        }
    }
}
