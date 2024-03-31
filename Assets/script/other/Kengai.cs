using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kengai : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] SpriteRenderer[] sprites;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(player.transform.position, this.transform.position);
        var alpha = Mathf.Lerp(1, 0, distance - 3);
        foreach(SpriteRenderer sprite in sprites) {
            sprite.color = new Color(1, 1, 1, alpha);
        }
        animator.SetBool("isMove", distance < 2);
    }
}
