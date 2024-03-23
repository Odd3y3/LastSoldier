using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem.Controls;

enum EnemyState
{
    None, Create, Chase, Attack, Dead
}
public class Enemy : HitTarget
{
    [SerializeField]
    SoundObject soundObj;

    //Enemy 고유 번호
    public int enemyNum = 0;
    [SerializeField]
    bool isBoss = false;
    //Enemy 처치시 점수
    [SerializeField]
    int enemyScore = 0;
    //Enemy 처치시 Item 드랍 확률
    [SerializeField, Range(0f, 100.0f)]
    float itemDropProb = 100.0f;

    [SerializeField]
    EnemyState curState = EnemyState.None;

    protected Animator anim;
    NavMeshAgent agent;
    
    //Dissolve 표현하기 위해서 필요.
    [SerializeField]
    SkinnedMeshRenderer[] meshRenderers;
    Material[] orgMats = null;
    Material[] dissolveMats = null;

    //hpBar 링크(런타임중 제거하기 위해서)
    GameObject hpBar;

    Transform target = null;
    bool isAttacking = false;

    [SerializeField]
    float attackDetectRange = 0.5f;
    public float AttackDetectRange
    {
        get { return attackDetectRange; }
        set
        {
            attackDetectRange = value;
            agent.stoppingDistance = value;
        }
    }
    [SerializeField]
    protected LayerMask playerMask;

    //hpBar 위치 링크
    [SerializeField]
    Transform hpBarPos = null;

    //dissolve 가 진행되는 시간
    const float DISAPPEAR_TIME = 3.0f;

    private void Awake()
    {
        //Material 초기화
        int matLen = meshRenderers.Length;
        orgMats = new Material[matLen];
        dissolveMats = new Material[matLen];
        for (int i = 0; i < matLen; ++i)
        {
            //SkinnedMeshRenderer에 Materials에
            //0번째는 기존 Material, 1번째는 Dissolve용 Material
            orgMats[i] = meshRenderers[i].materials[0];
            dissolveMats[i] = meshRenderers[i].materials[1];
        }

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
    }
    private void OnEnable()
    {
        Initialize();

        ChangeState(EnemyState.Create);
    }

    private void Update()
    {
        ProcessState();
    }

    private void FixedUpdate()
    {
        FixedProcessState();
    }

    private void ChangeState(EnemyState state)
    {
        if (state == curState) return;
        curState = state;
        switch(state)
        {
            case EnemyState.Create:
                //보스라면 등장 Sound, UI
                if (isBoss)
                {
                    GameManager.inst.uiManager.AppearBossUI();
                    GameManager.inst.soundManager.PlayBossAppearSound();
                }

                //Material origin으로 변경
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].materials = new Material[1] { orgMats[i] };
                }

                //Collider enable
                if (Col != null)
                    Col.enabled = true;

                //hpBar
                GameObject hpBarOrg = isBoss ? Resources.Load<GameObject>("UI/EnemyBossHpBar")
                    : Resources.Load<GameObject>("UI/EnemyHpBar");
                hpBar = ObjectPool.inst.GetObject(isBoss ? "EnemyBossHpBar" : "EnemyHpBar", hpBarOrg, GameManager.inst.uiManager.enemyBarsObj.transform);
                if (hpBar.TryGetComponent(out EnemyHpBar bar))
                    bar.SetTarget(this, hpBarPos);

                //NavmeshAgent
                agent.speed = curMoveSpeed;
                agent.stoppingDistance = attackDetectRange;
                agent.isStopped = true;

                target = GameManager.inst.player.transform;
                SetTarget(target);
                break;

            case EnemyState.Chase:
                //Chase 할 때만, isStop 끄기
                agent.isStopped = false;
                anim.SetBool("IsMove", true);
                break;

            case EnemyState.Attack:
                agent.isStopped = true;
                anim.SetBool("IsMove", false);
                anim.SetBool("IsAttack", true);
                Attack();

                break;

            case EnemyState.Dead:
                agent.isStopped = true;
                anim.SetBool("IsDead", true);
                anim.SetTrigger("Die");
                if(Col != null)
                    Col.enabled = false;

                //점수 추가
                GameManager.inst.inGameManager.AddScore(enemyScore);

                //아이템 Drop
                if(Random.Range(0, 100.0f) <= itemDropProb)
                    Item.CreateItem(transform.position);


