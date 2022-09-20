using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    #region �ݩ�
    [SerializeField, Header("�l�u�t��")]
    public float speedBullet = 10f;
    [SerializeField, Header("�l�u����ɶ�")]
    public float timer = 10;
    [SerializeField, Header("�֪��l�u")]
    private BulletKind bulletKind;
    private int direction;//1:�V�k -1:�V���C����Ʈw����
    #endregion


    private void Awake()
    {
        direction = PlayerPrefs.GetInt("Deep_�}�ű٤�V");
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
                    //print("deep ball ����ĤH");
                }
                break;
            default:
                break;
        }*/
    }

    #region �ۭq��k
    /// <summary>
    /// Deep�}�ű٤�V
    /// </summary>
    private void MoveDeepBullet()
    {
        if (direction == 1)//�l�u�V�k
        {
            this.gameObject.transform.position += new Vector3(speedBullet * Time.deltaTime, 0, 0);//�l�u����_transform
        }
        else if (direction == -1)//�l�u�V��
        {
            this.gameObject.transform.position += new Vector3(-speedBullet * Time.deltaTime, 0, 0);//�l�u����_transform
        }

        timer -= Time.deltaTime;//�C������ɶ�

        //����ɶ���A�P���l�u����
        if (timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
