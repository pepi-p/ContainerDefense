using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyAnimation : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private float pitchRange = 0.1f;
    public TutorialManager tutorialManager;
    private Animator animator;
    public float recoil;
    private float offset;
    private TutorialEnemy tutorialEnemy;

    private void Start()
    {
        animator = GetComponent<Animator>();
        tutorialEnemy = this.transform.parent.GetComponent<TutorialEnemy>();
    }

    private void Update()
    {
        if(recoil > 0) recoil = Mathf.Lerp(recoil, 0, 5 * Time.deltaTime);
    }
    
    void OnAnimatorIK()
    {
        if(recoil > 0 && (tutorialEnemy.isContainer || tutorialEnemy.isPlayer)) {
            var rotation = Quaternion.Inverse(animator.GetBoneTransform(HumanBodyBones.Hips).rotation) * Quaternion.Euler(3.476f, 24.773f, -13.062f + recoil);
            animator.SetBoneLocalRotation(HumanBodyBones.Spine, Quaternion.Euler(3.476f, 24.773f, -13.062f + recoil));
        }
    }
    
    void Down()
    {
        tutorialManager.killCount++;
        Destroy(this.transform.parent.gameObject);
    }
    
    public void PlayFootstepSE() 
    {
        audioSource.volume = Mathf.Sqrt(animator.GetFloat("speed"));
        audioSource.pitch = 1.1f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
}