﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetreDetecter : MonoBehaviour
{
    public float Metre;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == Constants.TAG_STACK)
        {
            GameManager.instance.ScoreController.FurtherStackMetre = Metre;
            GameManager.instance.SmothFollow.target = transform;
            gameObject.SetActive(false);
        }
    }
}
