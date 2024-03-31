using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private GameObject leftHand, magazine, magazineParent;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioClip[] reloadSE;
    [SerializeField] private AudioSource audioSourceReloadSE;
    [SerializeField] private float pitchRange = 0.1f;

    private Animator animator;
    private Vector3 vector;
    private Quaternion beforeSpineRotation;
    private bool down = false;

    public float IKWeight;
    public float angle;

    private void Start()
    {
        animator = GetComponent<Animator>();
        beforeSpineRotation = animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
    }

    private void Update()
    {
        if(playerMove.HP < 0 && !down) {
            down = true;
            animator.Play("down", 0);
        }
        if(Input.GetMouseButton(1)) beforeSpineRotation = animator.GetBoneTransform(HumanBodyBones.Spine).rotation;
    }
    private void OnAnimatorIK()
    {
        // [0, -90.067f, 8.248f]
        vector = new Vector3(-0.3f * (angle + 3) - 0.4f, angle <= 20 ? (((angle + 32) / 64f) * 8) : 6.5f, angle + 3);
        var rotation = Quaternion.Inverse(animator.GetBoneTransform(HumanBodyBones.Hips).rotation) * Quaternion.Euler(vector.x, animator.GetBoneTransform(HumanBodyBones.Spine).eulerAngles.y + vector.y, vector.z);
        if(IKWeight > 0.1f) animator.SetBoneLocalRotation(HumanBodyBones.Spine, rotation);
    }
    
    private void UnReload()
    {
        if(playerMove.reload) playerMove.reload = false;
    }
    
    private void MagazineParent0()
    {
        magazine.transform.parent = leftHand.transform;
    }
    
    private void MagazineParent1()
    {
        magazine.transform.parent = magazineParent.transform;
        magazine.transform.localPosition = new Vector3(-0.0002867314f, 0.03059091f, 0.01773768f);
        magazine.transform.localRotation = Quaternion.Euler(-180f, 0f, 0f);
        magazine.transform.localScale = new Vector3(1, 1, 1);
    }
    
    private void EndReload()
    {
        StartCoroutine(EndReloadCoroutine());
    }
    
    private IEnumerator EndReloadCoroutine()
    {
        if(IKWeight < 0.1f) {
            var flame = 1 / Time.deltaTime;
            for(float i = 0; i <= flame / 4; i++) {
                animator.SetLayerWeight(2, 1 - (i / (flame / 4)));
                yield return null;
            }
        }
        else {
            animator.SetLayerWeight(2, 0);
            playerMove.reload = false;
        }
        playerMove.reloaded = true;
    }
    
    public void PlayFootstepSE()
    {
        audioSource.volume = Mathf.Sqrt(animator.GetFloat("speed")) * 0.5f;
        audioSource.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
    
    public void PlayReloadSE(int num)
    {
        audioSourceReloadSE.PlayOneShot(reloadSE[num]);
    }
}
