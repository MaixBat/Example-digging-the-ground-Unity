using UnityEngine;

public class ObjSearch : MonoBehaviour, IObject
{
    [SerializeField] AudioClip _clip;

    public void PlaySound()
    {
        gameObject.GetComponent<AudioSource>().PlayOneShot(_clip);
    }

    public void TakeName()
    {
        Debug.Log(gameObject.name);
    }
}
