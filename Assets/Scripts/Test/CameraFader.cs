using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFader : MonoBehaviour
{
    private List<ObjectFader> fadersCurrentlyHidden = new List<ObjectFader>();
    private ObjectFader _faderFirst;
    [SerializeField]
    GameObject Player;
    [SerializeField]
    LayerMask fadeableLayer;
    void Update()
    {
        FadeAll();

    }


    public void FadeAll()
    {
        if (Player == null)
        {
            return;
        }

        Vector3 direction = Player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude, fadeableLayer);
        foreach (ObjectFader fader in fadersCurrentlyHidden)
        {
            fader.DoFade = false;
        }
        fadersCurrentlyHidden.Clear();
        foreach (RaycastHit hit in hits)
        {

            if (hit.collider == null || hit.collider.gameObject == Player)
            {
                continue;
            }


            ObjectFader fader = hit.collider.gameObject.GetComponent<ObjectFader>();
            if (fader != null)
            {
                fader.enabled = true;
                fader.DoFade = true;
                fadersCurrentlyHidden.Add(fader);
            }
        }

    }

    // Función auxiliar para verificar si un objeto bloquea al jugador

    public void FadeFirst()
    {

        if (Player == null)
        {
            return;

        }
        Vector3 direction = Player.transform.position - transform.position;
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == null)
            {
                return;
            }
            if (hit.collider.gameObject == Player)
            {
                if (_faderFirst != null)
                {
                    _faderFirst.DoFade = false;
                }
            }
            else
            {
                if (_faderFirst != null)
                {
                    _faderFirst.DoFade = false;
                }
                _faderFirst = hit.collider.gameObject.GetComponent<ObjectFader>();
                if (_faderFirst != null)
                {
                    _faderFirst.enabled = true;
                    _faderFirst.DoFade = true;
                }
            }
        }
    }
}