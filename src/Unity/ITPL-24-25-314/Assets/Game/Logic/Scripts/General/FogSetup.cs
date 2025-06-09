using UnityEngine;

namespace Game.Logic.Scripts.General{
    public class FogSetup : MonoBehaviour{
        public int textureSize = 512;
        public RenderTexture fogTexture;

        void Start()
        {
            fogTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.R8);
            fogTexture.enableRandomWrite = true; 
            fogTexture.Create();
        }
    }
}