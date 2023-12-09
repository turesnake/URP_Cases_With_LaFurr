using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SO
{


/*
    演示如何在一个常规的 MonoBehaviour 脚本中, 使用一个 so文件的内容;
    此处仅仅是打印了它的数据, 但既然可以打印它们, 自然也能把它们写入别的代码中; 以此实现 "读取数据"->"使用数据" 的目的;
*/
public class ReadScriptableObject : MonoBehaviour
{
    public UseScriptableObject scriptableObject_A;

    void Start()
    {
        if( scriptableObject_A == null )
        {
            Debug.LogError("您需要先绑定 scriptableObject_A, 再运行场景");
            return;
        }
        // 打印这个 scriptableObject_A 所指文件的内容:
        string log = scriptableObject_A.GetString();
        Debug.Log( "SO文件内容: \n" + log );
    }
}


}
