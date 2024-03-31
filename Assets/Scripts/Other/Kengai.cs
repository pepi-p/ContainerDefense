using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kengai : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer[] sprites;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var distance = Vector3.Distance(player.transform.position, this.transform.position);
        var alpha = Mathf.Lerp(1, 0, distance - 3);
        foreach(SpriteRenderer sprite in sprites) {
            sprite.color = new Color(1, 1, 1, alpha);
        }
        animator.SetBool("isMove", distance < 2);
    }
}
