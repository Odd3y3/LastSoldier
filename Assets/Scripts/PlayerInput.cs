using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : PlayerInfo
{
    Animator anim;
    Rigidbody rb;
    CapsuleCollider col;

    [Header("PlayerInput script")]
    public Transform headBone;
    public Transform bulletSpawnPos;
    public Transform bulletFlashPos;
    public GameObject bulletPrefab;

    [SerializeField]
    CameraArm cameraArm;
    [SerializeField]
    SoundObject soundObj;
    
    //애니메이션 Hash ID
    readonly int ANIM_ID_IsZoom = Animator.StringToHash("IsZoom");
    readonly int ANIM_ID_IsJump = Animator.StringToHash("IsJump");
    readonly int ANIM_ID_IsGround = Animator.StringToHash("IsGround");
    readonly int ANIM_ID_x = Animator.StringToHash("x");
    readonly int ANIM_ID_y = Animator.StringToHash("y");
    readonly int ANIM_ID_Shoot = Animator.StringToHash("Shoot");
    readonly int ANIM_ID_Die = Animator.StringToHash("Die");

    //LayerMask
    public LayerMask groundMask;
    public LayerMask enemyMask;
    public LayerMask itemMask;
    public LayerMask startButtonMask;

    PlayerInputActions playerInputActions;

    private bool canMove = true;
    public bool CanMove
    {
        get { return canMove; }
        set
        {
            if(value)
            {
                playerInputActions.Player.Enable();
                canMove = true;
            }
            else
            {
                playerInputActions.Player.Disable();
                canMove = false;
            }
        }
    }

    private bool canUseUI = true;
    public bool CanUseUI
    {
        get { return canUseUI; }
        set
        {
            if(value)
            {
                playerInputActions.UI.Enable();
                canUseUI = true;
            }
            else
            {
                playerInputActions.UI.Disable();
                canUseUI = false;
            }
        }
    }

    private bool isCollision = true;
    public bool IsCollision
    {
        get { return isCollision; }
        set
        {
            isCollision = value;
            if(value)
            {
                Col.enabled = true;
                rb.isKinematic = false;
            }
            else
            {
                //Collider 제거, 움직이지 않게 Kinematic 설정
                Col.enabled = false;
                rb.isKinematic = true;
            }
        }
    }

    private bool isZoom = false;
    private bool isGround = true;
    private bool isFiring = false;
    private bool wasGround = true;
    private bool isJump = false;

    


    //애니메이션 보간 속도(0 ~ 1)
    public float animSmoothSpeed = 0.3f;
    //마우스 감도
    public float mouseSensitivity = 0.1f;

    Vector2 curMoveVec = Vector2.zero;
    Vector2 targetMoveVec = Vector2.zero;
    Vector2 mouseDeltaVec = Vector2.zero;

    //마우스 위,아래 범위
    public Vector2 mouseLookUpRange = new Vector2(-90.0f, 90.0f);
    public Vector2 mouseLookUpAnimationRange = new Vector2(-60.0f, 50.0f);
    float mouseYRot = 0.0f;

    //점프
    float CheckGroundDistance = 0.3f;
    Vector3 CheckGroundOffSet = Vector3.up * 0.1f;

    //애니메이션에 맞춘 기본 이동속도
    const float BASE_MOVE_MUL_SPEED = 2.85f;

    //인 게임에서 보고있는 아이템 정보
    Item cursorItem = null;
    bool isShowItemInfo = false;

    bool isShowStartButton = false;


    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        if (cameraArm == null)
            cameraArm = Camera.main.transform.parent.GetComponent<CameraArm>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Move.performed += Move;
        playerInputActions.Player.Move.canceled += Move;
        playerInputActions.Player.Zoom.performed += Zoom_performed;
        playerInputActions.Player.Zoom.canceled += Zoom_canceled;
        playerInputActions.Player.Look.performed += MouseTurn;
        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Fire.performed += Fire_performed;
        playerInputActions.Player.Fire.canceled += Fire_canceled;
        playerInputActions.Player.Interaction.performed += InteractionKeyDown;
        playerInputActions.UI.Inventory.performed += ShowInventory;
        playerInputActions.UI.Inventory.canceled += HideInventory;
        playerInputActions.UI.UnlockCursor.performed += UnlockCursor_performed;
        playerInputActions.UI.EscMenu.performed += EscMenu_performed;

        //GameManager에 등록.
        GameManager.inst.player = this;
    }

    private void Start()
    {
        Initialize();

        mouseYRot = cameraArm.transform.localEulerAngles.y;

        //마우스 커서 잠그기(fps 모드)
        GameManager.inst.uiManager.IsCursorLock = true;
    }

    private void Update()
    {
        //바닥체크
        GroundCheck();
        //에임에 마우스가 있다면 ItemInfo 띄우기
        ShowItemInfo();

        //체력, 마나 재생
        RegenerateHp(Time.deltaTime);
        RegenerateMp(Time.deltaTime);

        //isFiring 이고, 총알 쿨타임이 지났으면, 마나가 있으면(과부하가 아니면), 총알 발사
        bulletCoolTime += Time.deltaTime;
        float finalAttackSpeed = isZoom ? curZoomAttackSpeed : curAttackSpeed;
        if(isFiring && bulletCoolTime >= 1.0f / finalAttackSpeed && !isOverload)
        {
            Fire();
        }

    }

    private void ShowItemInfo()
    {
        //아이템 정보 보기
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        float rayRange = isZoom ? 3.5f : 4.7f;
        if (Physics.Raycast(ray, out RaycastHit hit, rayRange, itemMask | startButtonMask))
        {
            Item item = hit.transform.GetComponent<Item>();
            if (item != null && cursorItem != item)
            {
                cursorItem = item;
                isShowItemInfo = true;
                //Item정보 보여짐
                GameManager.inst.uiManager.itemInfoUI.Show(item.ItemInfo, item.transform);
            }

            StartButton startButton = hit.transform.GetComponent<StartButton>();
            if(startButton != null)
            {
                isShowStartButton = true;
                GameManager.inst.uiManager.ShowStartButton(startButton.transform);
            }
        }
        else
        {
            cursorItem = null;
            isShowItemInfo = false;
            isShowStartButton = false;
            //Item정보 Hide
            GameManager.inst.uiManager.itemInfoUI.Hide();
            GameManager.inst.uiManager.HideStartButton();
        }
    }

    private void FixedUpdate()  
    {
        curMoveVec = Vector2.Lerp(curMoveVec, targetMoveVec, animSmoothSpeed);

        if (IsCollision)
        {
            rb.velocity = new Vector3(targetMoveVec.x * BASE_MOVE_MUL_SPEED * curMoveSpeed
                , rb.velocity.y, targetMoveVec.y * BASE_MOVE_MUL_SPEED * curMoveSpeed);
            rb.velocity = transform.rotation * rb.velocity;

            //달리기시 이속 2배
            if (!isZoom)
            {
                rb.velocity = new Vector3(rb.velocity.x * 2, rb.velocity.y, rb.velocity.z * 2);
            }
        }
        
        anim.SetFloat(ANIM_ID_x, curMoveVec.x);
        anim.SetFloat(ANIM_ID_y, curMoveVec.y);

    }

    private void LateUpdate()
    {
        if (IsLive)
        {
            float headAngle = Mathf.Clamp(mouseYRot, mouseLookUpAnimationRange.x, mouseLookUpAnimationRange.y);
            headBone.RotateAround(headBone.position, transform.right, headAngle);
        }
    }

    public void Move(InputAction.CallbackContext callback)
    {
        targetMoveVec = callback.ReadValue<Vector2>();
    }
    private void Zoom_performed(InputAction.CallbackContext callback)
    {
        if(GameManager.inst.uiManager.IsCursorLock)
        {
            anim.SetBool(ANIM_ID_IsZoom, true);
            isZoom = true;
            cameraArm.Zoom(true);
        }
    }
    private void Zoom_canceled(InputAction.CallbackContext callback)
    {
        if (GameManager.inst.uiManager.IsCursorLock)
        {
            anim.SetBool(ANIM_ID_IsZoom, false);
            isZoom = false;
            cameraArm.Zoom(false);
        }
    }
    private void MouseTurn(InputAction.CallbackContext callback)
    {
        if(GameManager.inst.uiManager.IsCursorLock)
        {
            mouseDeltaVec = callback.ReadValue<Vector2>();

            mouseYRot -= mouseDeltaVec.y * mouseSensitivity;
            mouseYRot = Mathf.Clamp(mouseYRot, mouseLookUpRange.x, mouseLookUpRange.y);
            cameraArm.transform.localEulerAngles = Vector3.right * mouseYRot;
        
            transform.Rotate(Vector3.up * mouseDeltaVec.x * mouseSensitivity, Space.World);
        }
    }
    private void Jump(InputAction.CallbackContext callback)
    {
        if (!isJump && isGround)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * curJumpForce, ForceMode.Impulse);

            anim.SetBool(ANIM_ID_IsJump, true);
            isJump = true;
        }
    }

    /// <summary>
    /// Fire 버튼 누를 때, 호출되는 함수
    /// </summary>
    private void Fire_performed(InputAction.CallbackContext callback)
    {
        if (GameManager.inst.uiManager.IsCursorLock)
            isFiring = true;
    }

    /// <summary>
    /// Fire 버튼 뗄 때, 호출되는 함수
    /// </summary>
    private void Fire_canceled(InputAction.CallbackContext callback)
    {
        if (GameManager.inst.uiManager.IsCursorLock)
            isFiring = false;
    }

    /// <summary>
    /// 총알 발사하는 함수
    /// </summary>
    private void Fire()
    {
        if (GameManager.inst.uiManager.IsCursorLock)
        {
            bulletCoolTime = 0f;

            //발사 애니메이션
            anim.Play(ANIM_ID_Shoot, 1, 0f);

            //발사 Sound 재생
            soundObj.PlayClip(1);

            //소모값
            CurMp -= curBulletMpCost;

            //bullet spawn
            GameObject bullet = ObjectPool.inst.GetObject(bulletPrefab.name, bulletPrefab, GameManager.inst.inGameManager.particleRoot);
            bullet.transform.position = bulletSpawnPos.position;

            //총알 방향 설정 ( RayHit 하는 곳으로 )
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            //Ray시작지점을 Player앞쪽으로 함
            ray.origin += Vector3.Normalize(ray.direction) * Vector3.Distance(Camera.main.transform.position, bulletSpawnPos.position);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundMask | enemyMask))
            {
                bullet.transform.LookAt(hit.point);
            }
            else
            {
                bullet.transform.rotation = Camera.main.transform.rotation;
            }

            //bullet 정보 전달후 Fire
            Bullet bulletComp = bullet.GetComponent<Bullet>();
            if (bulletComp != null)
            {
                BulletData bulletData = new BulletData();
                bulletData.typeName = bulletPrefab.name;
                bulletData.damage = curAttackPoint;
                bulletData.speed = curBulletSpeed;
                bulletData.range = curBulletRange;
                bulletData.targetMask = enemyMask;
                bulletComp.Fire(bulletData, bulletFlashPos);
            }
        }
    }

    /// <summary>
    /// Interaction키를 눌렀을 때, 호출되는 함수(F key)
    /// </summary>
    private void InteractionKeyDown(InputAction.CallbackContext callback)
    {
        if (isShowStartButton)
        {
            GameManager.inst.inGameManager.startButton.StartGame();
        }
        //아이템 획득
        else if (isShowItemInfo)
        {
            GetItem(cursorItem);
            return;
        }

    }

    /// <summary>
    /// 인벤토리 키(tab) 눌렀을 때, 호출되는 함수
    /// </summary>
    private void ShowInventory(InputAction.CallbackContext callback)
    {
        GameManager.inst.uiManager.IsShowInventory = true;
    }
    /// <summary>
    /// 인벤토리 키(tab) 땠을 때, 호출되는 함수
    /// </summary>
    private void HideInventory(InputAction.CallbackContext callback)
    {
        GameManager.inst.uiManager.IsShowInventory = false;
        if(IsLive)
            GameManager.inst.uiManager.IsCursorLock = true;
    }

    /// <summary>
    /// 인벤토리 키 누른상태에서 우클릭 누르면, 호출되는 함수
    /// 마우스를 Unlock해서 사용할 수 있게 함.
    /// </summary>
    private void UnlockCursor_performed(InputAction.CallbackContext callback)
    {
        if(GameManager.inst.uiManager.IsShowInventory)
        {
            GameManager.inst.uiManager.IsCursorLock = false;
        }
    }

    /// <summary>
    /// 인 게임에서 Esc눌렀을 때, 호출되는 함수.
    /// EscMenu 껐다 켜기.
    /// </summary>
    private void EscMenu_performed(InputAction.CallbackContext callback)
    {
        if (GameManager.inst.uiManager.IsShowEscMenu)
        {
            //EscMenu 켜져있으면, 끄고
            GameManager.inst.uiManager.IsShowEscMenu = false;
        }
        else
        {
            //EscMenu 꺼져있으면, 키고
            GameManager.inst.uiManager.IsShowEscMenu = true;
        }
    }

    /// <summary>
    /// 바닥 체크
    /// </summary>
    private void GroundCheck()
    {
        wasGround = isGround;

        RaycastHit hit;
        Vector3 p1 = transform.position + col.center + transform.up * (col.height / 2 - col.radius) + CheckGroundOffSet;
        Vector3 p2 = transform.position + col.center - transform.up * (col.height / 2 - col.radius) + CheckGroundOffSet;
        if(Physics.CapsuleCast(p1, p2, col.radius, Vector3.down, out hit, CheckGroundDistance, groundMask))
        {
            isGround = true;
            anim.SetBool(ANIM_ID_IsGround, true);
        }
        else
        {
            isGround = false;
            anim.SetBool(ANIM_ID_IsGround, false);
        }

        //Landing
        if(isGround && !wasGround)
        {
            anim.SetBool(ANIM_ID_IsJump, false);
            isJump = false;
        }
    }

    /// <summary>
    /// 플레이어가 맞았을 때, Sound 재생
    /// </summary>
    protected override void PlayDamagedSound()
    {
        base.PlayDamagedSound();

        //0번이 피격 사운드
        soundObj.PlayClip(0);
    }

    /// <summary>
    /// 플레이어가 죽었을 때,
    /// </summary>
    protected override void Dead()
    {
        base.Dead();

        //PlayTime 정지
        GameManager.inst.inGameManager.IsPlay = false;

        //PlayerDead Sound 재생
        soundObj.PlayClip(2);

        //이동 및 조작 금지시키기
        CanMove = false;
        CanUseUI = false;


        //enemy 전체 제거
        Destroy(GameManager.inst.inGameManager.enemysRoot.gameObject);

        //Animation 재생
        anim.SetTrigger(ANIM_ID_Die);

        //Collider 제거, 움직이지 않게 Kinematic 설정
        IsCollision = false;

        //바라보는 방향(Y축) 리셋
        headBone.RotateAround(headBone.position, transform.right, 0f);

        //Wave 정지, EnemySpawn 정지
        GameManager.inst.inGameManager.enemySpawner.StopWave();

        //Left Hand Grip Disable
        if(anim != null && anim.TryGetComponent<LeftHandGrip>(out LeftHandGrip lhg))
        {
            lhg.enabled = false;
        }

        GameManager.inst.uiManager.GameOver();
        GameManager.inst.uiManager.IsCursorLock = false;

        GameManager.inst.soundManager.PlayBGM(BGM.GameOver);
    }

    /// <summary>
    /// 씬이 넘어가거나, 플레이어가 다시 생성될 때, 버그 방지
    /// </summary>
    private void OnDestroy()
    {
        playerInputActions.Disable();
    }
}