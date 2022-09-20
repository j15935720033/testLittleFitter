using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace chia
{
    /// <summary>
    /// NPC資料:名稱、對話內容與音效
    /// ScriptableObject:腳本化物件(將程式內容儲存為物件放在Project內)
    /// </summary>
    [CreateAssetMenu(menuName ="chia/Data NPC",fileName ="DataNPC")]
    public class DataNPC : ScriptableObject
    {
        [Header("DataNPC名稱")]
        public string nameNPC;

        //NonReorderable:不要排列，解決陣列在屬性面板顯示的BUG
        [Header("對話內容、音效 陣列"), NonReorderable]
        public DataDialogue[] dataDialoge;
        //建構子初始化
        DataNPC()
        {
            dataDialoge = new DataDialogue[7];
            //陣列裡都是放DataDialogue，要new出物件才能設值==>dataDialoge[0].content = ""
            for (int i=0;i< dataDialoge.Length;i++)
            {
                dataDialoge[i] = new DataDialogue();
            }
            dataDialoge[0].content = "案空白鍵或是滑鼠左鍵繼續對對內容";
            dataDialoge[1].content = "按Ctrl+右鍵+Enter,放出劍氣";
            dataDialoge[2].content = "按Ctrl+下鍵+Enter,放出鬼哭斬";
            dataDialoge[3].content = "按Eneter,普攻";
        }
    }

    [System.Serializable]
    public class DataDialogue
    {
        [Header("對話內容")]
        public string content;
        [Header("對話音效")]
        public AudioClip sound;
    }
}
