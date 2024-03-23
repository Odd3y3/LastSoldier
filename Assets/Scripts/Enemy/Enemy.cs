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

    //Enemy ���� ��ȣ
    public int enemyNum = 0;
    [SerializeField]
    bool isBoss = false;
    //Enemy óġ�� ����
    [SerializeField]
    int enemyScore = 0;
    //Enemy óġ�� Item ��� Ȯ��
    [SerializeField, Range(0f, 100.0f)]
    float itemDropProb = 100.0f;

    [SerializeField]
    EnemyState curState = EnemyState.None;

    protected Animator anim;
    NavMeshAgent agent;
    
    //Dissolve ǥ���ϱ� ���ؼ� �ʿ�.
    [SerializeField]
    SkinnedMeshRenderer[] meshRenderers;
    Material[] orgMats = null;
    Material[] dissolveMats = null;

    //hpBar ��ũ(��Ÿ���� �����ϱ� ���ؼ�)
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

    //hpBar ��ġ ��ũ
    [SerializeField]
    Transform hpBarPos = null;

    //dissolve �� ����Ǵ� �ð�
    const float DISAPPEAR_TIME = 3.0f;

    private void Awake()
    {
        //Material �ʱ�ȭ
        int matLen = meshRenderers.Length;
        orgMats = new Material[matLen];
        dissolveMats = new Material[matLen];
        for (int i = 0; i < matLen; ++i)
        {
            //SkinnedMeshRenderer�� Materials��
            //0��°�� ���� Material, 1��°�� Dissolve�� Material
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
                //������� ���� Sound, UI
                if (isBoss)
                {
                    GameManager.inst.uiManager.AppearBossUI();
                    GameManager.inst.soundManager.PlayBossAppearSound();
                }

                //Material origin���� ����
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
                //Chase �� ����, isStop ����
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

                //���� �߰�
                GameManager.inst.inGameManager.AddScore(enemyScore);

                //������ Drop
                if(Random.Range(0, 100.0f) <= itemDropProb)
                    Item.CreateItem(transform.position);


                ObjectPool.inst.ReleaseObject(isBoss ? "EnemyBossHpBar" : "EnemyHpBar", hpBar);
                Disappear();
                ObjectPool.inst.ReleaseObject("Enemy" + enemyNum.ToString(), gameObject, DISAPPEAR_TIME + 1.0f);
                break;
        }
    }

    //Update������ ����
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

    //FixedUpdate ������ ����
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

        //�� Enemy���� ��� �޾Ƽ� ���� ����.
    }



    //ȸ�� �ӵ�
    const float ROTATE_SPEED = 100.0f;
    //ȸ�� ���
    public bool LockRotate { get; set; } = false;
    //ȸ��
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
                    //���ʿ� ���� ��,
                delta = -delta;
            }

            transform.Rotate(Vector2.up * delta);
        }
    }

    //Start������ ���� ( Target(Player) ���� )
    private void SetTarget(Transform target)
    {
        this.target = target;
        agent.destination = target.position;
        
        //t�� �Ŀ� Chase�� ����
        StartCoroutine(ChangingStateToChase(1.0f));
    }
    //t�� �Ŀ� Chase���·� ���� ( ������ �ֱ� ���ؼ� )
    IEnumerator ChangingStateToChase(float t)
    {
        yield return new WaitForSeconds(t);

        ChangeState(EnemyState.Chase);
    }

    /// <summary>
    /// UnityEvent�� Animation Event�� ���� �� �Լ�.
    /// �� Enemy�� ��� �޾Ƽ� ���
    /// </summary>
    virtual public void OnAttack()
    {

    }

    /// <summary>
    /// ���� ���� ����ϴ� �Լ�. Anim Event�� ����
    /// </summary>
    public void PlayAttackSound()
    {
        //���� ���� ���
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
    /// �װ� ���� ĳ���� ������ ������� ȿ��(Dissolve Shader ���)
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
                ////õõ�� ������� �ϱ� ���ؼ� x^2 �Լ� ���, 0 ~ 1
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
