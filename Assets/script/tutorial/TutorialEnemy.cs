using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TutorialEnemyAnimation tutorialEnemyAnimation;

    private NavMeshAgent agent;
    private CapsuleCollider capsuleCollider;

    [SerializeField] private GameObject Muzzle;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shotSE;

    private Transform player;
    private Vector3 containerPosition = Vector3.zero;
    private Vector3 goal;
    private Vector3 deltaPos;

    private RaycastHit rayhit;
    private float coolDown = 0;
    private int HP = 5;
    private float speed;
    private float attack = 0.01f;
    public bool isPlayer, isContainer;

    public bool stop;
    public TutorialManager tutorialManager;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        deltaPos = this.transform.position;
        speed = 6;
        agent.speed = speed;
        attack = 0.01f;
        tutorialEnemyAnimation.tutorialManager = tutorialManager;
    }

    // Update is called once per frame
    void Update()
    {
        // 停止
        if(stop) {
            if(agent.updatePosition) {
                agent.updatePosition = false;
                agent.updateRotation = false;
                agent.updateUpAxis = false;
                animator.SetFloat("speed", 0);
            }
            return;
        }

        // プレイヤー検知
        var playerDistance = Vector3.Distance(player.position, this.transform.position);
        isPlayer = playerDistance < 2 && Mathf.Abs(player.position.y - this.transform.position.y) < 0.1f;

        // ray
        Ray ray;
        if(isPlayer) ray = new Ray(this.transform.position + Vector3.up * 1.6f, (player.position + Vector3.up * 1.6f) - (this.transform.position + Vector3.up * 1.6f)); 
        else ray = new Ray(this.transform.position + Vector3.up * 1.6f, containerPosition - (this.transform.position + Vector3.up * 1.6f));
        Physics.Raycast(ray, out rayhit, 100);
        //Debug.DrawRay(ray.origin, containerPosition - (this.transform.position + Vector3.up * 1.6f), Color.green);
        //Debug.DrawRay(this.transform.position + Vector3.up * 1.6f, containerPosition - (this.transform.position + Vector3.up * 1.6f), Color.green);

        // move
        var velocity = (deltaPos - this.transform.position).magnitude / Time.deltaTime;
        deltaPos = this.transform.position;
        var distance = Vector3.Distance(this.transform.position, containerPosition);
        isContainer = distance < 4.1f;
        if(isPlayer) goal = this.transform.position;
        else if(distance > 4) goal = containerPosition;
        else goal = this.transform.position;
        //if(distance < 10 && rayhit.collider.CompareTag("container")) agent.speed = Mathf.Lerp(3, 6, (distance - 5) / 5);
        agent.speed = Mathf.Lerp(agent.speed, speed, Time.deltaTime);
        animator.SetFloat("speed", velocity > 2 ? 1 : velocity / 2);
        agent.SetDestination(goal);
        //if(distance > 5.1f) animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, 2 * Time.deltaTime));
        
        // shot
        if(isPlayer) {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(player.position - this.transform.position), 10 * Time.deltaTime);
            if(rayhit.collider.CompareTag("Player")) {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, 20 * Time.deltaTime));
                if(animator.GetLayerWeight(1) > 0.9f) {
                    if(Time.time - coolDown > 2) {
                        StartCoroutine("Shot3");
                        coolDown = Time.time;
                    }
                }
            }
        }
        else animator.SetLayerWeight(1, 0);
        if(distance < 4.1f) {
            if(rayhit.collider.CompareTag("container")) {
                animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1, 20 * Time.deltaTime));
                if(this.gameObject.layer == 13) this.gameObject.layer = 0;
                if(animator.GetLayerWeight(1) > 0.9f) {
                    this.transform.rotation = Quaternion.LookRotation(new Vector3(containerPosition.x - this.transform.position.x, 0, containerPosition.z - this.transform.position.z));
                    if(Time.time - coolDown > 2) {
                        StartCoroutine("Shot3");
                        coolDown = Time.time;
                    }
                }
            }
        }
    }
    private IEnumerator Shot3() {
        var cooltime = Time.time;
        Shot();
        while(true) {
            if(Time.time - cooltime > 0.25f) {
                Shot();
                break;
            }
            yield return null;
        }
        while(true) {
            if(Time.time - cooltime > 0.5f) {
                Shot();
                break;
            }
            yield return null;
        }
    }
    private void Shot() {
        audioSource.PlayOneShot(shotSE);
        if(rayhit.collider.CompareTag("container")) rayhit.collider.GetComponent<TutorialContainer>().HP -= attack;
        if(rayhit.collider.CompareTag("Player")) player.GetComponent<TutorialPlayer>().HP -= 0.01f;
        Instantiate(Bullet, Muzzle.transform.position, Quaternion.LookRotation(this.transform.forward));
        Instantiate(MuzzleFlash, Muzzle.transform.position, Quaternion.LookRotation(this.transform.forward), Muzzle.transform);
        tutorialEnemyAnimation.recoil = 5;
    }
    public void Hit(Vector3 hitPoint) {
        HP--;
        if(hitPoint.y - this.transform.position.y > 1.5f) HP = 0;
        if(HP <= 0) {
            stop = true;
            capsuleCollider.enabled = false;
            animator.SetLayerWeight(1, 0);
            animator.Play("down", 0);
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
        else {
            agent.speed = 0;
            //animator.SetLayerWeight(1, 1 - animator.GetLayerWeight(1) * 0.5f);
        }
    }
    public void SetPlayerPosition(Transform playerTransform) {
        player = playerTransform;
    }
    public void SetContainerPosition(Vector3 position) {
        containerPosition = position;
    }
}
