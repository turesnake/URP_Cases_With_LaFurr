
Shader "TPR/Distort" {
    Properties {
        _texture ("texture", 2D) = "white" {}
        _intensity ("intensity", Range(0, 1)) = 0
        _u ("u", Float ) = 0
        _v ("v", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [Enum(Off,0,On,1)]__ZWrite("ZWrite",Float) = 0
    }
    SubShader {

        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
       

        Pass {
            Tags{ "LightMode" = "GrabTexture" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite [__ZWrite]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            uniform sampler2D _GrabTexture;
            uniform sampler2D _texture; uniform float4 _texture_ST;
            uniform float _intensity;
            uniform float _u;
            uniform float _v;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 time0 = _Time;
                float2 uv_a = (float2((_u*time0.g),(time0.g*_v))+i.uv0);

                float4 _texture_var = tex2D(_texture,TRANSFORM_TEX(uv_a, _texture));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (float2(_texture_var.r,_texture_var.g)*_texture_var.a*i.vertexColor.a*_intensity);
                float4 sceneColor = tex2D(_GrabTexture, saturate(sceneUVs));
                fixed4 finalRGBA = fixed4(sceneColor.rgb, 1);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack Off
}
