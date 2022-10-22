using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    public class BulletController : MonoBehaviour
    {
        #region �ݩ�
        [SerializeField, Header("�l�u�t��")]
        public float speedBullet = 30f;
        [SerializeField, Header("�l�u����ɶ�")]
        public float timer = 5;
        public int direction;//1:�V�k -1:�V���C����Ʈw����

        #endregion


        protected virtual void Awake()
        {
            //direction = PlayerPrefs.GetInt("Deep_�}�ű٤�V");
            
        }
        // Update is called once per frame
        void Update()
        {
            MoveDeepBullet();

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
        }
        #endregion
    }
}