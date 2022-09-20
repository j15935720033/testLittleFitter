using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region 屬性
    [SerializeField, Header("子彈速度")]
    public float speedBullet = 10f;
    [SerializeField, Header("子彈飛行時間")]
    public float timer = 10;
    [SerializeField, Header("誰的子彈")]
    private BulletKind bulletKind;
    private int direction;//1:向右 -1:向左。接資料庫的值
    #endregion


    private void Awake()
    {
        direction = PlayerPrefs.GetInt("Deep_破空斬方向");
    }
    // Update is called once per frame
    void Update()
    {
        MoveDeepBullet();
       
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*switch (bulletKind)
        {
            case BulletKind.DeepBall:
                if (collision.gameObject.tag == "Enemy")
                {
                    //print("deep ball 打到敵人");
                }
                break;
            default:
                break;
        }*/
    }

    #region 自訂方法
    /// <summary>
    /// Deep破空斬方向
    /// </summary>
    private void MoveDeepBullet()
    {
        if (direction == 1)//子彈向右
        {
            this.gameObject.transform.position += new Vector3(speedBullet * Time.deltaTime, 0, 0);//子彈移動_transform
        }
        else if (direction == -1)//子彈向左
        {
            this.gameObject.transform.position += new Vector3(-speedBullet * Time.deltaTime, 0, 0);//子彈移動_transform
        }

        timer -= Time.deltaTime;//每次遞減時間

        //飛行時間到，銷毀子彈物件
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
