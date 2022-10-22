using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    [CreateAssetMenu(menuName = "chia /Data Health", fileName = "DataHealth")]
    public class DataHealth : ScriptableObject
    {
        [Header("血量")]
        public float hp = 100;
        [HideInInspector]//不需要顯示

        /*property
        public float hpMax
        {
            get { return hp;}
        }
        */

        /*用Landa簡化寫法.1
        public float hpMax
        {
            get=>hp;
        }
        */

        //用Landa簡化寫法.2
        public float hpMax => hp;//property寫法
    }
}