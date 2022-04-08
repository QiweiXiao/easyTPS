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

    //���
    private NavMeshAgent agent;
    private Animator anim;
    private EnemySight eyeSight;
    private EnemySight weaponSight;
    private GameObject player;
    private PlayerRobot playerRobot;


    public bool pause = false;              //��Ϸ��ͣ
    public AudioSource footAudio;
    public AudioSource getDamageAudio;
    private bool isDamaged = false;         // ��ǰ�Ƿ��ܻ��ж�
    private int isCoroutine = 0;            // �ܻ�Э�̵ȴ���
    public float unActTime = 1f;
    public Slider hp_Slider;                //Ѫ��
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
    public bool isGunner = true;            // Զ�����̹�
    public float shootWaitTime = 0.3f;
    private float shootTimer = 0f;
    public float cutWaitTime = 2f;
    private float cutTimer = 0f;
    public float errorAngle = 2f;           // ������������Ƕ�

    //Chasing
    public float chaseSpeed = 5f;
    public float chaseWaitTime = 5f;
    private float chaseTimer = 0f;
    public float rotationSpeed = 5f;

    // ���������Ʒ
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
        // �ܻ�����
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

        // Ѱ·������
        //if (!playerRobot.IsAlive())             //�������
        //{
        //    Patrolling();
        //}
        if (weaponSight.isPlayerInSight)   // ������Χ�ڣ�����
        {
            Attacking();
        }
        else if (eyeSight.isPlayerInSight)      // ��Ұ��Χ�ڣ�׷��
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
                wayPointIndex += Random.Range(1, pointsNum);    // ����ҿ�
                wayPointIndex %= pointsNum;
                patrolTimer = 0f;
            }
        }
        else
        {
            // ����ͣ���п���ȥ׷�������������£���ɵ��´�ͣ��ʱ�����̵����
            patrolTimer = 0f;
        }
        agent.SetDestination(wayPoints.GetChild(wayPointIndex).position);
    }

    private void Attacking()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        //��ָ���ٶ�ת�����
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
                    // ������ײ���,�������ӵ���ֻ������
                    Ray camRay = new Ray(transform.position + transform.up * 0.2f, newDir);        //����̧һЩ
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
                    // ֱ��ȥѪ
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
            //��ԭ����תһ������ֵ���
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
        //Ѫ������
        hp_Slider.value -= dmg * 100f / maxHP;
        //�����˺�����
        damageHP.DamageShow(dmg);
        if (!IsAlive())
        {
            anim.SetBool("isAlive", false);
            agent.isStopped = true;
            //������������
            anim.SetTrigger("die");
            Die();
            DropItems();
        }
    }

    // ��������Ѫ������Ʒ
    private void DropItems()
    {
        // Ȼ�����
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

    // PlayerSkill����
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
