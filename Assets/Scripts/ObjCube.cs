using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjCube : MonoBehaviour, IObject
{
    UnityAction ActSound;

    [SerializeField] AudioClip clip;

    private void Awake()
    {
        ActSound += Sound;
        ActSound += Name;
    }

    public UnityAction Info()
    {
        return ActSound;
    }

    public void Sound()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(clip);
    }

    public void Name()
    {
        Debug.Log(gameObject.name);
    }
}
