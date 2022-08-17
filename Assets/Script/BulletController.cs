using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField,Header("子彈速度")]
    public float speedBullet=10f;
    [SerializeField, Header("子彈飛行時間")]
    public float timer = 1;
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(20,0),ForceMode2D.Impulse);//子彈移動_AddForce
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += new Vector3(speedBullet* Time.deltaTime, 0,0);//子彈移動_transform
        //print($"Time.deltaTime={Time.deltaTime}");
        timer -= Time.deltaTime;//每次遞減時間
        
        //飛行時間到，銷毀子彈物件
        if (timer<=0)
        {
            Destroy(this.gameObject);
        }
    }

}
