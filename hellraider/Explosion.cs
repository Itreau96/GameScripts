using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Animation file used to determine destroy time
    public AnimationClip explodeAnim;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, explodeAnim.length);
    }
}
