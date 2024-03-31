using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] TutorialGenerater tutorialGenerater;
    [SerializeField] TutorialPlayer tutorialPlayer;
    [SerializeField] TutorialContainer tutorialContainer;
    [SerializeField] GameObject GUI, PlayerUI;
    [SerializeField] GameObject player;
    [SerializeField] GameObject directionalLight;
    [Header("UI")]
    [SerializeField] GameObject HPbar;
    [SerializeField] GameObject playerHPbar;
    [SerializeField] GameObject guns;
    [SerializeField] GameObject menu;
    [Header("tutorial")]
    [SerializeField] GameObject[] tutorials;
    [SerializeField] Text jissya;
    private int tutorialNumber = 0;
    public bool stop = true;
    public int killCount = 0;

    private bool openMenu = false;
    private Vector3 beforePlayerTransformPosition;
    private Quaternion beforePlayerTransformRotation;
    private Vector3 beforeCameraTransformPosition;
    private Quaternion beforeCameraTransformRotation;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        TutorialDisplay(0);
        GUI.SetActive(true);
        PlayerUI.SetActive(false);
        beforePlayerTransformPosition = player.transform.position;
        beforePlayerTransformRotation = player.transform.rotation;
        beforeCameraTransformPosition = Camera.main.transform.position;
        beforeCameraTransformRotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(!stop && Input.GetKeyDown(KeyCode.Escape)){
            openMenu = !openMenu;
            menu.SetActive(openMenu);
            if(openMenu) {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    public void ReturnTitle() {
        menu.SetActive(false);
        openMenu = false;
        Time.timeScale = 1;
        GUI.SetActive(true);
        PlayerUI.SetActive(false);
        stop = true;
        killCount = 0;
        Camera.main.transform.parent = null;
        player.transform.position = beforePlayerTransformPosition;
        player.transform.rotation = beforePlayerTransformRotation;
        Camera.main.transform.position = beforeCameraTransformPosition;
        Camera.main.transform.rotation = beforeCameraTransformRotation;
        directionalLight.transform.rotation = Quaternion.Euler(50, -12.3f, 0);
        tutorialPlayer.rotationY = -118.56f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void StartTutorialClick() {
        StartCoroutine("StartTutorial");
    }
    private void TutorialDisplay(int num) {
        for(int i = 0; i < tutorials.Length; i++) {
            tutorials[i].SetActive(i == num);
        }
    }
    IEnumerator StartTutorial() {
        // setup
        GUI.SetActive(false);
        HPbar.SetActive(false);
        playerHPbar.SetActive(false);
        guns.SetActive(false);
        tutorialPlayer.shotAble = false;
        tutorialPlayer.reloadAble = false;
        tutorialContainer.HP = 1;
        tutorialGenerater.generate = false;
        var flame = 1 / Time.deltaTime;
        var cameraBeforePosition = Camera.main.transform.position;
        var cameraBeforeRotation = Camera.main.transform.rotation;
        for(float i = 1 ; i <= flame * 0.5f; i++) {
            Camera.main.transform.position = Vector3.Lerp(cameraBeforePosition, new Vector3(5.377806f, 1.87f, 19.70515f), i / (flame * 0.5f));
            Camera.main.transform.rotation = Quaternion.Lerp(cameraBeforeRotation, Quaternion.Euler(14, -118.56f, 0), i / (flame * 0.5f));
            yield return null;
        }
        stop = false;
        PlayerUI.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // 移動・カメラ回転
        TutorialDisplay(0);
        while(true) {
            yield return null;
            if(Input.GetKeyDown(KeyCode.Space)) break;
            if(stop) yield break;
        }

        // 銃のUIの説明
        Time.timeScale = 0;
        TutorialDisplay(1);
        guns.SetActive(true);
        //for(int i = 0; i < flame * 5; i++) yield return null;
        while(true) {
            yield return null;
            if(Input.GetKeyDown(KeyCode.Space)) break;
            if(stop) yield break;
        }
        Time.timeScale = 1;

        // 銃の撃ち方
        TutorialDisplay(2);
        tutorialPlayer.shotAble = true;
        tutorialPlayer.reloadAble = true;
        while(true) {
            yield return null;
            if(Input.GetKeyDown(KeyCode.Space)) break;
            if(stop) yield break;
        }
        //yield return new WaitForSeconds(5);

        // コンテナのUI
        Time.timeScale = 0;
        TutorialDisplay(3);
        HPbar.SetActive(true);
        while(true) {
            yield return null;
            if(Input.GetKeyDown(KeyCode.Space)) break;
            if(stop) yield break;
        }

        //プレイヤーのHPの説明
        TutorialDisplay(4);
        playerHPbar.SetActive(true);
        while(true) {
            yield return null;
            if(Input.GetKeyDown(KeyCode.Space)) break;
            if(stop) yield break;
        }
        Time.timeScale = 1;

        // 実際にやってみる
        TutorialDisplay(5);
        tutorialGenerater.generate = true;
        while(true) {
            jissya.text = "敵を撃って\nコンテナを防衛してみよう\n（" + killCount.ToString() + "/5）";
            yield return null;
            if(killCount > 4) break;
            if(stop) yield break;
        }
        tutorialGenerater.generate = false;
        yield return new WaitForSeconds(0.5f);

        // チュートリアル終了
        TutorialDisplay(6);
    }
}
