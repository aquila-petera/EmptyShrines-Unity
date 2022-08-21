using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteShadowCaster : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;

    new private Renderer renderer;
    private Material mat;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        mat = Instantiate(renderer.material);
        mat.color = new Color(0, 0, 0, 0);
        renderer.material = mat;
        offset = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        renderer.transform.localScale = new Vector3(sprite.sprite.textureRect.width / 100f, sprite.sprite.textureRect.height / 100f, 1);
        renderer.transform.localPosition = new Vector3(-(24 - sprite.sprite.textureRect.width) / 200f, -(32 - sprite.sprite.textureRect.height) / 200f, 0) + offset;
        Texture2D newTex = new Texture2D((int)sprite.sprite.textureRect.width, (int)sprite.sprite.textureRect.height);
        var pixels = sprite.sprite.texture.GetPixels((int)sprite.sprite.textureRect.x, (int)sprite.sprite.textureRect.y, (int)sprite.sprite.textureRect.width, (int)sprite.sprite.textureRect.height);
        newTex.SetPixels(pixels);
        newTex.Apply();
        mat.SetTexture("_BaseMap", newTex);
    }
}
