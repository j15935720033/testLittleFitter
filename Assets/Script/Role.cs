using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Role : MonoBehaviour
{
    #region ���:�O�s�t�λݭn�����
    private int blood=500;
    private int mana=500;
   
    protected int Blood
    {
        get { return blood; }
        set { blood = value; }
    }//��q
    protected int Mana
    {
        get { return mana; }
        set { mana = value; }
    }//�]�O
    #endregion





    #region �\��:��@�Өt�Ϊ�������k
    abstract protected void Walk();
    abstract protected void Walk2();//��position����
    abstract protected  void Jump();//��position����
    abstract protected void JumpKey();
    abstract protected void JumpForce();//�׸��D&&�b�a�O�ɵ��V�W���O�q
    #endregion






    #region �ƥ�:�{���J�f
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
