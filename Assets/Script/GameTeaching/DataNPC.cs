using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    /// <summary>
    /// NPC���:�W�١B��ܤ��e�P����
    /// ScriptableObject:�}���ƪ���(�N�{�����e�x�s�������bProject��)
    /// </summary>
    [CreateAssetMenu(menuName ="chia/Data NPC",fileName ="DataNPC")]
    public class DataNPC : ScriptableObject
    {
        [Header("DataNPC�W��")]
        public string nameNPC;

        //NonReorderable:���n�ƦC�A�ѨM�}�C�b�ݩʭ��O��ܪ�BUG
        [Header("��ܤ��e�B���� �}�C"), NonReorderable]
        public DataDialogue[] dataDialoge;
        //�غc�l��l��
        DataNPC()
        {
            dataDialoge = new DataDialogue[7];
            //�}�C�̳��O��DataDialogue�A�nnew�X����~��]��==>dataDialoge[0].content = ""
            for (int i=0;i< dataDialoge.Length;i++)
            {
                dataDialoge[i] = new DataDialogue();
            }
            dataDialoge[0].content = "�תť���άO�ƹ������~���鷺�e";
            dataDialoge[1].content = "��Ctrl+�k��+Enter,��X�C��";
            dataDialoge[2].content = "��Ctrl+�U��+Enter,��X������";
            dataDialoge[3].content = "��Eneter,����";
        }
    }

    [System.Serializable]
    public class DataDialogue
    {
        [Header("��ܤ��e")]
        public string content;
        [Header("��ܭ���")]
        public AudioClip sound;
    }
}
