using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPlaceScript : MonoBehaviour
{
    [field: Header("Rotation limits")]
    [Range(0,180),SerializeField] private float rightRotationLimit;
    public float RightRotationLimit
    {
        get { return rightRotationLimit; }
        private set { rightRotationLimit = value; }
    }

    [Range(0, 180), SerializeField] private float leftRotationLimit;
    public float LeftRotationLimit
    {
        get { return leftRotationLimit; }
        private set { leftRotationLimit = value; }
    }

    [Range(0, 90), SerializeField] private float elevationLimit;
    public float ElevationLimit
    {
        get { return elevationLimit; }
        private set { elevationLimit = value; }
    }

    [Range(0, 90), SerializeField] private float depressionLimit;
    public float DepressionLimit
    {
        get { return depressionLimit; }
        private set { depressionLimit = value; }
    }
}
