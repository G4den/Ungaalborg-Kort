using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HiResScreenShots : MonoBehaviour
{
    public int resWidth = 1080;
    public int resHeight = 1920;

    public ImagePickAndSave imagePickAndSave;

    public Camera cam;

    public Image image;


    private Texture2D _screenShot;

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            renderAndSave();
    }

    public void renderAndSave ()
    {
        StartCoroutine(TakeScreenShot());
    }

    private IEnumerator TakeScreenShot()
    {
        yield return new WaitForEndOfFrame();

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        cam.targetTexture = rt;
        _screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        cam.Render();
        RenderTexture.active = rt;
        _screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        _screenShot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        //Sprite sprite = Sprite.Create(_screenShot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));


        imagePickAndSave.SaveToGallery(_screenShot, "Ungaalborg", "Kort");
    }
}