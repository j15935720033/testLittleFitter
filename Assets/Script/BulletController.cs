using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField,Header("�l�u�t��")]
    public float speedBullet=10f;
    [SerializeField, Header("�l�u����ɶ�")]
    public float timer = 1;
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(20,0),ForceMode2D.Impulse);//�l�u����_AddForce
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += new Vector3(speedBullet* Time.deltaTime, 0,0);//�l�u����_transform
        //print($"Time.deltaTime={Time.deltaTime}");
        timer -= Time.deltaTime;//�C������ɶ�
        
        //����ɶ���A�P���l�u����
        if (timer<=0)
        {
            Destroy(this.gameObject);
        }
    }

}
