using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class manager : MonoBehaviour
{
    [SerializeField] PlayerMove playerMove;
    [SerializeField] GameObject[] containers;
    [SerializeField] GameObject[] spawners;
    [SerializeField] GameObject[] HPbars;
    [SerializeField] Text modeText;
    [SerializeField] GameObject menu;
    [SerializeField] Text angleSpeedText;
    public static int enemyCount, killCount, spawnCount; // 場に存在している敵の数, キル数, 湧いた敵の総数
    public static int containerCount = 3, spawnerCount = 1; // コンテナの初期数, スポナーの初期数
    public static int level = 1; // レベル
    public static int mode = 0; // ゲームモード ( 0 : 敵数制, 1 : 時間制 )
    public static float spawnInterval = 1; // スポーン間隔
    public static int maxEnemy = 40; // 敵の倒さなければいけない数 ( mode = 0 限定)
    public static float timelimit = 100; // 制限時間 ( mode = 1 限定 )
    public static int maxEnemies = 10; // 一度に存在できる敵の上限
    public static float enemyAttack = 0.01f; // 敵の攻撃力
    public static bool endless = false; // エンドレスモード

    public bool stop = false;
    public bool[] containerEnable;
    public int[] countainerAroundEnemy;
    public int headShotCount = 0;

    private Generater[] generaters = new Generater[spawnerCount];
    private bool containerDestroy;
    private float clearTime;
    private bool openMenu = false;

    [Header("Result")]
    [SerializeField] private Animator resultAnimator;
    [SerializeField] private Image resultImg, resultBackGround;
    [SerializeField] private Sprite win, lose;
    [SerializeField] private Text killText;
    [SerializeField] private Text containerCountText;
    [SerializeField] private Text clearTimeText;
    [SerializeField] private Text killTextHighScore;
    [SerializeField] private Text containerCountTextHighScore;
    [SerializeField] private Text clearTimeTextHighScore;
    [SerializeField] private GameObject resultMatrix;
    [SerializeField] private GameObject allSuccessTxt, noDamageTxt;
    [SerializeField] private GameObject sippai_title_text;
    private int killCountHighScore;
    private int headShotCountHighScore;
    private Vector2 containerCountHighScore;
    private float clearTimeHighScore;
    
    private void Start()
    {        
        Application.targetFrameRate = 120;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        enemyCount = 0;
        killCount = 0;
        spawnCount = 0;
        for(int i = 0; i < containers.Length; i++) {
            var active = i < containerCount;
            containers[i].SetActive(active);
            HPbars[i].SetActive(active);
        }
        for(int i = 0; i < spawners.Length; i++) {
            spawners[i].SetActive(i < spawnerCount);
        }
        containerEnable = new bool[containers.Length];
        countainerAroundEnemy = new int[containerCount];
        for(int i = 0; i < containerEnable.Length; i++) containerEnable[i] = true;
        for(int i = 0; i < spawnerCount; i++) generaters[i] = spawners[i].GetComponent<Generater>();
        if(level > 0) GetHighScore();
        
        angleSpeedText.text = PlayerMove.angleSpeed.ToString("F1");
    }

    private void Update()
    {
        if(!stop) {
            if(Input.GetKey(KeyCode.T)) killCount++;
            if(Input.GetKey(KeyCode.Y)) clearTime++;
            clearTime += Time.deltaTime;
            if(timelimit > 0) timelimit -= Time.deltaTime;
            if(endless) modeText.text = "キル数 : " + killCount + "  経過時間  " + ((int)(clearTime / 60)).ToString() + ":" + ((int)(clearTime % 60)).ToString("D2");
            else if(mode == 0) modeText.text = "残敵 : " + (maxEnemy - killCount).ToString("D2") + " / " + maxEnemy.ToString("D2");
            else if(mode == 1) modeText.text = "残り時間  " + ((int)(timelimit / 60)).ToString() + ":" + ((int)(timelimit % 60)).ToString("D2");

            if(((mode == 0 && killCount == maxEnemy) || (mode == 1 && timelimit <= 0)) && !endless) StartCoroutine("end", 1);
            else if((containerDestroy || playerMove.HP < 0) && !endless) StartCoroutine("end", -1);
            else if((containerDestroy || playerMove.HP < 0) && endless) StartCoroutine("end", 1);
            if(Input.GetKeyDown(KeyCode.Escape)) {
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
    }
    
    public void ReturnTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Title");
    }
    
    public void ContainerDestory(int id)
    {
        containerEnable[id] = false;
        /*foreach(Generater generater in generaters) {
            generater.SetDestination(containerEnable);
        }*/
        bool destroy = true;
        for(int i = 0; i < containerCount; i++) {
            if(containerEnable[i]) {
                destroy = false;
                break;
            }
        }
        containerDestroy = destroy;
    }
    
    private int containerEnableCount()
    {
        int result = 0;
        for(int i = 0; i < containerCount; i++) if(containerEnable[i]) result++;
        return result;
    }
    
    private int GetHP()
    {
        float result = 0;
        for(int i = 0; i < containerCount; i++) {
            result += Mathf.Clamp01(containers[i].GetComponent<Container>().HP);
        }
        result /= containerCount;
        return (int)(result * 100);
    }
    
    private IEnumerator end(int result)
    {
        stop = true;
        resultBackGround.rectTransform.sizeDelta = new Vector2(1920, 720);
        float flame = 1 / Time.deltaTime;
        for(float i = 0; i <= flame * 0.5f; i++) {
            resultBackGround.color = new Color(0, 0, 0, (200f / 255f) * (i / (flame * 0.5f)));
            yield return null;
        }
        resultBackGround.color = new Color(0, 0, 0, 200f / 255f);
        resultImg.sprite = result > 0 ? win : lose;
        resultAnimator.SetInteger("result", result);
        if(result == -1) {
            sippai_title_text.SetActive(true);
            while(true) {
                if(Input.GetKeyDown(KeyCode.Escape)) break;
                yield return null;
            }
            ReturnTitle();
            yield break;
        }
        yield return new WaitForSeconds(1);
        resultAnimator.SetInteger("result", 0);
        yield return new WaitForSeconds(0.5f);
        killText.text = killCount + "（" + headShotCount + "）";
        clearTimeText.text = (int)(clearTime / 60) + ":" + ((int)clearTime % 60).ToString("D2");
        var _containerCount = 0;
        for(int i = 0; i < containerCount; i++) if(containerEnable[i]) _containerCount++;
        containerCountText.text = _containerCount.ToString() + "（" + GetHP().ToString("D2") + "%）";
        noDamageTxt.SetActive(GetHP() == 100);
        allSuccessTxt.SetActive(containerEnableCount() == containerCount && GetHP() != 100 && containerCount > 1);

        if(level > 0) {
            if(killCount > killCountHighScore) {
                PlayerPrefs.SetInt("killCount" + mode.ToString() + level, killCount);
                PlayerPrefs.SetInt("headShotCount" + mode.ToString() + level, headShotCount);
            }
            if(_containerCount >= containerCountHighScore.x && GetHP() > containerCountHighScore.y) {
                PlayerPrefs.SetInt("containerCount" + mode.ToString() + level, _containerCount);
                PlayerPrefs.SetInt("containerHP" + mode.ToString() + level, GetHP());
            }
            if(clearTime < clearTimeHighScore && !endless) PlayerPrefs.SetFloat("clearTime" + mode.ToString() + level, clearTime);
            else if(clearTime > clearTimeHighScore && endless) PlayerPrefs.SetFloat("clearTime" + mode.ToString() + level, clearTime);
            GetHighScore();
            killTextHighScore.text = killCountHighScore + "（" + headShotCountHighScore + "）";
            clearTimeTextHighScore.text = (int)(clearTimeHighScore / 60) + ":" + ((int)clearTimeHighScore % 60).ToString("D2");
            containerCountTextHighScore.text = containerCountHighScore.x.ToString() + "（" + ((int)containerCountHighScore.y).ToString("D2") + "%）";
        }

        resultBackGround.rectTransform.sizeDelta = new Vector2(1920, 680);
        resultMatrix.SetActive(true);
        while(true) {
            if(Input.GetKeyDown(KeyCode.Escape)) break;
            yield return null;
        }
        ReturnTitle();
    }
    
    private void GetHighScore()
    {
        killCountHighScore = PlayerPrefs.GetInt("killCount" + mode.ToString() + level, 0);
        headShotCountHighScore = PlayerPrefs.GetInt("headShotCount" + mode.ToString() + level, 0);
        containerCountHighScore = new Vector2(PlayerPrefs.GetInt("containerCount" + mode.ToString() + level, 0), PlayerPrefs.GetInt("containerHP" + mode.ToString() + level, 0));
        clearTimeHighScore = PlayerPrefs.GetFloat("clearTime" + mode.ToString() + level, 1000);
    }

    public void SetAngleSpeed(Slider slider)
    {
        var value = (float)(Math.Round(slider.value * 10) / 10.0);
        PlayerMove.angleSpeed = value;
        angleSpeedText.text = value.ToString("F1");
    }
}
