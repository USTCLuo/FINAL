using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CheckPoint
{
    public Transform point;
    public float radius;
}
public class WeaponAttackController : MonoBehaviour
{

    #region 字段
    public CheckPoint[] checkPoint;
    public Color color;

    private RaycastHit[] results = new RaycastHit[10];

    public LayerMask layerMask;
    // Start is called before the first frame update
    private bool Attack = false;
    public int damage;
    public GameObject myself;
    private List<GameObject> attackList = new List<GameObject>();

    #endregion

    #region Unity生命周期
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckGameObject();
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < checkPoint.Length; i++)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(checkPoint[i].point.position, checkPoint[i].radius);
        }
    }
    #endregion

    #region 方法
    public void BeginAttack()
    {
        Attack = true;
    }
    public void EndAttack()
    {
        Attack = false;
        attackList.Clear();
    }
    public void CheckGameObject()
    {
        if (!Attack) { return; }
        for (int i = 0; i < checkPoint.Length; i++)
        {
            int count = Physics.SphereCastNonAlloc(checkPoint[i].point.position, checkPoint[i].radius, Vector3.forward, results, 0, layerMask.value);
            for (int j = 0; j < count; j++)
            {
                //Debug.Log("检测到敌人 进行攻击:" + results[j].transform.name);
                CheckDamage(results[j].transform.gameObject);
            }
        }

    }
    //对敌人造成伤害
    public void CheckDamage(GameObject obj)
    {
        // 判断游戏物体是不是有受伤功能
        Damageable damageable = obj.GetComponent<Damageable>();
        if (damageable == null)
        {
            return;
        }
        //有可能检测到自己
        if (obj == myself)
        {
            return;
        }
        if (attackList.Contains(obj))
        {
            return;
        }

        //进行攻击
        DamageMessage data = new DamageMessage();
        data.damage = damage;
        data.damagePosition = myself.transform.position;
        damageable.OnDamage(data);
        //Debug.Log("攻击" + obj.name);

        attackList.Add(obj);
    }
    #endregion
    
}
