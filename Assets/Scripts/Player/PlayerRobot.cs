using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobot : RobotBase
{
    private static PlayerRobot instance;
    public static PlayerRobot GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;
    }

    // 跟随相机
    public Camera tpsCamera;

    // 技能参数
    public Skill skillPreb;
    public AudioSource skillAudio;
    public Transform skillPos;
    public float skillCd = 3f;
    public float skillDistance = 1000f;
    private float cdTimer = 0f;
    private bool skillUsed = false;

    // 武器参数
    private List<WeaponBase> weapons = new List<WeaponBase>();
    public WeaponBase defaultWeaponPrefab;
    public int weaponsNumMax = 2;               //每个角色所持有武器上限
    public Transform WeaponContainer;

    private float shootTimer = 0f;              //连续射击定时器
    private WeaponBase curWeapon;
    private int curWeaponsNum = 1;
    private int curWeaponIdx = 0;
    private bool reloadingFinish = true;        //Reload结束，动画帧事件调用
    private RaycastHit hit;                     //碰撞射击点
    private Vector3 remotePoint;                //远处射击点

    // 跳跃参数
    public float margin = 0.1f;
    public float jumpForce = 300f;
    private bool isGround = true;
    private Vector3 jumpMovement;

    // Audio参数
    public AudioSource getDamageAudio;
    public AudioSource walkAudio;
    public AudioSource runAudio;

    // 角色运动组件
    private bool isCrouch = false;
    private float crouchSpeedRatio = 0.5f;
    public float moveSpeed = 4f;
    private Vector3 movement;
    private float h;
    private float v;
    //private float shootMoveSpeedRatio = 0.2f;       //射击时移动速度减慢比例
    private Animator anim;
    private Rigidbody rigid;
    //private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        //cc = GetComponent<CharacterController>();
        GameObject newWeapon = Instantiate(defaultWeaponPrefab.gameObject, WeaponContainer.position, WeaponContainer.rotation, WeaponContainer);
        curWeapon = newWeapon.GetComponent<WeaponBase>();
        weapons.Add(curWeapon);
        ChangeWeapon(0);
    }

    // Update is called once per frame
    void Update()
    {
        // 技能
        Skill();

        // 开火
        Shoot();

        // 换武器
        ChangeWeapon();

        // UI更新
        HUD.GetInstance().UpdateWeaponUI(curWeapon.icon, curWeapon.GetCurBulletNum(),
                                         curWeapon.bulletsNumInBag, curWeapon.GetModeOne());
        HUD.GetInstance().UpdateHpUI(hp);

        // 背景音乐
        //BGAudioPlay();
    }

    private void FixedUpdate()
    {
        // 移动、旋转、跳跃
        Jump();
        MoveAndRotate();
    }

    private void Jump()
    {
        if (IsGround() && PlayerInput.GetJumpInput())
        {
            jumpMovement = movement;
            rigid.AddForce(Vector3.up * jumpForce);
            //ccSpeedY += jumpSpeed;
        }
        anim.SetBool("isGround", isGround);
        anim.SetFloat("speedY", rigid.velocity.y);
    }

    private bool IsGround()
    {
        isGround = Physics.Raycast(transform.position, -Vector3.up, margin);
        //// 增加四点检测 更不易卡边缘 但会卡墙上
        //isGround |= Physics.Raycast(transform.position + transform.forward * 0.2f, -Vector3.up, margin);
        //isGround |= Physics.Raycast(transform.position - transform.forward * 0.2f, -Vector3.up, margin);
        //isGround |= Physics.Raycast(transform.position + transform.right * 0.2f, -Vector3.up, margin);
        //isGround |= Physics.Raycast(transform.position - transform.right * 0.2f, -Vector3.up, margin);
        return isGround;
    }

    private void Skill()
    {
        if(skillUsed && cdTimer <= skillCd)
        {
            cdTimer += Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(1))
        {
            // 技能CD
            if (skillUsed)
            {
                if (cdTimer > skillCd)
                {
                    skillUsed = false;
                }
                else
                {
                    return;
                }
            }
            // 技能释放
            anim.SetTrigger("useSkill");
            cdTimer = 0;
            skillUsed = true;
        }
    }

    // 动画调用
    public void Throw()
    {
        LayerMask mask = ~(1 << 7); 
        if (Physics.Raycast(tpsCamera.transform.position, tpsCamera.transform.forward, out hit, skillDistance, mask))
        {
            Instantiate(skillPreb, skillPos.position, Quaternion.LookRotation(hit.point - skillPos.position));
        }
        else
        {
            Instantiate(skillPreb, skillPos.position, Quaternion.LookRotation(tpsCamera.transform.position + tpsCamera.transform.forward * skillDistance - skillPos.position));
        }
        skillAudio.Play();
    }

    private void MoveAndRotate()
    {
        // 使用(2)RigidBody或(1)Transfom控制
        if (isGround)
        {
            // Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouch = !isCrouch;
                anim.SetBool("isCrouch", isCrouch);
            }
            // 翻滚
            if (Input.GetKeyDown(KeyCode.Z) && !isCrouch)
            {
                anim.SetTrigger("roll");
            }
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            //if(anim.GetBool("isShoot"))
            //{
            //    h *= shootMoveSpeedRatio;
            //}
            movement = h * transform.right + v * transform.forward;
            anim.SetFloat("speed", v);
            anim.SetFloat("direction", h);
            movement.Normalize();
            // transform.position += movement * Time.deltaTime * moveSpeed;
            if (isCrouch)
            {
                rigid.MovePosition(rigid.position + movement * crouchSpeedRatio * moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                rigid.MovePosition(rigid.position + movement * moveSpeed * Time.fixedDeltaTime);
            }
            if (movement.magnitude < 0.1f)
            {
                walkAudio.Pause();
                runAudio.Pause();
            }
            else if (movement.magnitude < 0.5f)
            {
                walkAudio.Play();
            }
            else
            {
                runAudio.Play();
            }
        }
        else
        {
            // 保持起跳前的水平位移
            rigid.MovePosition(rigid.position + jumpMovement * moveSpeed * Time.fixedDeltaTime);
        }
        // 使用(3)Character Controller控制,待完成
        //Vector3 movement = PlayerInput.GetMovementInput(transform, anim);
        //movement.y -= gravity * Time.deltaTime;
        //cc.Move(movement * moveSpeed * Time.deltaTime);
    }

    //private void OnAnimatorIK(int layerIndex)
    //{
    //    remotePoint = tpsCamera.transform.position + tpsCamera.transform.forward * curWeaponShootDis;
    //    anim.SetLookAtPosition(remotePoint);
    //    anim.SetLookAtWeight(0.9f, 0.6f, 0.7f, 0.9f);//总权重、后面是各个身体部位的权重
    //}

    public void Shoot()
    {
        // 1.点射、连射模式切换
        if (Input.GetKeyUp(KeyCode.T))
        {
            curWeapon.ChangeMode();
        }

        // 2.换弹
        if (!reloadingFinish)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R) && curWeapon.bulletsNumInBag > 0 && curWeapon.GetCurBulletNum() < curWeapon.magizine)
        {
            Reloading();
        }
        if (curWeapon.GetCurBulletNum() <= 0)
        {
            if (curWeapon.bulletsNumInBag > 0)
            {
                Reloading();
            }
            // 保证当前弹夹子弹数量大于0才会进行射击
            return;
        }

        // 3.射击
        if (curWeapon.GetModeOne())
        {
            //点射
            if (PlayerInput.UpFire1())
            {
                anim.SetBool("isShoot", true);
                OpenFire();                              // 不由动画调用
            }
        }
        else
        {
            // 连射
            if (PlayerInput.KeepFire1())
            {
                shootTimer += Time.deltaTime;
                if (shootTimer >= curWeapon.GetShootWaitTime())
                {
                    shootTimer = 0;
                    anim.SetBool("isShoot", true);
                    OpenFire();
                }
            }
        }
    }

    private void Reloading()
    {
        curWeapon.reloadAudio.Play();
        anim.SetBool("reloading", true);
        reloadingFinish = false;
    }

    // 动画结束调用
    private void ReloadingFinish()
    {
        reloadingFinish = true;
        curWeapon.Reload();
    }


    public void SetisShoot()
    {
        anim.SetBool("isShoot", false);
    }

    public override void OpenFire()
    {
        base.OpenFire();
        //screenCenter.y = Screen.height / 2;
        //screenCenter.x = Screen.width / 2;
        //camRay = tpsCamera.ScreenPointToRay(screenCenter);
        LayerMask mask = ~(1 << 7);                            // 开启除了Ignore Bullet ，相当于~LayerMask.NameToLayer("Ignore Bullet")
        if (Physics.Raycast(tpsCamera.transform.position, tpsCamera.transform.forward, out hit, curWeapon.shootDistance, mask))
        {
            // 子弹发射方向，采用简易校准方式:1.至摄像机到武器射程内焦点处
            curWeapon.OpenFire(hit.point);
        }
        else
        {
            remotePoint = tpsCamera.transform.position + tpsCamera.transform.forward * curWeapon.shootDistance;
            // 2.至射程最远处
            //curWeapon.OpenFire(camRay.GetPoint(curWeaponShootDis));
            curWeapon.OpenFire(remotePoint);
        }
    }

    private void ChangeWeapon()
    {
        float f = Input.GetAxis("Mouse ScrollWheel");
        if (f > 0)
        {
            ChangeWeapon(1);
        }
        else if (f < 0)
        {
            ChangeWeapon(-1);
        }
    }

    public override void GetDamage(int dmg)
    {
        if (!IsAlive())
        {
            return;
        }
        base.GetDamage(dmg);
        getDamageAudio.Play();
        if (!IsAlive())
        {
            anim.SetTrigger("die");
            Die();
        }
    }

    private void ChangeWeapon(int f)
    {
        int index = (curWeaponIdx + f + weapons.Count) % weapons.Count;
        curWeapon.gameObject.SetActive(false);
        curWeapon = weapons[index];
        curWeapon.gameObject.SetActive(true);
        curWeaponIdx = index;
    }

    public void AddWeapon(WeaponBase weapon)
    {
        // 相同武器补子弹
        if (curWeapon.weaponID == weapon.weaponID)
        {
            curWeapon.AddBulletsNum(weapon);
            return;
        }

        // 生成武器并拿在手里
        GameObject newWeapon = Instantiate(weapon.gameObject, WeaponContainer.position, WeaponContainer.rotation, WeaponContainer);
        WeaponBase newWeaponBase = newWeapon.GetComponent<WeaponBase>();
        if(curWeaponsNum >= weaponsNumMax)
        {
            // 达到持有武器数量上限，替换
            weapons[curWeaponIdx] = newWeaponBase;
            ChangeWeapon(0);
        }
        else
        {
            curWeaponsNum++;
            weapons.Add(newWeaponBase);
            ChangeWeapon(weapons.Count - 1 - curWeaponIdx);
        }
    }
}
