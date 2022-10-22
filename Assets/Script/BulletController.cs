using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    public class BulletController : MonoBehaviour
    {
        #region 屬性
        [SerializeField, Header("子彈速度")]
        public float speedBullet = 30f;
        [SerializeField, Header("子彈飛行時間")]
        public float timer = 5;
        public int direction;//1:向右 -1:向左。接資料庫的值

        #endregion


        protected virtual void Awake()
        {
            //direction = PlayerPrefs.GetInt("Deep_破空斬方向");
            
        }
        // Update is called once per frame
        void Update()
        {
            MoveDeepBullet();

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
        }
        #endregion
    }
}