using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody rb;
    private Vector3 startPoint;
    // Start is called before the first frame update
    void Start()
    {
        var parent = this.transform.parent.gameObject;
        this.transform.parent = null;
        Destroy(parent);
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.transform.up * speed;
        startPoint = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(startPoint, this.transform.position) > 50) Destroy(this.gameObject);
    }
    void OnCollisionEnter(Collision other) {
        Destroy(this.gameObject);
    }
}
