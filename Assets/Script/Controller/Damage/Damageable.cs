using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class DamageMessage
{
    public int damage;
    public Vector3 damagePosition;
}
[Serializable]
public class DamageEvent:UnityEvent<Damageable,DamageMessage> {}
public class Damageable : MonoBehaviour
{
    #region 字段
    public int hp; // 当前血量
    public int maxHp;// 最大血量
    public float invincibleTime = 0; // 无敌时间
    private bool isInvincible = false; // 是否无敌
    private float invincibleTimer = 0;
    public DamageEvent onHurt;
    public DamageEvent onDeath;
    public DamageEvent onReset;
    #endregion
    #region 生命周期
    private void Start()
    {
        hp = maxHp;
    }
    void Update()
    {
        if (isInvincible)
        {
            invincibleTimer += Time.deltaTime;
            if (invincibleTimer >= invincibleTime)
            {
                isInvincible = false;
                invincibleTimer = 0;
            }
        }
    }

    #endregion

    #region 方法
    public void OnDamage(DamageMessage data)
    {
        if (hp <= 0)
        {
            onDeath?.Invoke(this, data);
            return;
        }
        if (isInvincible)
        {
            return;
        }
        hp -= data.damage;
        isInvincible = true;
        if (hp <= 0)
        {
            // 死了
            onDeath?.Invoke(this, data);
            Debug.Log("死亡"+gameObject.name);
        }
        else
        {
            //受伤
            onHurt?.Invoke(this, data);
        }
    }



    public void ResetDamage()
    {
        hp = maxHp;
        isInvincible = false;
        invincibleTimer = 0;
        onReset?.Invoke(this, null);
    }
    #endregion

}
