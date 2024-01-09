using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Controller_Script : MonoBehaviour
{
    int score;

    void Start()
    {
        score = HUD_Script.score_counter;
        score = 0;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("SprayCan"))
        {
            Destroy(other.gameObject);
            score += 1;
            HUD_Script.score_counter = score;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("RedBarrel"))
        {
            Destroy(other.gameObject);
            score = score <= 5 ? score = 0 : score -= 5;
            HUD_Script.score_counter = score;
        }
    }

}
