using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody rb;
    private Vector3 startPoint;
    
    private void Start()
    {
        var parent = this.transform.parent.gameObject;
        this.transform.parent = null;
        Destroy(parent);
        rb = GetComponent<Rigidbody>();
        rb.velocity = this.transform.up * speed;
        startPoint = this.transform.position;
    }

    private void Update()
    {
        if(Vector3.Distance(startPoint, this.transform.position) > 50) Destroy(this.gameObject);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        Destroy(this.gameObject);
    }
}
