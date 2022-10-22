using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    [CreateAssetMenu(menuName = "chia/Data Attack", fileName = "DataAttack")]
    public class DataAttack : ScriptableObject
    {

        [Header("����")]
        public float attack = 30;
        [Header("������")]
        public float skill01 = 50;
        [Header("�l�u����")]
        public float skill02 = 60;
        [Header("�]�B����")]
        public float skill03 = 40;

        [SerializeField, Header("��������")]
        public AttackKind attackKind;
        [SerializeField, Header("�֧���")]
        public WhoAttack whoAttack;
    }
}