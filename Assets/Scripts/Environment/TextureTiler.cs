using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTiler : MonoBehaviour
{
    [SerializeField]
    private float tileXOverride;
    [SerializeField]
    private float tileYOverride;
    [SerializeField]
    private float scalarOverride;
    [SerializeField]
    private int tiledMaterialId;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.materials[tiledMaterialId] = Instantiate<Material>(renderer.materials[tiledMaterialId]);
        float scalar = scalarOverride > 0 ? scalarOverride : 64 / renderer.material.mainTexture.width;
        renderer.materials[tiledMaterialId].mainTextureScale = new Vector2(tileXOverride != 0 ? tileXOverride : transform.lossyScale.x * scalar, tileYOverride != 0 ? tileYOverride : transform.lossyScale.y * scalar);
    }
}
