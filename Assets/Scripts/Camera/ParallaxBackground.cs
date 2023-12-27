using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private SpriteRenderer sr;
    private float startPos;
    private float lenght;

    private float offset;
    private float totalDist;
    
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float parallax;

    public void Start() {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position.x;
        lenght = sr.bounds.size.x;
    }

    public void Update() {
        
        offset = target.position.x * parallax;
        totalDist = target.position.x * (1 - parallax);

        transform.position = new Vector3(startPos + offset, transform.position.y, transform.position.z);

        if(totalDist < startPos - lenght) startPos -= lenght;
        else if(totalDist > startPos + lenght) startPos += lenght;
    }
}
