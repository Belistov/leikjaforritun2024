using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hlutursnu : MonoBehaviour
{
    public float xSnu = 0;
    public float ySnu = 0;
    public float zSnu = 0;

    void Update()
    {
        transform.Rotate(new Vector3(xSnu, ySnu, zSnu) * Time.deltaTime);
    }
}
