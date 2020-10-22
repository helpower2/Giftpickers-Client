using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : MonoBehaviour
{
    public int _networkId { get; private set; }

    public void ReciveTransform(Vector3 _position, Quaternion _quaternion, Vector3 _scale)
    {
        this.transform.position = _position;
        this.transform.rotation = _quaternion;
        this.transform.localScale = _scale;
    }

    public void SetId(int _id)
    {
        _networkId = _id;
    }
}
