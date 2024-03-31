using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float pitchRange = 0.1f;
    [SerializeField] private EnemyController enemyController;
    private Animator animator;
    public float recoil;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(recoil > 0) recoil = Mathf.Lerp(recoil, 0, 5 * Time.deltaTime);
    }
    
    private void OnAnimatorIK()
    {
        if(recoil > 0 && (enemyController.isPlayer || enemyController.isContainer)) {
            var rotation = Quaternion.Inverse(animator.GetBoneTransform(HumanBodyBones.Hips).rotation) * Quaternion.Euler(3.476f, 24.773f, -13.062f + recoil);
            animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(3.476f, 24.773f, -13.062f + recoil));
        }
    }
    
    private void Down()
    {
        manager.enemyCount--;
        manager.killCount++;
        Destroy(this.transform.parent.gameObject);
    }
    
    public void PlayFootstepSE() {
        audioSource.volume = Mathf.Sqrt(animator.GetFloat("speed"));
        audioSource.pitch = 1.2f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
    public void SetAgentSpeed(float speed) {
        enemyController.speed = speed;
    }
}
