using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SO
{


/*
    本 class 演示了如何在一个 ScriptableObject class 体内嵌套一个 "可序列化的 class";
    可在下面的 UseScriptableObject_A 类中看到本 class 的使用;
*/
[System.Serializable]
public class Node
{
    public string key;
    public float value;


    // 可被序列化的 class 内是可以写 函数的, 不影响它被序列化;
    // 本函数仅仅是把数据打印出来:
    public string GetString()
    {
        return "--: key: " + key + ", value: " + value;
    }
}




/*
    ScriptableObject 文件生成器:
    用法:
    
        ( 此脚本它不继承于 MonoBehaviour, 所以它是不能挂载到场景的 gameObject 身上的 )

        请在 Project 面板里, 在一个目录下鼠标右键, 选 "Create", 然后你会在右侧列表里看到一个名为 "新建一个 ScriptableObject 文件" 的按钮,
        点击它 就能新建一个 文件, 名为 "SO实例文件.asset"; 
        点击这个文件, 就能在 inspector 中看到它的所有数据, 这些数据和 本class 的内容是完全一致的;

        你可以在 inspector 中配置这个文件, 然后点击 ctl+s 来保存它;
        ---

        -1- 这个文件 ("SO实例文件.asset") 其实是个 字符串文件, 你可以在 txt编辑器里打开它, 看看它是怎么放置那些变量的;
        -2- 这个文件的名字可以被随意修改;


*/
[CreateAssetMenu(fileName = "SO实例文件", menuName = "新建一个 ScriptableObject 文件")]
public class UseScriptableObject : ScriptableObject
{

    // 如果一个变量想要被 序列化, 要么给它加一个 attribute:  [SerializeField], 要么把它设置为 public 的; (如下面的distance)
    [SerializeField] 
    int age;    // 演示如何序列化一个 int 变量

    public float distance; // 演示如何序列化一个 float 变量

    [SerializeField] 
    string selfName;    // 演示如何序列化一个 string


    [SerializeField] 
    Node someNode;  // 演示如何序列化一个 嵌套的 class实例


    [SerializeField] 
    List<Node> nodes = new List<Node>();   // 演示如何序列化一个 List



    // 可被序列化的 class 内是可以写 函数的, 不影响它被序列化;
    // 本函数仅仅是把数据打印出来:
    public string GetString()
    {
        string log = "age = " + age +
                    "\ndistance = " +  distance +
                    "\nselfName = " +  selfName +
                    "\nsomeNode = " +  someNode.GetString() +
                    "\nnodes: ";

        foreach( var e in nodes )
        {
            log += "\n" + e.GetString();
        }
        return log;
    }
} 


} 
