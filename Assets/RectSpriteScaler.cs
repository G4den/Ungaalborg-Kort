using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RectSpriteScaler : MonoBehaviour
{

    Texture tex;

    float xSize;
    float ySize;

    Image spriteRenderer;

    RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<Image>();
    }

    void Update()
    {
        xSize = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        ySize = transform.parent.GetComponent<RectTransform>().sizeDelta.y;

        tex = spriteRenderer.sprite.texture;

        print("(" + xSize + " / " + tex.width + ") * " + tex.height);

        if (tex.width <= tex.height)
            rectTransform.sizeDelta = new Vector2(xSize, (float)(xSize / tex.width) * tex.height);
        else
            rectTransform.sizeDelta = new Vector2((float)(ySize / tex.height) * tex.width, ySize);
    }
}
