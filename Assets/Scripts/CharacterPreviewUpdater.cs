using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewUpdater : MonoBehaviour
{
    public RawImage characterFaceImage;
    public Camera characterCamera;
    public RenderTexture characterFaceRenderTexture;

    private bool isRendered = false;

    private void Start()
    {
        RenderCharacterFace();
    }

    private void RenderCharacterFace()
    {
        if (!isRendered && characterCamera != null && characterFaceRenderTexture != null)
        {
            RenderTexture previousTargetTexture = characterCamera.targetTexture;

            characterCamera.targetTexture = characterFaceRenderTexture;
            characterCamera.backgroundColor = Color.clear;
            characterCamera.Render();

            RenderTexture.active = characterFaceRenderTexture;
            Texture2D faceTexture = new Texture2D(characterFaceRenderTexture.width, characterFaceRenderTexture.height, TextureFormat.RGBA32, false);
            faceTexture.ReadPixels(new Rect(0, 0, characterFaceRenderTexture.width, characterFaceRenderTexture.height), 0, 0);
            faceTexture.Apply();
            RenderTexture.active = null;

            characterFaceImage.texture = faceTexture;
            characterCamera.targetTexture = previousTargetTexture;
            isRendered = true;

            characterCamera.gameObject.SetActive(false);
        }
    }
}