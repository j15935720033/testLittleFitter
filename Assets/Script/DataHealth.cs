using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    [CreateAssetMenu(menuName = "chia /Data Health", fileName = "DataHealth")]
    public class DataHealth : ScriptableObject
    {
        [Header("��q")]
        public float hp = 100;
        [HideInInspector]//���ݭn���

        /*property
        public float hpMax
        {
            get { return hp;}
        }
        */

        /*��Landa²�Ƽg�k.1
        public float hpMax
        {
            get=>hp;
        }
        */

        //��Landa²�Ƽg�k.2
        public float hpMax => hp;//property�g�k
    }
}