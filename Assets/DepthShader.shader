Shader "Unlit/DepthShader" { //深度渲染器，用于赋予图像中每一个像素深度值
    SubShader{
        // 设置渲染标签，表示这个shader是用于不透明的物体
        Tags { "RenderType" = "Opaque" }

        // 定义Pass部分，一个shader可以包含多个Pass
        Pass{
            // 开始CG程序段，CG是C for Graphics，一个由NVIDIA开发的高级图形渲染语言
            CGPROGRAM
            // 指定vertex shader函数为"vert"
            #pragma vertex vert  
            // 指定fragment shader函数为"frag"
            #pragma fragment frag  
            // 指定fragment shader函数为"frag"
            #include "UnityCG.cginc"  
                
            // 定义相机深度纹理的采样器
            sampler2D _CameraDepthTexture;
            
            // 定义vertex到fragment的数据结构
            struct v2f {
               // SV_POSITION是一个语义，表示这是顶点的屏幕位置
               float4 pos : SV_POSITION;
               // TEXCOORD1表示这是一个纹理坐标
               float4 scrPos:TEXCOORD1;
            };

            // Vertex Shader函数
            v2f vert(appdata_base v) {
               v2f o;
               // 将物体空间的顶点位置转换为裁剪空间的位置
               o.pos = UnityObjectToClipPos(v.vertex);
               // 计算屏幕位置
               o.scrPos = ComputeScreenPos(o.pos);
               return o;
            }

            // Fragment Shader函数
            half4 frag(v2f i) : COLOR{
               // 从_CameraDepthTexture纹理中采样深度值，然后将其转换为线性视觉深度
               float depthValue = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(i.scrPos)).r);
               // 返回深度值作为RGB三通道的值，透明度为1
               return half4(depthValue,depthValue,depthValue,1);
            }
            // 结束CG程序段
            ENDCG
        }
    }
        // 如果当前的渲染硬件不支持这个shader，那么使用Diffuse作为备选方案
        FallBack "Diffuse"
}
