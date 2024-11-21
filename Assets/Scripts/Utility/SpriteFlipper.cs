using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    public Transform objectRoot;
    public int direction;    
    
    private void Start()
    {
        StartCoroutine(FixFlip());
    }

    IEnumerator FixFlip()
    {
        yield return null;
        direction = objectRoot.transform.localScale.x < 0 ? -1 : 1;
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y,
            transform.localScale.z);
    }

    public void SetSpriteFlip(int _direction)
    {
        StopAllCoroutines();
        direction = _direction;
        transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y,
            transform.localScale.z);
    }
}
