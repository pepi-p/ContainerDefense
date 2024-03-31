using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class TutorialPlayer : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angleSpeed;
    
    [SerializeField] private PostProcessVolume volume;

    [Header("Characters")]
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject CameraParent;
    [SerializeField] private GameObject stageHanten;
    [SerializeField] private GameObject Light;
    [SerializeField] private GameObject Torso;
    [SerializeField] private GameObject syoujun;
    [SerializeField] private Image[] syoujuns;
    [Header("Guns")]
    [SerializeField] private GameObject Gun;
    [SerializeField] private GameObject Bullet;
    [SerializeField] private GameObject Muzzle;
    [SerializeField] private GameObject MuzzleFlash;
    [Header("UI")]
    [SerializeField] private Image[] AmmoImg;
    [SerializeField] private Text AmmoText;
    [SerializeField] private Image playerHPbar;
    [Header("Scripts")]
    [SerializeField] private TutorialManager tutorialManager;
    [Header("SE")]
    [SerializeField] private AudioSource audioSourceMuzzle;
    [SerializeField] private AudioSource audioSourceNormal;
    [SerializeField] private AudioSource audioSourceHit;
    [SerializeField] private AudioClip kamaeSE;
    [SerializeField] private AudioClip shotSE;
    [SerializeField] private AudioClip hitSE;
    [SerializeField] private GameObject audioHit;
    [SerializeField] private GameObject hitSpark;
    private Animator animator;
    private CharacterController controller;
    private Camera mainCamera;
    private float rotationX, recoil; // X軸回転(視点上下) Y軸回転(視点左右) X軸回転の反動分
    private float torsoWeight;
    private float coolDown = 0;
    private int ammo, maxAmmo;
    private Color ammoTrue = new Color(1, 1, 1, 180f / 255f), ammoFalse = new Color(0, 0, 0, 0);
    private Quaternion lookRotation;
    private RaycastHit rayhit, cameraHit;
    private LayerMask cameraMask = ~(1 << 7);

    public bool reload = false;
    public bool reloaded = false;
    public bool shotAble = false;
    public bool reloadAble = false;
    public float rotationY;
    public float HP = 1;

    // scripts
    private TutorialPlayerAnimation tutorialPlayerAnimation;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        tutorialPlayerAnimation = character.GetComponent<TutorialPlayerAnimation>();
        animator = character.GetComponent<Animator>();
        mainCamera = Camera.main;
        lookRotation = this.transform.rotation;

        maxAmmo = 30;
        ammo = maxAmmo;

        rotationY = -118.56f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!shotAble) ammo = maxAmmo;

        var managerStop = tutorialManager.stop;

        var horizontal = !managerStop ? Input.GetAxis("Horizontal") : 0;
        var vertical = !managerStop ? Input.GetAxis("Vertical") : 0;
        var inputVector = new Vector3(horizontal, 0, vertical).normalized;
        var localInputVector = this.transform.right * inputVector.x + this.transform.forward * inputVector.z;
        var inputAngle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;
        var inputMagnitude = new Vector2(horizontal, vertical).magnitude;
        if(inputMagnitude > 1) inputMagnitude = 1;

        var mouseX = !managerStop ? Input.GetAxis("Mouse X") : 0;
        var mouseY = !managerStop ? Input.GetAxis("Mouse Y") : 0;

        var aim = !managerStop ? Input.GetMouseButton(1) : false;
        var shot = !managerStop ? Input.GetMouseButton(0) : false;

        if(!shotAble) aim = false;

        rotationY += !managerStop ? mouseX * angleSpeed * Time.deltaTime : 0;
        rotationX += !managerStop ? mouseY * angleSpeed * Time.deltaTime : 0;
        if(aim) {
            if(rotationX < -40f) rotationX = -40f;
            if(rotationX > 50) rotationX = 50;
        }
        else {
            if(rotationX < -40) rotationX = -40;
            if(rotationX > 40) rotationX = 40;
        }

        if(recoil > 0) recoil = Mathf.Lerp(recoil, 0, 5 * Time.deltaTime);

        if(Input.GetMouseButtonDown(1) && shotAble) audioSourceNormal.PlayOneShot(kamaeSE);

        Vignette vignette;
        volume.profile.TryGetSettings<Vignette>(out vignette);
        vignette.intensity.value = aim ? 0.4f : 0.3f;

        // raycast
        Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayhit, 100f);
        var isHit = Physics.Raycast(CameraParent.transform.position, mainCamera.transform.position - CameraParent.transform.position, out cameraHit, 2.73f + 0.5f, cameraMask);
        //Debug.DrawRay(CameraParent.transform.position, mainCamera.transform.position - CameraParent.transform.position, Color.green, 2.73f);

        CameraParent.transform.position = Vector3.Lerp(CameraParent.transform.position, this.transform.position + new Vector3(0, 1.87f, 0), 10 * Time.deltaTime);
        CameraParent.transform.rotation = Quaternion.Euler(-rotationX - recoil, rotationY, 0);

        if(inputMagnitude > 0 && !aim) lookRotation = Quaternion.Euler(0, 90 - inputAngle + rotationY, 0);
        else if(aim) lookRotation = Quaternion.Euler(0, rotationY, 0);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, lookRotation, 10 * Time.deltaTime);
        if(!managerStop) Light.transform.rotation = Quaternion.Euler(50, -30 + rotationY, 0);
        
        var inputZ = this.transform.forward * inputMagnitude;
        if(aim) inputZ = localInputVector;
        controller.Move(inputZ * Time.deltaTime * speed  + Vector3.down * 100);

        animator.SetFloat("speed", inputMagnitude);
        torsoWeight = aim ? 1 : 0;
        animator.SetLayerWeight(1, Mathf.Sqrt(torsoWeight));
        stageHanten.SetActive(!aim);

        // syoujun
        syoujun.SetActive(torsoWeight > 0.5f);
        syoujuns[0].rectTransform.localPosition = new Vector2(0, Mathf.Lerp(syoujuns[0].rectTransform.localPosition.y, 30, 5 * Time.deltaTime));
        syoujuns[1].rectTransform.localPosition = new Vector2(0, Mathf.Lerp(syoujuns[1].rectTransform.localPosition.y, -30, 5 * Time.deltaTime));
        syoujuns[2].rectTransform.localPosition = new Vector2(Mathf.Lerp(syoujuns[2].rectTransform.localPosition.x, 30, 5 * Time.deltaTime), 0);
        syoujuns[3].rectTransform.localPosition = new Vector2(Mathf.Lerp(syoujuns[3].rectTransform.localPosition.x, -30, 5 * Time.deltaTime), 0);
        if(rayhit.collider.CompareTag("Enemy")) for(int i = 0; i < 4; i++) syoujuns[i].color = Color.red;
        else if(rayhit.collider.CompareTag("container")) for(int i = 0; i < 4; i++) syoujuns[i].color = Color.cyan;
        else for(int i = 0; i < 4; i++) syoujuns[i].color = Color.white;

        tutorialPlayerAnimation.IKWeight = torsoWeight;
        //playerAnimation.angle = rotationX + recoil <= 15 ? (rotationX + recoil) * 0.8f : 6.2f * Mathf.Sqrt(rotationX + recoil) - 12;
        if(rotationX + recoil > 15) tutorialPlayerAnimation.angle = 6.2f * Mathf.Sqrt(rotationX + recoil) - 12;
        else if(rotationX + recoil < -25) tutorialPlayerAnimation.angle = -(1 / 38.44f) * Mathf.Pow(rotationX + recoil + 9.624f, 2) -13.8496f;
        else tutorialPlayerAnimation.angle = (rotationX + recoil) * 0.8f;

        if(!managerStop) {
            if(torsoWeight > 0.5f) {
                mainCamera.transform.parent = Torso.transform;
                //[-2.648f, -1.19258f, 0.5012466f] [-24.225f, 101.329f, -7.057f] >> [0.3275235f, 1.579207f, -0.7157664f] [1.199f, -0.012f, 0.005f] 20 >> -0.1
                //mainCamera.transform.localPosition = Vector3.Lerp(beforeCameraPosition, new Vector3(0, 0, -0.7157664f - (rotationX * 0.8f) / 200), torsoWeight);
                //mainCamera.transform.localRotation = Quaternion.Lerp(beforeCameraRotation, Quaternion.Inverse(Quaternion.Euler(1.199f, -0.012f, 0.005f)), torsoWeight);
                mainCamera.transform.localPosition = new Vector3(0, 0, -0.7157664f - (rotationX * 0.8f) / 200);
                mainCamera.transform.localRotation = Quaternion.Inverse(Quaternion.Euler(1.199f, -0.012f, 0.005f));

                //rotationX = 0;

                // 上半身の回転の初期値 : [3.4758f, 24.773f, -13.062f]
                //Torso.transform.rotation = Quaternion.Euler(3.4758f, 24.773f, -13.062f) * Quaternion.Euler(rotationX, 0, 0);
                Torso.transform.localRotation = Quaternion.Inverse(Quaternion.Euler(1.199f + rotationX + recoil, -0.012f, 0.005f));
            }
            else if(isHit) {
                mainCamera.transform.parent = CameraParent.transform;
                //mainCamera.transform.position = cameraHit.point;
                var hitDistance = Vector3.Distance(cameraHit.point, CameraParent.transform.position);
                if(hitDistance > 0.5f) mainCamera.transform.localPosition = new Vector3(0, 0, -hitDistance + 0.5f);
            }
            else {
                mainCamera.transform.parent = CameraParent.transform;
                // [0.3275235f, -0.2907927f, -0.7157664f] [1.199f, -0.012f, 0.005f] >> [0, 0, -2.73f] [14, 0, 0]
                //mainCamera.transform.localPosition = Vector3.Lerp(new Vector3(0, 0, -2.73f), new Vector3(0.33f, -0.3564f, -0.56767f), torsoWeight);
                //mainCamera.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(14, 0, 0), Quaternion.Euler(1.2f, 0.055f, 0.005f), torsoWeight);
                mainCamera.transform.localPosition = new Vector3(0, 0, -2.73f);
                mainCamera.transform.localRotation = Quaternion.Euler(14, 0, 0);
            }
        }
        // [0.45, 1.71, 2.08] [-1.357f, 0, 0]

        // reload
        if(Input.GetKeyDown(KeyCode.R) && !reload && reloadAble) StartCoroutine("Reload");

        // shot
        if(shot && ammo > 0 && !reload && torsoWeight > 0.9f && coolDown > 0.1f) {
            coolDown = 0;
            Shot();
        }
        coolDown += Time.deltaTime;

        // ammo
        for(int i = 0; i < AmmoImg.Length; i++) {
            if(i < ammo) AmmoImg[i].color = ammoTrue;
            else AmmoImg[i].color = ammoFalse;
        }
        AmmoText.text = ammo.ToString("D2") + " / -";
        if(reloaded) {
            if(ammo == 0) ammo = maxAmmo;
            else ammo = maxAmmo + 1;
            reloaded = false;
        }

        // HPbar
        playerHPbar.rectTransform.localPosition = new Vector3(-250 + ((HP * 500 * 0.5f)), 0, 0);
        playerHPbar.rectTransform.sizeDelta = new Vector2(500 * HP, 20);
    }
    private void Shot() {
        ammo--;
        audioSourceMuzzle.PlayOneShot(shotSE);
        syoujuns[0].rectTransform.localPosition = new Vector2(0, 45);
        syoujuns[1].rectTransform.localPosition = new Vector2(0, -45);
        syoujuns[2].rectTransform.localPosition = new Vector2(45, 0);
        syoujuns[3].rectTransform.localPosition = new Vector2(-45, 0);
        audioHit.transform.position = rayhit.point;
        if(rayhit.collider.CompareTag("Enemy") || rayhit.collider.CompareTag("container")) {
            if(rayhit.collider.CompareTag("Enemy") && rayhit.point.y - rayhit.collider.gameObject.transform.position.y > 1.5f) {
                audioSourceHit.pitch = 2;
                audioSourceHit.volume = 0.5f;
            }
            else {
                audioSourceHit.pitch = 1.2f;
                audioSourceHit.volume = 0.3f;
                Instantiate(hitSpark, audioHit.transform.position, Quaternion.LookRotation(rayhit.normal), audioHit.transform);
            }
            audioSourceHit.PlayOneShot(hitSE);
        }
        if(rayhit.collider.CompareTag("Enemy")) {
            rayhit.collider.GetComponent<TutorialEnemy>().Hit(rayhit.point);
            //if(rayhit.point.y - rayhit.collider.gameObject.transform.position.y > 1.5f) tutorialManager.headShotCount++;
        }
        if(rayhit.collider.CompareTag("container")) {
            rayhit.collider.GetComponent<TutorialContainer>().HP -= 0.01f;
        }
        GameObject bullet = Instantiate(Bullet, Muzzle.transform.position, Quaternion.LookRotation(rayhit.point - Muzzle.transform.position)) as GameObject;
        Instantiate(MuzzleFlash, Muzzle.transform.position, Quaternion.LookRotation(this.transform.forward), Muzzle.transform);
        recoil = 2.5f;
    }
    IEnumerator Reload() {
        reload = true;
        animator.Play("reload", 2);
        if(Input.GetMouseButton(1)) {
            animator.SetLayerWeight(2, 1);
            yield break;
        }
        var flame = 1/ Time.deltaTime;
        for(float i = 0; i <= flame / 4; i++) {
            animator.SetLayerWeight(2, Mathf.Sqrt(i / (flame / 4)));
            yield return null;
        }
    }
}

