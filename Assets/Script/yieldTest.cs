using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yieldTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        //StartCoroutine(Wait10());//�Ұʰ��P�{��
    }
    // Update is called once per frame
    void Update()
    {
        print("Update Start");
        StartCoroutine(Wait10());//�Ұʰ��P�{��
        print("Update finish");
    }
   
    private IEnumerator Wait10()
    {
        print("wait10 start");
        yield return new WaitForSeconds(10);
        print("wait10 finish");
    }
    /*private IEnumerator Wait20()
    {
        print("wait20 start");
        yield return new WaitForSeconds(20);
        print("wait20 finish");
    }*/
}