                ObjectPool.inst.ReleaseObject(isBoss ? "EnemyBossHpBar" : "EnemyHpBar", hpBar);
                Disappear();
                ObjectPool.inst.ReleaseObject("Enemy" + enemyNum.ToString(), gameObject, DISAPPEAR_TIME + 1.0f);
                break;
        }
    }

    //Update문에서 실행
    private void ProcessState()
    {
        switch(curState)
        {
            case EnemyState.Create:
                break;
            case EnemyState.Chase:
                break;
            case EnemyState.Attack:
                if (!anim.GetBool("IsAttack"))
                {
                    if (agent.remainingDistance > attackDetectRange)
                    {
                        ChangeState(EnemyState.Chase);
                    }
                    else if(!isAttacking)
                    {
                        Attack();
                    }
                }
                else
                {
                    isAttacking = false;
                }

                break;
            case EnemyState.Dead:
                break;
        }
    }

    //FixedUpdate 문에서 실행
    private void FixedProcessState()
    {
        switch (curState)
        {
            case EnemyState.Create:
                break;
            case EnemyState.Chase:
                agent.destination = target.position;
                if (agent.remainingDistance <= attackDetectRange)
                    ChangeState(EnemyState.Attack);
                break;
            case EnemyState.Attack:
                agent.destination = target.position;
                RotateToTarget(target);
                break;
            case EnemyState.Dead:
                break;
        }
    }

    virtual protected void Attack()
    {
        isAttacking = true;

        //각 Enemy마다 상속 받아서 공격 구현.
    }



    //회전 속도
    const float ROTATE_SPEED = 100.0f;
    //회전 잠금
    public bool LockRotate { get; set; } = false;
    //회전
    private void RotateToTarget(Transform target)
    {
        if (!LockRotate)
        {
            Vector3 dir = target.position - transform.position;
        
            float angle = Vector3.Angle(dir, transform.forward);

            float delta = ROTATE_SPEED * Time.fixedDeltaTime;
            if (delta > angle)
                delta = angle;

            if(Vector3.Dot(dir, transform.right) < 0)
            {
                    //왼쪽에 있을 때,
                delta = -delta;
            }

            transform.Rotate(Vector2.up * delta);
        }
    }

    //Start문에서 실행 ( Target(Player) 지정 )
    private void SetTarget(Transform target)
    {
        this.target = target;
        agent.destination = target.position;
        
        //t초 후에 Chase로 변경
        StartCoroutine(ChangingStateToChase(1.0f));
    }
    //t초 후에 Chase상태로 변경 ( 딜레이 주기 위해서 )
    IEnumerator ChangingStateToChase(float t)
    {
        yield return new WaitForSeconds(t);

        ChangeState(EnemyState.Chase);
    }

    /// <summary>
    /// UnityEvent로 Animation Event에 연결 될 함수.
    /// 각 Enemy가 상속 받아서 사용
    /// </summary>
    virtual public void OnAttack()
    {

    }

    /// <summary>
    /// 공격 사운드 재생하는 함수. Anim Event로 실행
    /// </summary>
    public void PlayAttackSound()
    {
        //공격 사운드 재생
        if (soundObj != null)
            soundObj.PlayClip(0);
    }

    public void PlayAttackSound2()
    {
        if (soundObj != null)
            soundObj.PlayClip(1);
    }

    protected override void Dead()
    {
        base.Dead();

        ChangeState(EnemyState.Dead);
    }

    /// <summary>
    /// 죽고 나서 캐릭터 서서히 사라지는 효과(Dissolve Shader 사용)
    /// </summary>
    private void Disappear()
    {
        for(int i = 0; i < meshRenderers.Length; ++i)
        {
            meshRenderers[i].material = dissolveMats[i];
            meshRenderers[i].material.SetFloat("_Cutoff", 0f);
        }
        StartCoroutine(DisappearCo(DISAPPEAR_TIME));
    }

    IEnumerator DisappearCo(float time)
    {
        float curTime = 0.0f;
        while (curTime < time)
        {
            foreach (var meshRenderer in meshRenderers)
            {
                ////천천히 사라지게 하기 위해서 x^2 함수 사용, 0 ~ 1
                meshRenderer.material.SetFloat("_Cutoff", (curTime / time) * (curTime / time));
            }

            curTime += Time.deltaTime;
            yield return null;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        isAttacked = false;
    }
}
