using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineConnectionSprite : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Transform firstObject;
    public Transform secondObject;

    private float width = 1f;
    // Start is called before the first frame update
    void Start()
    {
        width = spriteRenderer.size.x;
    }

    private void OnEnable()
    {
        spriteRenderer.enabled = true;
    }

    private void OnDisable()
    {
        spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        var z = transform.position.z;
        var firstPos = firstObject.transform.position;
        var secondPos = secondObject.transform.position;
        var pos = (firstPos + secondPos) / 2;
        pos.z = z;
        spriteRenderer.transform.position = pos;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, firstPos - secondPos));
        spriteRenderer.size = new Vector2(width, Vector2.Distance(firstPos, secondPos));
    }
}
