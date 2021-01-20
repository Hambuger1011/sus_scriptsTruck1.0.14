// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.30 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.30;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:14,ufog:False,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1168,x:33215,y:32762,varname:node_1168,prsc:2|custl-9026-OUT;n:type:ShaderForge.SFN_Tex2d,id:9588,x:32257,y:32815,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_9588,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-6161-OUT;n:type:ShaderForge.SFN_TexCoord,id:7960,x:31084,y:32645,varname:node_7960,prsc:2,uv:1;n:type:ShaderForge.SFN_Time,id:3610,x:31217,y:33110,varname:node_3610,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3255,x:31559,y:33214,varname:node_3255,prsc:2|A-3610-T,B-6647-OUT;n:type:ShaderForge.SFN_Append,id:6647,x:31401,y:33330,varname:node_6647,prsc:2|A-3960-OUT,B-412-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3960,x:31172,y:33309,ptovrint:False,ptlb:Speed_U,ptin:_Speed_U,varname:node_3960,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:412,x:31161,y:33421,ptovrint:False,ptlb:Speed_V,ptin:_Speed_V,varname:node_412,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:8912,x:31788,y:32917,varname:node_8912,prsc:2|A-3014-OUT,B-3255-OUT;n:type:ShaderForge.SFN_Color,id:1140,x:32077,y:32988,ptovrint:False,ptlb:color,ptin:_color,varname:node_1140,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9339,x:32790,y:32856,varname:node_9339,prsc:2|A-4084-OUT,B-1140-RGB,C-1140-A,D-5235-OUT,E-9060-A;n:type:ShaderForge.SFN_Slider,id:5235,x:31976,y:33183,ptovrint:False,ptlb:Intensity,ptin:_Intensity,varname:node_5235,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_VertexColor,id:9060,x:32145,y:33320,varname:node_9060,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9026,x:32987,y:32944,varname:node_9026,prsc:2|A-9339-OUT,B-9878-RGB;n:type:ShaderForge.SFN_Tex2d,id:9878,x:32769,y:33470,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_9878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-7899-OUT;n:type:ShaderForge.SFN_Multiply,id:4084,x:32558,y:32756,varname:node_4084,prsc:2|A-9588-RGB,B-9588-A;n:type:ShaderForge.SFN_SwitchProperty,id:3014,x:31436,y:32787,ptovrint:False,ptlb:UV,ptin:_UV,varname:node_3014,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-7960-UVOUT,B-2221-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:2221,x:31080,y:32821,varname:node_2221,prsc:2,uv:0;n:type:ShaderForge.SFN_TexCoord,id:5897,x:32219,y:33451,varname:node_5897,prsc:2,uv:0;n:type:ShaderForge.SFN_SwitchProperty,id:7899,x:32480,y:33579,ptovrint:False,ptlb:mask_UV,ptin:_mask_UV,varname:_UV_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-5897-UVOUT,B-555-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:555,x:32167,y:33607,varname:node_555,prsc:2,uv:1;n:type:ShaderForge.SFN_Rotator,id:2698,x:31677,y:32455,varname:node_2698,prsc:2|UVIN-3014-OUT,SPD-3328-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:6161,x:31986,y:32599,ptovrint:False,ptlb:Rotation,ptin:_Rotation,varname:node_6161,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:False|A-8912-OUT,B-2698-UVOUT;n:type:ShaderForge.SFN_Slider,id:3328,x:31257,y:32419,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_3328,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-5,cur:0.5,max:5;proporder:1140-9588-412-3960-5235-9878-3014-7899-6161-3328;pass:END;sub:END;*/

Shader "LD/LD_uv_add" {
    Properties {
        [HDR]_color ("color", Color) = (0.5,0.5,0.5,1)
        _Texture ("Texture", 2D) = "white" {}
        _Speed_V ("Speed_V", Float ) = 0
        _Speed_U ("Speed_U", Float ) = 1
        _Intensity ("Intensity", Range(0, 5)) = 0
        _mask ("mask", 2D) = "white" {}
        [MaterialToggle] _UV ("UV", Float ) = 0
        [MaterialToggle] _mask_UV ("mask_UV", Float ) = 0
        [MaterialToggle] _Rotation ("Rotation", Float ) = 0
        _Speed ("Speed", Range(-5, 5)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Speed_U;
            uniform float _Speed_V;
            uniform float4 _color;
            uniform float _Intensity;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform fixed _UV;
            uniform fixed _mask_UV;
            uniform fixed _Rotation;
            uniform float _Speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                float2 _UV_var = lerp( i.uv1, i.uv0, _UV );
                float4 node_3610 = _Time + _TimeEditor;
                float4 node_2328 = _Time + _TimeEditor;
                float node_2698_ang = node_2328.g;
                float node_2698_spd = _Speed;
                float node_2698_cos = cos(node_2698_spd*node_2698_ang);
                float node_2698_sin = sin(node_2698_spd*node_2698_ang);
                float2 node_2698_piv = float2(0.5,0.5);
                float2 node_2698 = (mul(_UV_var-node_2698_piv,float2x2( node_2698_cos, -node_2698_sin, node_2698_sin, node_2698_cos))+node_2698_piv);
                float2 _Rotation_var = lerp( (_UV_var+(node_3610.g*float2(_Speed_U,_Speed_V))), node_2698, _Rotation );
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(_Rotation_var, _Texture));
                float2 _mask_UV_var = lerp( i.uv0, i.uv1, _mask_UV );
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(_mask_UV_var, _mask));
                float3 finalColor = (((_Texture_var.rgb*_Texture_var.a)*_color.rgb*_color.a*_Intensity*i.vertexColor.a)*_mask_var.rgb);
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
