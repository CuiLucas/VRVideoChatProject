using UnityEngine;
using UnityEngine.Events;

public class FT_InteractableRay : MonoBehaviour
{
    public bool isActive = true;

    public UnityEvent OnRayEnter;
    public UnityEvent OnRayExit;
    public UnityEvent OnRayClick;

    public virtual void RayEnter()
    {
        OnRayEnter?.Invoke();
    }

    public virtual void RayExit()
    {
        OnRayExit?.Invoke();
    }

    public virtual void RayClick()
    {
        Debug.Log("射线点击:" + name);
        switch (tag)
        {
            case "BiZhi":
                print("点击到了壁纸");
                Sprite sprite = Sprite.Create(TextureToTexture2D(GetComponent<Material>().mainTexture),
                new Rect(0, 0, Screen.width, Screen.height),
                new Vector2(0.5f, 0.5f));
                FT_Pointer._Pointer.image_Show.sprite = sprite;
                break;
            case "3DItem":
                print("点击到了实物");
                break;
            default:
                break;
        }
    }

    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

}