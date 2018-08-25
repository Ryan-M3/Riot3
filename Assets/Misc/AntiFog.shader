Shader "Custom/AntiFog" {
    SubShader{
        Pass {
            CGPROGRAM
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            void MyVertexProgram() {
            }

            void MyFragmentProgram() {
            }


            ENDCG
        }
    }
}
