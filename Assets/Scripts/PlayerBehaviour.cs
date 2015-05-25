using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

    public string EnemyTag = "Enemy";
    public string CollectableTag = "Collectable";

    public Transform currentCheckpoint;

    public float bounceForceConstant = 1000f;
    public Rigidbody physicsRigidBody;

    public float edgeBuffer = 0.4f;

    public float velocityDown = -5f;
    public float velocityHorizontal = 5f;

    public bool onlyOneCollision = false;

    public Vector3 startingScale;

    public float velocityConvertor;
    public float velocityPower = 2f;
    public float scaleVelocityMax;

	// Use this for initialization
	void Start () {
        onlyOneCollision = false;
        startingScale = transform.localScale;
	}
	
    void OnCollisionEnter(Collision Col)
    {
        if (!onlyOneCollision && !CurrentlyBoostJumping())
        {
            Vector3 localPosition = this.transform.worldToLocalMatrix.MultiplyPoint(Col.contacts[0].point);
            Vector3 direction = localPosition.normalized;
            if (Mathf.Abs(direction.x) < edgeBuffer) {
                physicsRigidBody.velocity = new Vector3(physicsRigidBody.velocity.x, 0, 0); // set y velocity to zero
                physicsRigidBody.AddForce(new Vector3(0f, bounceForceConstant, 0f)); // some constant force here
                onlyOneCollision = true;
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
        }
    }

    bool CurrentlyBoostJumping()
    {
        return Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.RightArrow);
    }

	// Update is called once per frame
	void Update () {
        if (CurrentlyBoostJumping())
        {
            physicsRigidBody.velocity = new Vector3(physicsRigidBody.velocity.x, velocityDown, 0f);
        }
        else
        {
            physicsRigidBody.velocity = new Vector3(Input.GetAxis("Horizontal") * velocityHorizontal, physicsRigidBody.velocity.y, 0f);
        }
	}

    void LateUpdate()
    {
        float yScale = startingScale.y * Mathf.Min(1.0f + Mathf.Pow(Mathf.Abs(physicsRigidBody.velocity.y),velocityPower) * velocityConvertor, scaleVelocityMax);
        this.transform.localScale = new Vector3(startingScale.x, yScale, startingScale.z);
        onlyOneCollision = false;
    }
}
