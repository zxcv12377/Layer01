using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("test", 5f);
        test1();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void test()
    {
        print("처음");
    }
    
    void test1()
    {
        print("마지막");
    }
}
