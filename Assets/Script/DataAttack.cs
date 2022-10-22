using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    [CreateAssetMenu(menuName = "chia/Data Attack", fileName = "DataAttack")]
    public class DataAttack : ScriptableObject
    {

        [Header("´¶§ð")]
        public float attack = 30;
        [Header("¨¾¡õ§ð")]
        public float skill01 = 50;
        [Header("¤l¼u§ðÀ»")]
        public float skill02 = 60;
        [Header("¶]¨B¸õ¬å")]
        public float skill03 = 40;

        [SerializeField, Header("§ðÀ»ºØÃþ")]
        public AttackKind attackKind;
        [SerializeField, Header("½Ö§ðÀ»")]
        public WhoAttack whoAttack;
    }
}