using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pixelDisappear : MonoBehaviour
{
    private Texture2D spriteTexture;
    private Color32[] spritePixels;
    private int width, height;

    private void Start()
    {
        Texture2D originalSpriteTexture = GetComponent<SpriteRenderer>().sprite.texture;
        spriteTexture = Instantiate(originalSpriteTexture);
        width = originalSpriteTexture.width;
        height = originalSpriteTexture.height;
        GetComponent<SpriteRenderer>().sprite = Sprite.Create(spriteTexture, new Rect(0, 0, width, height), new Vector2(0f, 0f), 16);
        //spritePixels = spriteTexture.GetPixels32();
        spritePixels = (Color32[])spriteTexture.GetPixels32().Clone();
        StartCoroutine(doAll());
    }


    private IEnumerator doAll()
    {
        int remainingPixels = 0;
        for (int i = 0; i < spritePixels.Length; i++)
        {
            if (spritePixels[i].a != 0)
            {
                remainingPixels++;
            }
        }
        if (remainingPixels == 0)
        {
            Destroy(gameObject);
            yield break;
        }
        int randomPixel = Random.Range(0, spritePixels.Length);
        while (spritePixels[randomPixel].a == 0)
        {
            randomPixel = Random.Range(0, spritePixels.Length);
        }
        spritePixels[randomPixel].a = 0;
        spriteTexture.SetPixels32(spritePixels);
        spriteTexture.Apply();
        yield return new WaitForSeconds(0.005f);
        StartCoroutine(doAll());

    }

}
