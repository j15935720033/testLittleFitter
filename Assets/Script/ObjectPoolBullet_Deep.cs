using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace chia
{
    public class ObjectPoolBullet_Deep : MonoBehaviour
    {
        [SerializeField, Header("�l�u")]
        private GameObject prefabBullet;
        /// <summary>
        /// �l�u�����
        /// </summary>
        private ObjectPool<GameObject> poolBullet;
        int count;//�ƶq
        AttackSystem attackSystem;




        private void Awake()
        {
            //��O�ƪ����=�غc�l(�إߡB������B�٪���B�W�X�ɳB�z�B�O�_�ݿ�X�T���B�e�q
            poolBullet = new ObjectPool<GameObject>(
                CreatePool, GetBullet, ReleaseBullet, DestroyBullet, false, 100
                );
            attackSystem = prefabBullet.GetComponent<AttackSystem>();
        }
        /// <summary>
        /// �إߪ�����n�B�z���欰
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
        /// �򪫥��������
        /// </summary>
        /// <param name="bullet"></param>
        private void GetBullet(GameObject bullet)
        {
            bullet.SetActive(true);
            if (bullet!=null)
            {
                bullet.GetComponent<BulletController>().timer = 5;//��q�����������ɡA��timer���]��5
            }
            
        }
        /// <summary>
        /// �⪫���ٵ������
        /// </summary>
        /// <param name="ball"></param>
        private void ReleaseBullet(GameObject bullet)
        {
            bullet.SetActive(false);
        }
        /// <summary>
        /// �ƶq�W�X������e�q�n�����B�z
        /// </summary>
        private void DestroyBullet(GameObject bullet)
        {
            Destroy(bullet);
        }
        /// <summary>
        /// ���o�����������
        /// </summary>
        public GameObject GetPoolObject(WhoAttack whoAttack)
        {
            attackSystem.dataAttack.whoAttack = whoAttack;//�֪�����
            return poolBullet.Get();
        }
        public void ReleasePoolObject(GameObject bullet)
        {
            poolBullet.Release(bullet);
        }
    }
}