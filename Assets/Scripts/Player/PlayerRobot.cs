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

    // �������
    public Camera tpsCamera;

    // ���ܲ���
    public Skill skillPreb;
    public AudioSource skillAudio;
    public Transform skillPos;
    public float skillCd = 3f;
    public float skillDistance = 1000f;
    private float cdTimer = 0f;
    private bool skillUsed = false;

    // ��������
    private List<WeaponBase> weapons = new List<WeaponBase>();
    public WeaponBase defaultWeaponPrefab;
    public int weaponsNumMax = 2;               //ÿ����ɫ��������������
    public Transform WeaponContainer;

    private float shootTimer = 0f;              //���������ʱ��
    private WeaponBase curWeapon;
    private int curWeaponsNum = 1;
    private int curWeaponIdx = 0;
    private bool reloadingFinish = true;        //Reload����������֡�¼�����
    private RaycastHit hit;                     //��ײ�����
    private Vector3 remotePoint;                //Զ�������

    // ��Ծ����
    public float margin = 0.1f;
    public float jumpForce = 300f;
    private bool isGround = true;
    private Vector3 jumpMovement;

    // Audio����
    public AudioSource getDamageAudio;
    public AudioSource walkAudio;
    public AudioSource runAudio;

    // ��ɫ�˶����
    private bool isCrouch = false;
    private float crouchSpeedRatio = 0.5f;
    public float moveSpeed = 4f;
    private Vector3 movement;
    private float h;
    private float v;
    //private float shootMoveSpeedRatio = 0.2f;       //���ʱ�ƶ��ٶȼ�������
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
        // ����
        Skill();

        // ����
        Shoot();

        // ������
        ChangeWeapon();

        // UI����
        HUD.GetInstance().UpdateWeaponUI(curWeapon.icon, curWeapon.GetCurBulletNum(),
                                         curWeapon.bulletsNumInBag, curWeapon.GetModeOne());
        HUD.GetInstance().UpdateHpUI(hp);

        // ��������
        //BGAudioPlay();
    }

    private void FixedUpdate()
    {
        // �ƶ�����ת����Ծ
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
        //// �����ĵ��� �����׿���Ե ���Ῠǽ��
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
            // ����CD
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
            // �����ͷ�
            anim.SetTrigger("useSkill");
            cdTimer = 0;
            skillUsed = true;
        }
    }

    // ��������
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
        // ʹ��(2)RigidBody��(1)Transfom����
        if (isGround)
        {
            // Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouch = !isCrouch;
                anim.SetBool("isCrouch", isCrouch);
            }
            // ����
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
            // ��������ǰ��ˮƽλ��
            rigid.MovePosition(rigid.position + jumpMovement * moveSpeed * Time.fixedDeltaTime);
        }
        // ʹ��(3)Character Controller����,�����
        //Vector3 movement = PlayerInput.GetMovementInput(transform, anim);
        //movement.y -= gravity * Time.deltaTime;
        //cc.Move(movement * moveSpeed * Time.deltaTime);
    }

    //private void OnAnimatorIK(int layerIndex)
    //{
    //    remotePoint = tpsCamera.transform.position + tpsCamera.transform.forward * curWeaponShootDis;
    //    anim.SetLookAtPosition(remotePoint);
    //    anim.SetLookAtWeight(0.9f, 0.6f, 0.7f, 0.9f);//��Ȩ�ء������Ǹ������岿λ��Ȩ��
    //}

    public void Shoot()
    {
        // 1.���䡢����ģʽ�л�
        if (Input.GetKeyUp(KeyCode.T))
        {
            curWeapon.ChangeMode();
        }

        // 2.����
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
            // ��֤��ǰ�����ӵ���������0�Ż�������
            return;
        }

        // 3.���
        if (curWeapon.GetModeOne())
        {
            //����
            if (PlayerInput.UpFire1())
            {
                anim.SetBool("isShoot", true);
                OpenFire();                              // ���ɶ�������
            }
        }
        else
        {
            // ����
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

    // ������������
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
        LayerMask mask = ~(1 << 7);                            // ��������Ignore Bullet ���൱��~LayerMask.NameToLayer("Ignore Bullet")
        if (Physics.Raycast(tpsCamera.transform.position, tpsCamera.transform.forward, out hit, curWeapon.shootDistance, mask))
        {
            // �ӵ����䷽�򣬲��ü���У׼��ʽ:1.�����������������ڽ��㴦
            curWeapon.OpenFire(hit.point);
        }
        else
        {
            remotePoint = tpsCamera.transform.position + tpsCamera.transform.forward * curWeapon.shootDistance;
            // 2.�������Զ��
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
        // ��ͬ�������ӵ�
        if (curWeapon.weaponID == weapon.weaponID)
        {
            curWeapon.AddBulletsNum(weapon);
            return;
        }

        // ������������������
        GameObject newWeapon = Instantiate(weapon.gameObject, WeaponContainer.position, WeaponContainer.rotation, WeaponContainer);
        WeaponBase newWeaponBase = newWeapon.GetComponent<WeaponBase>();
        if(curWeaponsNum >= weaponsNumMax)
        {
            // �ﵽ���������������ޣ��滻
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
