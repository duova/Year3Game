Shader "Shader Graphs/UIBoxShaderHLSL"
{
    Properties
    {
        _Width("Width", Float) = 0.1
        _BorderColor("BorderColor", Color) = (0, 0, 0, 0)
        _PixelSize("PixelSize", Vector) = (1, 1, 0, 0)
        _OpenPercentage("OpenPercentage", Float) = 0.5
        _BackgroundAlpha("BackgroundAlpha", Float) = 0.4
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalSpriteUnlitSubTarget"
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "Universal2D"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Width;
        float4 _BorderColor;
        float2 _PixelSize;
        float _OpenPercentage;
        float _BackgroundAlpha;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Round_float2(float2 In, out float2 Out)
        {
            Out = round(In);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Ceiling_float2(float2 In, out float2 Out)
        {
            Out = ceil(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_InvertColors_float2(float2 In, float2 InvertColors, out float2 Out)
        {
            Out = abs(InvertColors - In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_e7b4f4416ef6419f8ddc9bc2a2324ba2_Out_0 = _BorderColor;
            float4 _UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0 = IN.uv0;
            float2 _Rotate_9750281fa46942188643bd3c8e58629e_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 180, _Rotate_9750281fa46942188643bd3c8e58629e_Out_3);
            float _Property_799288f5eff74ad0b0886ddee556ad74_Out_0 = _OpenPercentage;
            float _Add_56b7396878804e36abf1c34332e6ce6e_Out_2;
            Unity_Add_float(_Property_799288f5eff74ad0b0886ddee556ad74_Out_0, -0.5, _Add_56b7396878804e36abf1c34332e6ce6e_Out_2);
            float4 _Combine_1398120c6b1c4aea866ddee831d9bc63_RGBA_4;
            float3 _Combine_1398120c6b1c4aea866ddee831d9bc63_RGB_5;
            float2 _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6;
            Unity_Combine_float(0, _Add_56b7396878804e36abf1c34332e6ce6e_Out_2, 0, 0, _Combine_1398120c6b1c4aea866ddee831d9bc63_RGBA_4, _Combine_1398120c6b1c4aea866ddee831d9bc63_RGB_5, _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6);
            float2 _Add_902273281d3844308686203d4e45f4dd_Out_2;
            Unity_Add_float2(_Rotate_9750281fa46942188643bd3c8e58629e_Out_3, _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6, _Add_902273281d3844308686203d4e45f4dd_Out_2);
            float2 _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1;
            Unity_Round_float2(_Add_902273281d3844308686203d4e45f4dd_Out_2, _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1);
            float _Split_e86506a0a4924e579691a43bfbaf4134_R_1 = _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1[0];
            float _Split_e86506a0a4924e579691a43bfbaf4134_G_2 = _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1[1];
            float _Split_e86506a0a4924e579691a43bfbaf4134_B_3 = 0;
            float _Split_e86506a0a4924e579691a43bfbaf4134_A_4 = 0;
            float2 _Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 0, _Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3);
            float _Property_04ea3d104d874d58980cdc53fb94921c_Out_0 = _Width;
            float2 _Property_ce7170dc0b8644c4b3f912fbd9705dc2_Out_0 = _PixelSize;
            float2 _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2;
            Unity_Multiply_float2_float2(_Property_ce7170dc0b8644c4b3f912fbd9705dc2_Out_0, float2(0.01, 0.01), _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2);
            float _Split_c4942bcf02c740d696ebd5e789ab3380_R_1 = _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2[0];
            float _Split_c4942bcf02c740d696ebd5e789ab3380_G_2 = _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2[1];
            float _Split_c4942bcf02c740d696ebd5e789ab3380_B_3 = 0;
            float _Split_c4942bcf02c740d696ebd5e789ab3380_A_4 = 0;
            float _Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2;
            Unity_Divide_float(_Property_04ea3d104d874d58980cdc53fb94921c_Out_0, _Split_c4942bcf02c740d696ebd5e789ab3380_R_1, _Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2);
            float2 _Vector2_bc84e6cc11e1471eb49fa334658247e1_Out_0 = float2(_Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2, 1);
            float2 _Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2;
            Unity_Subtract_float2(_Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3, _Vector2_bc84e6cc11e1471eb49fa334658247e1_Out_0, _Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2);
            float2 _Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1;
            Unity_Ceiling_float2(_Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2, _Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1);
            float2 _Rotate_1a78881da4364abfaba854d54e653412_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 90, _Rotate_1a78881da4364abfaba854d54e653412_Out_3);
            float _Divide_4953004b6f274d33bd9aa90390d130cd_Out_2;
            Unity_Divide_float(_Property_04ea3d104d874d58980cdc53fb94921c_Out_0, _Split_c4942bcf02c740d696ebd5e789ab3380_G_2, _Divide_4953004b6f274d33bd9aa90390d130cd_Out_2);
            float2 _Vector2_b469503d92d649688ec36d07161f4667_Out_0 = float2(_Divide_4953004b6f274d33bd9aa90390d130cd_Out_2, 1);
            float2 _Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2;
            Unity_Subtract_float2(_Rotate_1a78881da4364abfaba854d54e653412_Out_3, _Vector2_b469503d92d649688ec36d07161f4667_Out_0, _Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2);
            float2 _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1;
            Unity_Ceiling_float2(_Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2, _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1);
            float2 _Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2;
            Unity_Multiply_float2_float2(_Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1, _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1, _Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2);
            float2 _Rotate_860a212d604942ad95f877c29e7d1e56_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 180, _Rotate_860a212d604942ad95f877c29e7d1e56_Out_3);
            float2 _Vector2_60e7def779d74b1aafb006d2e4f24220_Out_0 = float2(_Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2, 1);
            float2 _Subtract_0862faeb238d4c85bd3f41e964952915_Out_2;
            Unity_Subtract_float2(_Rotate_860a212d604942ad95f877c29e7d1e56_Out_3, _Vector2_60e7def779d74b1aafb006d2e4f24220_Out_0, _Subtract_0862faeb238d4c85bd3f41e964952915_Out_2);
            float2 _Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1;
            Unity_Ceiling_float2(_Subtract_0862faeb238d4c85bd3f41e964952915_Out_2, _Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1);
            float2 _Rotate_22a269632468498f9d26cdf324563f0f_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 270, _Rotate_22a269632468498f9d26cdf324563f0f_Out_3);
            float2 _Vector2_46114c00d6414447b42cc96721346bc7_Out_0 = float2(_Divide_4953004b6f274d33bd9aa90390d130cd_Out_2, 1);
            float2 _Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2;
            Unity_Subtract_float2(_Rotate_22a269632468498f9d26cdf324563f0f_Out_3, _Vector2_46114c00d6414447b42cc96721346bc7_Out_0, _Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2);
            float _Property_edff904d8b43496d92910c6fae070b0c_Out_0 = _OpenPercentage;
            float _OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1;
            Unity_OneMinus_float(_Property_edff904d8b43496d92910c6fae070b0c_Out_0, _OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1);
            float2 _Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2;
            Unity_Subtract_float2(_Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2, (_OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1.xx), _Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2);
            float2 _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1;
            Unity_Ceiling_float2(_Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2, _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1);
            float2 _Multiply_c16858283d7748cda183ad4fa697b052_Out_2;
            Unity_Multiply_float2_float2(_Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1, _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1, _Multiply_c16858283d7748cda183ad4fa697b052_Out_2);
            float2 _Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2;
            Unity_Multiply_float2_float2(_Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2, _Multiply_c16858283d7748cda183ad4fa697b052_Out_2, _Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2);
            float2 _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1;
            float2 _InvertColors_1d7ebb005877434997ff694674a936d0_InvertColors = float2 (1, 0);
            Unity_InvertColors_float2(_Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2, _InvertColors_1d7ebb005877434997ff694674a936d0_InvertColors, _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1);
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_R_1 = _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1[0];
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_G_2 = _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1[1];
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_B_3 = 0;
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_A_4 = 0;
            float _Property_0d02ab15fecd4446a1f3bcd8457506cc_Out_0 = _BackgroundAlpha;
            float _Add_a93dd523795c4584839df742b66140c1_Out_2;
            Unity_Add_float(_Split_db1ce8c1ab4b415ba54c08e99a599435_R_1, _Property_0d02ab15fecd4446a1f3bcd8457506cc_Out_0, _Add_a93dd523795c4584839df742b66140c1_Out_2);
            float _Multiply_1514306055ec4f909135d104f5d272cf_Out_2;
            Unity_Multiply_float_float(_Split_e86506a0a4924e579691a43bfbaf4134_G_2, _Add_a93dd523795c4584839df742b66140c1_Out_2, _Multiply_1514306055ec4f909135d104f5d272cf_Out_2);
            surface.BaseColor = (_Property_e7b4f4416ef6419f8ddc9bc2a2324ba2_Out_0.xyz);
            surface.Alpha = _Multiply_1514306055ec4f909135d104f5d272cf_Out_2;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "Sprite Unlit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEFORWARD
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
             float4 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyzw =  input.texCoord0;
            output.interp2.xyzw =  input.color;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.texCoord0 = input.interp1.xyzw;
            output.color = input.interp2.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _Width;
        float4 _BorderColor;
        float2 _PixelSize;
        float _OpenPercentage;
        float _BackgroundAlpha;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            Rotation = Rotation * (3.1415926f/180.0f);
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_Round_float2(float2 In, out float2 Out)
        {
            Out = round(In);
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Ceiling_float2(float2 In, out float2 Out)
        {
            Out = ceil(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_InvertColors_float2(float2 In, float2 InvertColors, out float2 Out)
        {
            Out = abs(InvertColors - In);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_e7b4f4416ef6419f8ddc9bc2a2324ba2_Out_0 = _BorderColor;
            float4 _UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0 = IN.uv0;
            float2 _Rotate_9750281fa46942188643bd3c8e58629e_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 180, _Rotate_9750281fa46942188643bd3c8e58629e_Out_3);
            float _Property_799288f5eff74ad0b0886ddee556ad74_Out_0 = _OpenPercentage;
            float _Add_56b7396878804e36abf1c34332e6ce6e_Out_2;
            Unity_Add_float(_Property_799288f5eff74ad0b0886ddee556ad74_Out_0, -0.5, _Add_56b7396878804e36abf1c34332e6ce6e_Out_2);
            float4 _Combine_1398120c6b1c4aea866ddee831d9bc63_RGBA_4;
            float3 _Combine_1398120c6b1c4aea866ddee831d9bc63_RGB_5;
            float2 _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6;
            Unity_Combine_float(0, _Add_56b7396878804e36abf1c34332e6ce6e_Out_2, 0, 0, _Combine_1398120c6b1c4aea866ddee831d9bc63_RGBA_4, _Combine_1398120c6b1c4aea866ddee831d9bc63_RGB_5, _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6);
            float2 _Add_902273281d3844308686203d4e45f4dd_Out_2;
            Unity_Add_float2(_Rotate_9750281fa46942188643bd3c8e58629e_Out_3, _Combine_1398120c6b1c4aea866ddee831d9bc63_RG_6, _Add_902273281d3844308686203d4e45f4dd_Out_2);
            float2 _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1;
            Unity_Round_float2(_Add_902273281d3844308686203d4e45f4dd_Out_2, _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1);
            float _Split_e86506a0a4924e579691a43bfbaf4134_R_1 = _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1[0];
            float _Split_e86506a0a4924e579691a43bfbaf4134_G_2 = _Round_ad6e6dc8f05f46279cd26c11703661e3_Out_1[1];
            float _Split_e86506a0a4924e579691a43bfbaf4134_B_3 = 0;
            float _Split_e86506a0a4924e579691a43bfbaf4134_A_4 = 0;
            float2 _Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 0, _Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3);
            float _Property_04ea3d104d874d58980cdc53fb94921c_Out_0 = _Width;
            float2 _Property_ce7170dc0b8644c4b3f912fbd9705dc2_Out_0 = _PixelSize;
            float2 _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2;
            Unity_Multiply_float2_float2(_Property_ce7170dc0b8644c4b3f912fbd9705dc2_Out_0, float2(0.01, 0.01), _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2);
            float _Split_c4942bcf02c740d696ebd5e789ab3380_R_1 = _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2[0];
            float _Split_c4942bcf02c740d696ebd5e789ab3380_G_2 = _Multiply_3cd86dc6d12941819164295baa4e679e_Out_2[1];
            float _Split_c4942bcf02c740d696ebd5e789ab3380_B_3 = 0;
            float _Split_c4942bcf02c740d696ebd5e789ab3380_A_4 = 0;
            float _Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2;
            Unity_Divide_float(_Property_04ea3d104d874d58980cdc53fb94921c_Out_0, _Split_c4942bcf02c740d696ebd5e789ab3380_R_1, _Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2);
            float2 _Vector2_bc84e6cc11e1471eb49fa334658247e1_Out_0 = float2(_Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2, 1);
            float2 _Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2;
            Unity_Subtract_float2(_Rotate_b3974d982c524474bc1f0bb6da7851a5_Out_3, _Vector2_bc84e6cc11e1471eb49fa334658247e1_Out_0, _Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2);
            float2 _Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1;
            Unity_Ceiling_float2(_Subtract_d55fe29eb51d48f6b6ff90f0623ab3e0_Out_2, _Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1);
            float2 _Rotate_1a78881da4364abfaba854d54e653412_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 90, _Rotate_1a78881da4364abfaba854d54e653412_Out_3);
            float _Divide_4953004b6f274d33bd9aa90390d130cd_Out_2;
            Unity_Divide_float(_Property_04ea3d104d874d58980cdc53fb94921c_Out_0, _Split_c4942bcf02c740d696ebd5e789ab3380_G_2, _Divide_4953004b6f274d33bd9aa90390d130cd_Out_2);
            float2 _Vector2_b469503d92d649688ec36d07161f4667_Out_0 = float2(_Divide_4953004b6f274d33bd9aa90390d130cd_Out_2, 1);
            float2 _Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2;
            Unity_Subtract_float2(_Rotate_1a78881da4364abfaba854d54e653412_Out_3, _Vector2_b469503d92d649688ec36d07161f4667_Out_0, _Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2);
            float2 _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1;
            Unity_Ceiling_float2(_Subtract_3943a100205542bd82d3eb1f3a066b10_Out_2, _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1);
            float2 _Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2;
            Unity_Multiply_float2_float2(_Ceiling_d866bc2b21a2470c9916d26c3e3ec2b9_Out_1, _Ceiling_a144c857ae274b1796629554d0de7f99_Out_1, _Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2);
            float2 _Rotate_860a212d604942ad95f877c29e7d1e56_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 180, _Rotate_860a212d604942ad95f877c29e7d1e56_Out_3);
            float2 _Vector2_60e7def779d74b1aafb006d2e4f24220_Out_0 = float2(_Divide_bbbd46a2acbd40138c937dc8e707c139_Out_2, 1);
            float2 _Subtract_0862faeb238d4c85bd3f41e964952915_Out_2;
            Unity_Subtract_float2(_Rotate_860a212d604942ad95f877c29e7d1e56_Out_3, _Vector2_60e7def779d74b1aafb006d2e4f24220_Out_0, _Subtract_0862faeb238d4c85bd3f41e964952915_Out_2);
            float2 _Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1;
            Unity_Ceiling_float2(_Subtract_0862faeb238d4c85bd3f41e964952915_Out_2, _Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1);
            float2 _Rotate_22a269632468498f9d26cdf324563f0f_Out_3;
            Unity_Rotate_Degrees_float((_UV_e532ebe2d74444e0bffb49c0d8252fdb_Out_0.xy), float2 (0.5, 0.5), 270, _Rotate_22a269632468498f9d26cdf324563f0f_Out_3);
            float2 _Vector2_46114c00d6414447b42cc96721346bc7_Out_0 = float2(_Divide_4953004b6f274d33bd9aa90390d130cd_Out_2, 1);
            float2 _Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2;
            Unity_Subtract_float2(_Rotate_22a269632468498f9d26cdf324563f0f_Out_3, _Vector2_46114c00d6414447b42cc96721346bc7_Out_0, _Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2);
            float _Property_edff904d8b43496d92910c6fae070b0c_Out_0 = _OpenPercentage;
            float _OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1;
            Unity_OneMinus_float(_Property_edff904d8b43496d92910c6fae070b0c_Out_0, _OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1);
            float2 _Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2;
            Unity_Subtract_float2(_Subtract_183628c8b34e43fba37a7b3c17d5180d_Out_2, (_OneMinus_6d580d0cd35247eea49fac3e82a574a4_Out_1.xx), _Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2);
            float2 _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1;
            Unity_Ceiling_float2(_Subtract_8fd6a1a1a4fb4c499ae47e2a914d46ce_Out_2, _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1);
            float2 _Multiply_c16858283d7748cda183ad4fa697b052_Out_2;
            Unity_Multiply_float2_float2(_Ceiling_d7b49a4a6b414ed48d9cefffb8a3e41a_Out_1, _Ceiling_e681929cbf324547b5a137f8ce6e02af_Out_1, _Multiply_c16858283d7748cda183ad4fa697b052_Out_2);
            float2 _Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2;
            Unity_Multiply_float2_float2(_Multiply_8b432c6bdbc343f0a974171aa2341c34_Out_2, _Multiply_c16858283d7748cda183ad4fa697b052_Out_2, _Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2);
            float2 _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1;
            float2 _InvertColors_1d7ebb005877434997ff694674a936d0_InvertColors = float2 (1, 0);
            Unity_InvertColors_float2(_Multiply_6861dcbf1be4465ea4013b3d490181f9_Out_2, _InvertColors_1d7ebb005877434997ff694674a936d0_InvertColors, _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1);
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_R_1 = _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1[0];
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_G_2 = _InvertColors_1d7ebb005877434997ff694674a936d0_Out_1[1];
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_B_3 = 0;
            float _Split_db1ce8c1ab4b415ba54c08e99a599435_A_4 = 0;
            float _Property_0d02ab15fecd4446a1f3bcd8457506cc_Out_0 = _BackgroundAlpha;
            float _Add_a93dd523795c4584839df742b66140c1_Out_2;
            Unity_Add_float(_Split_db1ce8c1ab4b415ba54c08e99a599435_R_1, _Property_0d02ab15fecd4446a1f3bcd8457506cc_Out_0, _Add_a93dd523795c4584839df742b66140c1_Out_2);
            float _Multiply_1514306055ec4f909135d104f5d272cf_Out_2;
            Unity_Multiply_float_float(_Split_e86506a0a4924e579691a43bfbaf4134_G_2, _Add_a93dd523795c4584839df742b66140c1_Out_2, _Multiply_1514306055ec4f909135d104f5d272cf_Out_2);
            surface.BaseColor = (_Property_e7b4f4416ef6419f8ddc9bc2a2324ba2_Out_0.xyz);
            surface.Alpha = _Multiply_1514306055ec4f909135d104f5d272cf_Out_2;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}