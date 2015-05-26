using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    public string EnemyTag = "Enemy";
    public string CollectableTag = "Collectable";
    public string SpringTag = "Spring";

    public Transform currentCheckpoint;

    public float bounceForceConstant = 1000f;
    public Rigidbody physicsRigidBody;

    public float edgeBuffer = 0.4f;

    public float smashDownForce = 10f;
    public float velocityHorizontal = 5f;

    public bool onlyOneCollision = false;
    public bool boostJumping = false;
    public bool isgrounded = false;

    public float distToGround = 0.1f;

    public Vector3 startingScale;

    public float velocityChangeCoeff = 0.9f;

    public float velocityConvertor;
    public float velocityPower = 2f;
    public float scaleVelocityMax;

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }

    public CollisionInfo collisions;

	// Use this for initialization
	void Start () {
        onlyOneCollision = false;
        boostJumping = false;
        startingScale = transform.localScale;
        collisions.Reset();
	}
	
    void BoostFromGround(float scale)
    {
        physicsRigidBody.velocity = new Vector3(physicsRigidBody.velocity.x, 0, 0); // set y velocity to zero
        physicsRigidBody.AddForce(new Vector3(0f, bounceForceConstant * scale, 0f)); // some constant force here
        onlyOneCollision = true;
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    void OnCollisionEnter(Collision Col)
    {
        if (!onlyOneCollision && !CurrentlyBoostJumping())
        {
            Vector3 localPosition = this.transform.worldToLocalMatrix.MultiplyPoint(Col.contacts[0].point);
            Vector3 direction = localPosition.normalized;
            if (Mathf.Abs(direction.x) < edgeBuffer && direction.y <= 0f) // collision bottom
            {
                BoostFromGround(1f);
            }
        }
    }

    void OnTriggerEnter (Collider col)
    {
        if (string.Compare(col.gameObject.tag,EnemyTag) == 0)
        {
            Debug.Log("Dead");
            this.transform.position = currentCheckpoint.position;
            this.physicsRigidBody.velocity = Vector3.zero;
        } else if (string.Compare(col.gameObject.tag,CollectableTag) == 0)
        {
            Destroy(col.gameObject);
        } else if (string.Compare(col.gameObject.tag,SpringTag) == 0)
        {
            BoostFromGround(2.5f);
        }
    }

    bool CurrentlyBoostJumping()
    {
        return Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow);
    }

	// Update is called once per frame
	void Update () {
        isgrounded = isGrounded();
        if (!CurrentlyBoostJumping() && boostJumping)
        {
            BoostFromGround(2f);
            boostJumping = false;
        }
        else if (CurrentlyBoostJumping())
        {
            physicsRigidBody.AddForce(Physics.gravity * physicsRigidBody.mass * smashDownForce);
            if (isGrounded())
            {
                boostJumping = true;
            }
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            //physicsRigidBody.AddForce(new Vector3(20f * Input.GetAxis("Horizontal"),0f,0f) * physicsRigidBody.mass);

            physicsRigidBody.velocity = new Vector3(physicsRigidBody.velocity.x * velocityChangeCoeff + (1f-velocityChangeCoeff) * Input.GetAxis("Horizontal") * velocityHorizontal, physicsRigidBody.velocity.y, 0f);
        }
	}

    void LateUpdate()
    {
        float yScale = startingScale.y * Mathf.Min(1.0f + Mathf.Pow(Mathf.Abs(physicsRigidBody.velocity.y),velocityPower) * velocityConvertor, scaleVelocityMax);
        this.transform.localScale = new Vector3(startingScale.x, yScale, startingScale.z);
        onlyOneCollision = false;
    }
}
