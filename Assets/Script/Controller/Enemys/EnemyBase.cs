using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Damageable))]
public class EnemyBase : MonoBehaviour
{

    #region 字段
    public float checkDistance;//最大检测半径
    public float maxHeightDiff; //最大的高度差
    [Range(0, 180)]
    public float lookAngle;//视野范围

    RaycastHit[] results = new RaycastHit[10];
    public float followDistance;//追踪距离
    public float attackDistance;//攻击距离
    public LayerMask layerMask;
    public GameObject Target;
    protected NavMeshAgent meshAgent;
    protected Vector3 startPosition;
    public float runSpeed = 4;
    public float walkSpeed = 2;
    protected float moveSpeed = 0;
    protected Animator animator;
    // protected Rigidbody mRigidbody;
    // protected bool isCanAttack = true;
    // public float attackTime;//攻击时间间隔
    // private float attackTimer = 0;
    #endregion

    #region 生命周期
    protected virtual void Start()
    {
        meshAgent = transform.GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        animator = transform.GetComponent<Animator>();
        var damageable = transform.GetComponent<Damageable>();
        damageable.onHurt.AddListener(OnHurt);
        // mRigidbody = transform.GetComponent<Rigidbody>();
    }
    protected virtual void Update()
    {
        CheckTarget();
        FollowTarget();
        // if (isCanAttack)
        // {
        //     attackTimer += Time.deltaTime;
        //     if (attackTimer >= attackTime)
        //     {
        //         isCanAttack = true;
        //         attackTimer = 0;
        //     }
        // }
    }
    // private void OnAnimatorMove()
    // {
    //     mRigidbody.MovePosition(transform.position + animator.deltaPosition);
    // }
    protected virtual void OnDrawGizmosSelected()
    {
        //画出检查范围
        Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.4f);
        Gizmos.DrawWireSphere(transform.position, checkDistance);


        //画出高度差
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * maxHeightDiff);
        Gizmos.DrawLine(transform.position, transform.position - Vector3.up * maxHeightDiff);


        //画出视野范围
        UnityEditor.Handles.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0.4f);
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, lookAngle, checkDistance);
        UnityEditor.Handles.DrawSolidArc(transform.position, -Vector3.up, transform.forward, lookAngle, checkDistance);


        //画出追踪距离
        Gizmos.color = new Color(Color.gray.r, Color.gray.g, Color.gray.b, 0.4f);
        Gizmos.DrawWireSphere(transform.position, followDistance);


        //画出攻击距离
        Gizmos.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0.4f);
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
    #endregion
    #region 方法
    public virtual void CheckTarget()
    {
        int count = Physics.SphereCastNonAlloc(transform.position, checkDistance, Vector3.forward, results, 0, layerMask.value);

        for (int i = 0; i < count; i++)
        {
            //判断是不是可攻击的游戏物体
            if (results[i].transform.GetComponent<Damageable>() == null)
            {
                continue;
            }
            //判断高度差
            if (Mathf.Abs(results[i].transform.position.y - transform.position.y) > maxHeightDiff)
            {
                continue;
            }
            //判断是不是在视野范围内
            if (Vector3.Angle(transform.forward, results[i].transform.position - transform.position) > lookAngle)
            {
                continue;
            }
            //找到目标了 (找到离自己最近的目标攻击)
            if (Target != null)
            {
                //判断距离
                float distance = Vector3.Distance(transform.position, Target.transform.position);
                float currentDistance = Vector3.Distance(transform.position, results[i].transform.position);
                if (currentDistance < distance)
                {
                    Target = results[i].transform.gameObject;
                }
            }
            else
            {
                Target = results[i].transform.gameObject;
            }

        }
    }
    //向目标移动
    public virtual void MoveToTarget()
    {
        if (Target != null && meshAgent.enabled && meshAgent.isOnNavMesh)
        {
            meshAgent.SetDestination(Target.transform.position);
        }
    }
    //追踪目标
    public virtual void FollowTarget()
    {
        //监听速度
        ListenerSpeed();

        if (Target != null)
        {
            try
            {
                //向目标移动
                MoveToTarget();
                //判断路径是否有效
                if (meshAgent.pathStatus == NavMeshPathStatus.PathPartial || meshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                {
                    LoseTarget();
                    return;
                }
                //是否在可追踪的距离内
                if (Vector3.Distance(transform.position, Target.transform.position) > followDistance)
                {
                    //目标丢失
                    LoseTarget();
                    return;
                }
                //判断是不是在攻击范围内
                if (Vector3.Distance(transform.position, Target.transform.position) <= attackDistance)
                {
                    //Debug.Log("进行攻击");
                    // if (isCanAttack)
                    // {
                    Attack();
                    // isCanAttack = false;
                    // }

                }
            }
            catch (Exception e)
            {
                //追踪出错 目标丢失
                LoseTarget();
            }


        }
    }
    public virtual void LoseTarget()
    {
        Target = null;
        //回到初始位置
        if (meshAgent.enabled && meshAgent.isOnNavMesh) {
            meshAgent.SetDestination(startPosition);
        }
        moveSpeed = walkSpeed;
    }
    //监听速度
    public virtual void ListenerSpeed()
    {
        if (Target != null)
        {
            moveSpeed = runSpeed;
        }
        meshAgent.speed = moveSpeed;

        animator.SetFloat("speed", meshAgent.velocity.magnitude);

    }
    public virtual void Attack()
    {

    }
    public virtual void OnDeath(Damageable damageable, DamageMessage data)
    {

    }
    public virtual void OnHurt(Damageable damageable, DamageMessage data)
    {
        //Debug.Log("受伤");
        animator.SetTrigger("hurt");
    }
    #endregion
}
