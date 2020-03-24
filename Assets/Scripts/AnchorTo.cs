using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorTo : MonoBehaviour
{
    [Range(0f,1f)]
    public float MinLerpToCenterRange;
    [Range(0f,1f)]
    public float MinClipRange;
     //grabbed this from unity's tutorial site
    public GameObject player;        //Public variable to store a reference to the player game object

    private Vector3 FollowPosition;
    private Vector3 AnchorRelativePosition;
    private Bounds Bounds;
    [Range(.1f,10f)]
    public float AnchorSpeed =1;
    public AnimationCurve Curve;
    // Use this for initialization
    void Start()
    {
        Bounds = GetComponent<Camera>().OrthographicBounds();
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 thisPos = transform.position;
        AnchorRelativePosition = Vector3.Scale(playerPos - thisPos, Bounds.size.Invert());
        int negativex = AnchorRelativePosition.x >= 0 ? 1 : -1;
        int negativey= AnchorRelativePosition.y >= 0 ? 1 : -1;
        FollowPosition.x = playerPos.x - Bounds.size.x * GetNewBoundsPercentage(MinLerpToCenterRange / 2, MinClipRange / 2, Mathf.Abs(AnchorRelativePosition.x)) * negativex;
        FollowPosition.y = playerPos.y - Bounds.size.y * GetNewBoundsPercentage(MinLerpToCenterRange / 2, MinClipRange / 2, Mathf.Abs(AnchorRelativePosition.y)) * negativey;
        FollowPosition.z = thisPos.z;

        transform.position = FollowPosition;
    }

    public float GetNewBoundsPercentage(float minLerpPer, float minClampPer, float currentPer)
    {
        var retVal = currentPer;
        if(currentPer > minLerpPer)
        {
            //lerp
            var tween = Curve.Evaluate((currentPer - minLerpPer) / (minClampPer - minLerpPer)) * AnchorSpeed;
            var delta = tween * Time.deltaTime * (minClampPer - minLerpPer);
            Debug.Log($"tweening {tween:0.000} resulting in total camera movement of {delta: 000.000}");
            retVal = currentPer = currentPer - delta;
        }

        if (currentPer > minClampPer)
        {
            //clamp
            Debug.Log($"outside {minClampPer} of bound size, clamping");
            retVal = minClampPer;
        }

        return retVal;
    }
}