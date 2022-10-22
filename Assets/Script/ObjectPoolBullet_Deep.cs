using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace chia
{
    public class ObjectPoolBullet_Deep : MonoBehaviour
    {
        [SerializeField, Header("子彈")]
        private GameObject prefabBullet;
        /// <summary>
        /// 子彈物件池
        /// </summary>
        private ObjectPool<GameObject> poolBullet;
        int count;//數量
        AttackSystem attackSystem;




        private void Awake()
        {
            //實力化物件池=建構子(建立、拿物件、還物件、超出時處理、是否看輸出訊息、容量
            poolBullet = new ObjectPool<GameObject>(
                CreatePool, GetBullet, ReleaseBullet, DestroyBullet, false, 100
                );
            attackSystem = prefabBullet.GetComponent<AttackSystem>();
        }
        /// <summary>
        /// 建立物件池要處理的行為
        /// </summary>
        /// <returns></returns>
        private GameObject CreatePool()
        {
            count++;
            GameObject temp = Instantiate(prefabBullet);
            temp.name = prefabBullet.name + " " + count;
            return temp;
        }
        /// <summary>
        /// 跟物件池拿物件
        /// </summary>
        /// <param name="bullet"></param>
        private void GetBullet(GameObject bullet)
        {
            bullet.SetActive(true);
            if (bullet!=null)
            {
                bullet.GetComponent<BulletController>().timer = 5;//當從物件池拿物件時，把timer重設為5
            }
            
        }
        /// <summary>
        /// 把物件還給物件池
        /// </summary>
        /// <param name="ball"></param>
        private void ReleaseBullet(GameObject bullet)
        {
            bullet.SetActive(false);
        }
        /// <summary>
        /// 數量超出物件池容量要做的處理
        /// </summary>
        private void DestroyBullet(GameObject bullet)
        {
            Destroy(bullet);
        }
        /// <summary>
        /// 取得物件池的物件
        /// </summary>
        public GameObject GetPoolObject(WhoAttack whoAttack)
        {
            attackSystem.dataAttack.whoAttack = whoAttack;//誰的攻擊
            return poolBullet.Get();
        }
        public void ReleasePoolObject(GameObject bullet)
        {
            poolBullet.Release(bullet);
        }
    }
}