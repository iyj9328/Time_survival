using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed;
    Rigidbody rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent <Rigidbody > ();
    }
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxisRaw("Horizontal") * speed;
        float v = Input.GetAxisRaw("Vertical") * speed;
        Move(h, v);
	}

    private void Move(float h, float v)
    {
        var newPosition = new Vector3(h, 0, v).normalized * speed * Time.deltaTime;
        var nextPosition = transform.position + newPosition;
        
        rb.MovePosition(nextPosition);
    }


}
