using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soul : MonoBehaviour
{
    #region Properties

    public Vector2 velocity;
    private SoulDisplay soulDisplay;
    private bool moving;
    private Vector3 trajectory;

    #endregion

    // Called on start
    void Start()
    {
        soulDisplay = GameObject.FindGameObjectWithTag("SoulDisplay").GetComponent<SoulDisplay>();
        Redirect();
        moving = true;
    }

    // Helper method used to redirect during movement
    void Redirect()
    {
        var heading = Camera.main.ScreenToWorldPoint(soulDisplay.iconTransform.position) - transform.position;
        var direction = heading / Vector3.Distance(Camera.main.ScreenToWorldPoint(soulDisplay.iconTransform.position), transform.position);
        trajectory = direction;
    }

    // Called every fixed-framerate frame. Used to apply velocity.
    void FixedUpdate()
    {
        // Initialize velocity
        if (moving)
        {
            if (DisplayReached())
            {
                soulDisplay.GetComponent<SoulDisplay>().AddSoul();
                Destroy(gameObject);
            }
            else
            {
                Redirect();
                gameObject.transform.Translate(trajectory * velocity * Time.fixedDeltaTime);
            }
        }
    }

    // Helper method used to determine if in contact with icon
    private bool DisplayReached()
    {
        var displayPosition = Camera.main.ScreenToWorldPoint(soulDisplay.iconTransform.position);
        if (Vector3.Distance(displayPosition, transform.position) < 3.5f)
        {
            return true;
        }
        return false;
    }
}
