using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Role : MonoBehaviour
{
    #region 資料:保存系統需要的資料
    private int blood=500;
    private int mana=500;
   
    protected int Blood
    {
        get { return blood; }
        set { blood = value; }
    }//血量
    protected int Mana
    {
        get { return mana; }
        set { mana = value; }
    }//魔力
    #endregion





    #region 功能:實作該系統的複雜方法
    abstract protected void Walk();
    abstract protected void Walk2();//用position控制
    abstract protected  void Jump();//用position控制
    abstract protected void JumpKey();
    abstract protected void JumpForce();//案跳躍&&在地板時給向上的力量
    #endregion






    #region 事件:程式入口
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    

    #endregion

}
