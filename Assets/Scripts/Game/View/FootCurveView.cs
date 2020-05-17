using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCurveView : MonoBehaviour
{
    public void SetCurve(float value)
    {
        GetComponent<Animator>().SetFloat("FootCurve", value);
    }
}
