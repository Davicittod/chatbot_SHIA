using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Ce script gère la direction du regard de l'agent
 */
public class Gaze : MonoBehaviour
{
    public Transform leftEye;
    public Transform rightEye;
    public Transform head;
    public Transform spine;
    public Transform target;
    public float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;
        
        // Determine which direction to rotate towards
        Vector3 spineDirection = target.position - spine.position;
        // Rotate the forward vector towards the target direction by one step
        Vector3 newSpineDirection = Vector3.RotateTowards(spine.forward, spineDirection, 0.25f, 0.0f);
        spine.rotation = Quaternion.LookRotation(newSpineDirection);

        // Determine which direction to rotate towards
        Vector3 headDirection = target.position - head.position;
        // Rotate the forward vector towards the target direction by one step
        Vector3 newHeadDirection = Vector3.RotateTowards(head.forward, headDirection, 0.75f, 0.0f);
        head.rotation = Quaternion.LookRotation(newHeadDirection);

        // Determine which direction to rotate towards
        Vector3 leftEyeDirection = target.position - leftEye.position;
        // Determine which direction to rotate towards
        Vector3 rightEyeDirection = target.position - rightEye.position;
        // Rotate the forward vector towards the target direction by one step
        Vector3 newLeftDirection = Vector3.RotateTowards(leftEye.forward, leftEyeDirection, 0.70f, 0.0f);
        // Rotate the forward vector towards the target direction by one step
        Vector3 newRightDirection = Vector3.RotateTowards(rightEye.forward, rightEyeDirection, 0.70f, 0.0f);
        //Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);
        // Calculate a rotation a step closer to the target and applies rotation to this object
        leftEye.rotation = Quaternion.LookRotation(newLeftDirection);
        // Calculate a rotation a step closer to the target and applies rotation to this object
        rightEye.rotation = Quaternion.LookRotation(newRightDirection);
        
        
        
    }
}
