using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIViewHandler : MonoBehaviour
{
    [SerializeField] private AIMovement AIMove = default;
    [SerializeField] private AIVisionHandler VHandler = default;
    [SerializeField] private float stopDistanceWall = 3.0f;
    [SerializeField] private float stopDistancePlatform = 1.0f;
    [SerializeField] private float visionPlayer = 4.0f;

    private bool lookingLeft = false;
    private bool lookForWallsAndPlatforms = true;
    private float actualVision = 4.0f;
    private bool foundPlayer = false;

    void FixedUpdate()
    {
        if (lookForWallsAndPlatforms)
        {
            checkForWallAndUpdateVisuals();
        }
        checkForPlayer();
    }

    void checkForPlayer()
    {
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, Vector2.right * (lookingLeft ? -1 : 1), actualVision, LayerMask.GetMask("Default"));

        if (!foundPlayer && hitWall.collider != null && hitWall.collider.tag == "Player")
        {
            foundPlayer = true;
            AIMove.FoundPlayer(lookingLeft);
        }

        if (foundPlayer && !(hitWall.collider != null && hitWall.collider.tag == "Player"))
        {
            foundPlayer = false;
            AIMove.LostPlayer();
        }
    }

    void checkForWallAndUpdateVisuals()
    {
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, Vector2.right * (lookingLeft ? -1 : 1), visionPlayer, LayerMask.GetMask("Walls"));

        if (hitWall.collider != null)
        {
            actualVision = hitWall.distance;
            VHandler.SetScale(hitWall.distance);
            if (hitWall.distance <= stopDistanceWall)
            {
                AIMove.ReachedWall(lookingLeft);
                lookForWallsAndPlatforms = false;
            }
        }

        RaycastHit2D hitPlatform = Physics2D.Raycast(transform.position, Vector2.right * (lookingLeft ? -1 : 1), visionPlayer, LayerMask.GetMask("Platforms"));

        if (hitPlatform.collider != null)
        {
            actualVision = hitPlatform.distance;
            VHandler.SetScale(hitPlatform.distance);
            if (hitPlatform.distance <= stopDistancePlatform)
            {
                AIMove.ReachedWall(lookingLeft);
                lookForWallsAndPlatforms = false;
            }
        }

        if (hitWall.collider == null && hitPlatform.collider == null)
        {
            VHandler.SetScale(visionPlayer);
            actualVision = visionPlayer;
        }
    }

    public void FlipView(bool _lookingLeft)
    {
        if (_lookingLeft == lookingLeft) return;

        lookingLeft = _lookingLeft;
        checkForWallAndUpdateVisuals();
        VHandler.FlipView(_lookingLeft);
        lookForWallsAndPlatforms = true;
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Walls"))
    //    {
    //        AIMove.ReachedWall(lookingLeft);
    //    }
    //    else if (other.tag == "Player")
    //    {
    //        Debug.Log("FOUND PLAYER!!!");
    //    }
    //}
}
