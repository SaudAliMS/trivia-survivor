using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{
    private Material material;
//    public SpriteRenderer sRenderer;
    private float speed = -0.2f;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(0, Time.time * speed);
        //        sRenderer.material.mainTextureOffset = offset;

        material.mainTextureOffset = offset;
    }
}
