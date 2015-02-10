// Shader created with Shader Forge Beta 0.27 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.27;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:0,limd:1,uamb:False,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,blpr:1,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,ufog:False,aust:False,igpj:False,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:0,x:33338,y:32442|diff-224-RGB,transm-223-OUT,alpha-240-A,clip-240-A;n:type:ShaderForge.SFN_TexCoord,id:26,x:34443,y:32505,uv:0;n:type:ShaderForge.SFN_Vector1,id:28,x:34443,y:32662,v1:1;n:type:ShaderForge.SFN_Vector1,id:223,x:33996,y:32590,v1:0;n:type:ShaderForge.SFN_Tex2d,id:224,x:33666,y:32374,ptlb:node_224,ptin:_node_224,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:240,x:33809,y:32776,ptlb:node_240,ptin:_node_240,ntxv:0,isnm:False;proporder:224-240;pass:END;sub:END;*/

Shader "Shader Forge/Examples/Refraction" {
    Properties {
        _node_224 ("node_224", 2D) = "white" {}
        _node_240 ("node_240", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_224; uniform float4 _node_224_ST;
            uniform sampler2D _node_240; uniform float4 _node_240_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_269 = i.uv0;
                float4 node_240 = tex2D(_node_240,TRANSFORM_TEX(node_269.rg, _node_240));
                clip(node_240.a - 0.5);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float node_223 = 0.0;
                float3 backLight = max(0.0, -NdotL ) * float3(node_223,node_223,node_223);
                float3 diffuse = (forwardLight+backLight) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_224,TRANSFORM_TEX(node_269.rg, _node_224)).rgb;
/// Final Color:
                return fixed4(finalColor,node_240.a);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_224; uniform float4 _node_224_ST;
            uniform sampler2D _node_240; uniform float4 _node_240_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_270 = i.uv0;
                float4 node_240 = tex2D(_node_240,TRANSFORM_TEX(node_270.rg, _node_240));
                clip(node_240.a - 0.5);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                
                float nSign = sign( dot( viewDirection, i.normalDir ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 forwardLight = max(0.0, NdotL );
                float node_223 = 0.0;
                float3 backLight = max(0.0, -NdotL ) * float3(node_223,node_223,node_223);
                float3 diffuse = (forwardLight+backLight) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_224,TRANSFORM_TEX(node_270.rg, _node_224)).rgb;
/// Final Color:
                return fixed4(finalColor * node_240.a,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _node_240; uniform float4 _node_240_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float4 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_271 = i.uv0;
                float4 node_240 = tex2D(_node_240,TRANSFORM_TEX(node_271.rg, _node_240));
                clip(node_240.a - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers gles xbox360 ps3 flash 
            #pragma target 3.0
            uniform sampler2D _node_240; uniform float4 _node_240_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float4 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_272 = i.uv0;
                float4 node_240 = tex2D(_node_240,TRANSFORM_TEX(node_272.rg, _node_240));
                clip(node_240.a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
