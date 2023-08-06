using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/*
    目前实现的功能:
        捕捉场景中一个 LightMode 值为 "KOKO" 的 shader pass, 并指定它在 渲染队列中的位置;
*/

public class ParticalPass : ScriptableRendererFeature
{

    class CustomRenderPass : ScriptableRenderPass
    {   
        
        // 将目标 shader pass 的 LightMode 设置为  "KOKO", 然后它就能被本 passId 捕捉到了;
        ShaderTagId passId = new ShaderTagId("KOKO");


        //    DrawRenderers() 的参数, shader 过滤器, 就算你某个材质球的 shader 里拥有 LightMode == "KOKO" 的 shader pass, 
        //    只要它的 renderQueueRange 不在 [1000,5000] 区间, 也会被过滤掉;
        //    在这里我们暂时用不到这个 过滤器, 所以给它设置的区间很宽泛
        FilteringSettings FilteringSettings = new FilteringSettings( new RenderQueueRange(1000, 5000) );


        public CustomRenderPass( RenderPassEvent event_ )
        {
            this.renderPassEvent = event_;
        }

    
        //    可在本函数体内编写: 渲染逻辑本身, 也就是 用户希望本 render pass 要做的那些工作;
        //    使用参数 context 来发送 绘制指令, 执行 commandbuffers;
        //    不需要在本函数实现体内 调用 "ScriptableRenderContext.submit()", 渲染管线会在何时的时间点自动调用它;
        public override void Execute( ScriptableRenderContext context, ref RenderingData renderingData )
        {
            // 一个渲染 "KOKO" shader pass 的配置文件, 它 记录并传递以下信息:
            // -1- how to sort visible objects (sortingSettings)  和渲染之前的 物体排序 有关
            // -2- which shader passes to use (shaderPassName).   在这里就是 LightMode 为 "KOKO" 的 shader pass
            DrawingSettings DrawingSettings = CreateDrawingSettings(passId, ref renderingData, SortingCriteria.CommonOpaque );

            // 渲染指定的一组 可见物体, 同时还向本类体内的 command list 中添加一系列 commands;
            // 这些 commands 最后会在 ScriptableRenderContext.Submit() 调用中, 被全部执行;
            // (其实它的功能很强大, 比如上面提到的 commands,  不过这里我暂时不展开了...)
            context.DrawRenderers(renderingData.cullResults, ref DrawingSettings, ref FilteringSettings);
        }
    }



    [System.Serializable]
    public class HLSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }


    CustomRenderPass m_ScriptablePass;
    // 命名为 settings，可以直接显示在 inspector 中
    public HLSettings settings = new HLSettings();


    // 仅在代码改变后，或者程序启动时，此函数才会被调用一次
    // Initializes this feature's resources. This is called every time serialization happens.
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass( settings.renderPassEvent );
    }

    /*
        ---------------------------------------------------------- +++
        Here you can inject one or multiple render passes in the renderer.
        This method is called when setting up the renderer once per-camera.
        ---
        在派生类的实现体中:
            可将 数个 "ScriptableRenderPass" 注入到本 feature 中;
            代码:
                renderer.EnqueuePass( m_pass );
                此处
                "renderer" 就是本函数提供的的参数;
                m_pass 就是一个 "ScriptableRenderPass" 或其继承者的 实例;

        参数:
        renderer:
            如 "ForwardRenderer"
        renderingData:
            Rendering state. Use this to setup render passes.
    */
    public override void AddRenderPasses( ScriptableRenderer renderer, ref RenderingData renderingData )
    {           

        /*       
            tpr:
                只有 stack 中的最后一个 camera 可以执行此 render pass
                若不做此限制, 那么这个 render pass 会作用于 参数 renderer 体内的每一个 camera 上;
                (其它更多 筛选 camera 的方法 还有待探索..)
        */
        if( !renderingData.cameraData.resolveFinalTarget  ){
            return;
        }
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


