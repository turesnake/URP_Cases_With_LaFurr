using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;




public class ParticalDistortPass : ScriptableRendererFeature
{


    /*
        Pass -1-:
            在 "AfterRenderingTransparents" 阶段, 把屏幕color 写入 texture: "_GrabTexture", 已供后续 pass 使用
    */
    public class GrabTextureRenderPass : ScriptableRenderPass
    {
        RenderTargetIdentifier src { get; set; }
        ScriptableRenderer srcRenderer; 
        RenderTargetHandle m_tmpColorTexture;  // 临时 rt，做 blit 数据中转站

        public GrabTextureRenderPass()
        {
            this.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            m_tmpColorTexture.Init("tmpColorTexture");
        }

        public void Setup( ref ScriptableRenderer srcRenderer_ )
        {
            this.srcRenderer = srcRenderer_;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            this.src = this.srcRenderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get( "GrabTexture" );
            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0; // 不生成 z-buffer
            cmd.GetTemporaryRT( m_tmpColorTexture.id, opaqueDesc, FilterMode.Point );

            //---
            Blit(cmd, src, m_tmpColorTexture.Identifier());
            cmd.SetGlobalTexture("_GrabTexture", srcRenderer.cameraColorTarget ); // 设置全局 texture, 供后续的 DistortRenderPass 访问
        
            //---
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup( CommandBuffer cmd )
        {   
            cmd.ReleaseTemporaryRT(m_tmpColorTexture.id); // 释放 tmprt
        }
    }




    /*
         Pass -2-:
            拿到 pass -1- 中准备好的 "_GrabTexture", 提供给 Distort.shader 用,
            同时通过 LightMode, 控制 Distort.shader 的 pass 的执行时机, 将它放在 "AfterRenderingPostProcessing" 时刻执行;
    */
    class DistortRenderPass : ScriptableRenderPass
    {   
        ShaderTagId passId = new ShaderTagId("GrabTexture");
        FilteringSettings FilteringSettings = new FilteringSettings( new RenderQueueRange(1000, 5000) );

        public DistortRenderPass()
        {
            this.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }
    
        public override void Execute( ScriptableRenderContext context, ref RenderingData renderingData )
        {
            DrawingSettings DrawingSettings = CreateDrawingSettings(passId, ref renderingData, SortingCriteria.BackToFront );
            context.DrawRenderers(renderingData.cullResults, ref DrawingSettings, ref FilteringSettings);
        }
    }

    
    // ============================================================================================== //

    DistortRenderPass m_DistortRenderPass;
    GrabTextureRenderPass m_GrabTextureRenderPass;

    public override void Create()
    {
        m_GrabTextureRenderPass = new GrabTextureRenderPass();
        m_DistortRenderPass     = new DistortRenderPass();
    }


    public override void AddRenderPasses( ScriptableRenderer renderer, ref RenderingData renderingData )
    {           

        // 只渲染 Game 窗口:
        // !!! 重要 !!!, 否则当 unity 同时显示 Game 和 Scene 两个窗口时, 会出现渲染异常;
        if( renderingData.cameraData.cameraType != CameraType.Game )
        {
            return;
        }
        
        // tpr:
        //    只有 stack 中的最后一个 camera 可以执行此 render pass
        //    若不做此限制, 那么这个 render pass 会作用于 参数 renderer 体内的每一个 camera 上;
        //    (其它更多 筛选 camera 的方法 还有待探索..)
        if( !renderingData.cameraData.resolveFinalTarget  ){
            return;
        }

        // pass 1:
        m_GrabTextureRenderPass.Setup( ref renderer );
        renderer.EnqueuePass(m_GrabTextureRenderPass);

        // pass 2:
        renderer.EnqueuePass(m_DistortRenderPass);
    }
}


