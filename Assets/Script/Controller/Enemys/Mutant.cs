using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mutant : EnemyBase
{
    public WeaponAttackController weapon;
    public override void Attack()
    {
        base.Attack();
        animator.SetTrigger("attack");

    }
    public void AttackBegin()
    {
        weapon.BeginAttack();
    }
    public void AttackEnd()
    {
        weapon.EndAttack();
    }

    public override void OnDeath(Damageable damageable, DamageMessage data)
    {
        base.OnDeath(damageable, data);
        //Destroy(gameObject);
        //丢失目标
        LoseTarget();
        //停止追踪
        meshAgent.isStopped = true;
        meshAgent.enabled = false;
        //播放死亡动画
        animator.SetTrigger("death");
        //添加一个力 让它飞出去

        //3s后销毁
        Destroy(gameObject, 3f);
    }
}
