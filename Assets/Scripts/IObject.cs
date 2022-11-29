using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IObject
{
    UnityAction Info();
    void Sound();
    void Name();
}
