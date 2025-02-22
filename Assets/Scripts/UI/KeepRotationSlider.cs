using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotationSlider : MonoBehaviour
{
    Quaternion defRot;
    void Start()
    {
        defRot = transform.rotation;
    }
    void Update()
    {
        transform.rotation = defRot;
    }
}
