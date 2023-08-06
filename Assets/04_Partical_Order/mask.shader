
Shader "TPR/blend_mask" {
    Properties {
        _Brightness ("Brightness", Float ) = 1
        _Contrast ("Contrast", Float ) = 1
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Main Tex", 2D) = "white" {}
        _MaskTex ("Mask Tex", 2D) = "white" {}
        _MainPannerX ("Main Panner X", Float ) = 0
        _MainPannerY ("Main Panner Y", Float ) = 0
        _SecPannerX ("Sec Panner X", Float ) = 0
        _SecPannerY ("Sec Panner Y", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [Enum(Off,0,On,1)]__ZWrite("ZWrite",Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="KOKO"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite [__ZWrite]
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
           // #pragma multi_compile_fwdbase
            //#pragma only_renderers d3d9 d3d11 glcore gles
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            
            #pragma target 2.0
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Brightness;
            uniform float _MainPannerX;
            uniform float _MainPannerY;
            uniform float4 _MainColor;
            uniform float _SecPannerX;
            uniform float _SecPannerY;
            uniform float _Contrast;
            uniform sampler2D _MaskTex; uniform float4 _MaskTex_ST;

            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                half4  mask : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );

                float2 pixelSize = o.pos.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

            	float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
            	
            	o.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float alpha=1.0;

            	#ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                alpha *= m.x * m.y;
                #endif
                
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );


                float4 node_6409 = _Time;
                float2 node_514 = (i.uv0+(float2(_MainPannerX,_MainPannerY)*node_6409.g));
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_514, _MainTex));
                float4 node_482 = _Time;
                float2 node_5906 = (i.uv0+(float2(_SecPannerX,_SecPannerY)*node_482.g));
                float4 _MaskTex_var = tex2D(_MaskTex,TRANSFORM_TEX(node_5906, _MaskTex));

                float brightness_ = max( 0.0, abs(_Brightness) );
                float contrast_ = max( 0.001, abs(_Contrast) );
                float3 emissive = (_MainColor.rgb*brightness_*pow(_MainTex_var.rgb,contrast_)*i.vertexColor.rgb*_MaskTex_var.rgb);
                float3 finalColor = emissive;
                return fixed4(finalColor, saturate(_MainColor.a*_MainTex_var.a*_MaskTex_var.r*i.vertexColor.a*alpha));
            }
            ENDCG
        }
    }
    FallBack off
}
