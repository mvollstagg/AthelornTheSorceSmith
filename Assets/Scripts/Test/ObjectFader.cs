using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    [SerializeField]
    float fadeSpeed = 1f, fadeAmount = 0.25f;
    float originalOpacity;
    Material material;
    public bool DoFade = false;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        originalOpacity = material.color.a;

    }

    // Update is called once per frame
    void Update()
    {
        if (DoFade)
        {
            Fade();
        }
        else
        {
            ReseteFade();
        }

    }
    void Fade()
    {
        Color currentColor = material.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        material.color = smoothColor;


    }
    void ReseteFade()
    {
        if (originalOpacity - material.color.a < 0.01)
        {
            material.color = Color.white;
            this.enabled = false;
            return;
        }
        Color currentColor = material.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b, Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
        material.color = smoothColor;
    }
}



