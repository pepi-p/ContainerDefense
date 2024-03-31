using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Select : MonoBehaviour
{
    [SerializeField] private GameObject[] phases;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject[] rules;
    [Header("custom")]
    [SerializeField] private GameObject custom;
    [SerializeField] private Slider[] customSliders; // 0 : コンテナ数, 1 : スポナー数, 2 : 敵の数, 3 : 制限時間, 4 : スポーン間隔, 5 : 攻撃力, 6 : モード
    [SerializeField] private Text[] customValues;
    [SerializeField] private AudioClip selectSE;
    [SerializeField] private AudioClip enterSE;
    [SerializeField] private GameObject Info0, Info1;
    [SerializeField] private Text levelInfo, highScore, levelText;
    [SerializeField] private GameObject zenkontena, nodametassei;
    [Header("endless")]
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject endlessobj;
    [SerializeField] private GameObject endlessmessege;
    private AudioSource audioSource;
    private int phase = 0;
    private int mode = 0;
    private int selectLevel = 0;
    private bool endless = false;
    // Start is called before the first frame update
    void Start()
    {
        PhaseChange(0);
        custom.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        foreach(GameObject rule in rules) rule.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(phase == -1) {
            for(int i = 0; i < customSliders.Length; i++){
                float display = 0;
                var value = customSliders[i].value;
                if(i == 4 || i == 5) display = (int)(value * 10) / 10f;
                else display = Mathf.Round(value);
                customValues[i].text = display.ToString();
            }
        }
        if(phase == 2) {
            if(endless && mode == 0) {
                for(int i = 0; i < levels.Length; i++) levels[i].transform.localPosition = new Vector3(levels[i].transform.localPosition.x, -90 * (i - 1), levels[i].transform.localPosition.z);
                endlessobj.SetActive(endless);
            }
            else {
                for(int i = 0; i < levels.Length; i++) levels[i].transform.localPosition = new Vector3(levels[i].transform.localPosition.x, -90 * i, levels[i].transform.localPosition.z);
            }
        }
        else endlessobj.SetActive(false);
    }
    private void PhaseChange(int num) {
        phase = num;
        for(int i = 0; i < phases.Length; i++) {
            phases[i].SetActive(i == num);
        }
        if(phase <= 0) {
            foreach(GameObject rule in rules) rule.SetActive(false);
        }
        if(phase == 2 && mode == 0) endlessEnable();
    }
    private void SetLevel(int level) {
        int container = 1; // コンテナの数
        int spawner = 1; // スポナーの数
        int maxEnemy = 50; // 倒さなければいけない敵の数 ( mode = 0 限定 )
        int maxEnemies = 10; // 一度に存在できる敵の数
        float timelimit = 60; // 制限時間 ( mode = 1 限定 )
        float interval = 5; // 敵の湧く間隔
        float enemyAttack = 0.01f; // 敵の攻撃力
        manager.endless = false;
        switch(level) {
            case 1:
                container = 1;
                spawner = 1;
                maxEnemy = 20;
                maxEnemies = 10;
                timelimit = 40;
                interval = 2;
                enemyAttack = 0.015f;
                break;
            case 2:
                container = 2;
                spawner = 1;
                maxEnemy = 25;
                maxEnemies = 6;
                timelimit = 40;
                interval = 1f;
                enemyAttack = 0.005f;
                break;
            case 3:
                container = 2;
                spawner = 2;
                maxEnemy = 25;
                maxEnemies = 10;
                timelimit = 40;
                interval = 2.5f;
                enemyAttack = 0.004f;
                break;
            case 4:
                /*
                container = 2;
                spawner = 2;
                maxEnemy = 20;
                maxEnemies = 5;
                timelimit = 50;
                interval = 3;
                enemyAttack = 0.02f;
                */
                container = 3;
                spawner = 2;
                maxEnemy = 30;
                maxEnemies = 15;
                timelimit = 50;
                interval = 1.5f;
                enemyAttack = 0.006f;
                break;
            case 5:
                container = 3;
                spawner = 2;
                maxEnemy = 40;
                maxEnemies = 10;
                timelimit = 60;
                interval = 1f;
                enemyAttack = 0.01f;
                break;
            case 6:
                container = 3;
                spawner = 2;
                maxEnemy = 1;
                maxEnemies = 10;
                interval = 0.5f;
                enemyAttack = 0.015f;
                manager.endless = true;
                break;
        }
        manager.level = level;
        manager.containerCount = container;
        manager.spawnerCount = spawner;
        if(mode == 0) manager.maxEnemy = maxEnemy;
        else if(mode == 1) manager.timelimit = timelimit;
        manager.maxEnemies = maxEnemies;
        manager.spawnInterval = interval;
        manager.enemyAttack = enemyAttack;
        manager.mode = mode;
        SceneManager.LoadScene("MainGame");
    }
    private int GetRule(string name) {
        int result = 0;
        switch(name) {
            case "Button":
                result = 0;
                break;
            case "Button (1)":
                result = 1;
                break;
            case "Button (2)":
                result = 2;
                break;
        }
        return result;
    }
    public void OnClickStart() {
        PhaseChange(1);
    }
    public void OnClickLevel(int num) {
        SetLevel(num);
    }
    public void OnClickMode(int num) {
        mode = num;
        PhaseChange(2);
    }
    public void RuleDisplay(int num) {
        for(int i = 0; i < rules.Length; i++) {
            rules[i].SetActive(i == num);
        }
    }
    public void PhaseReturn() {
        phase--;
        PhaseChange(phase);
    }
    public void Custom() {
        PhaseChange(-1);
        custom.SetActive(true);
    }
    public void CustomStart() {
        mode = Mathf.RoundToInt(customSliders[6].value);
        manager.containerCount = Mathf.RoundToInt(customSliders[0].value);
        manager.spawnerCount = Mathf.RoundToInt(customSliders[1].value);
        if(mode == 0) manager.maxEnemy = Mathf.RoundToInt(customSliders[2].value);
        else if(mode == 1) manager.timelimit = Mathf.RoundToInt(customSliders[3].value);
        manager.maxEnemies = 10;
        manager.spawnInterval = Mathf.Round(customSliders[4].value * 10) / 10f;
        manager.enemyAttack = (Mathf.Round(customSliders[5].value * 10) / 10f) * 0.01f;
        manager.mode = mode;
        manager.level = -1;
        SceneManager.LoadScene("MainGame");
    }
    public void ReturnCustom() {
        PhaseChange(1);
        custom.SetActive(false);
    }
    public void Exit() {
        Application.Quit();
    }
    public void PlaySelectSE() {
        audioSource.PlayOneShot(selectSE);
    }
    public void PlayEnterSE() {
        audioSource.PlayOneShot(enterSE);
    }
    public void UpdateInfo(int level) {
        endlessmessege.SetActive(level == 6);
        selectLevel = level;
        levelText.text = level.ToString();
        Info0.SetActive(mode == 0);
        Info1.SetActive(mode == 1);
        var killCountHighScore = PlayerPrefs.GetInt("killCount" + mode.ToString() + level, 0);
        var headShotCountHighScore = PlayerPrefs.GetInt("headShotCount" + mode.ToString() + level, 0);
        var containerCountHighScore = PlayerPrefs.GetInt("containerCount" + mode.ToString() + level, 0);
        var containerHPHighScore = PlayerPrefs.GetInt("containerHP" + mode.ToString() + level, 0);
        var clearTimeHighScore = PlayerPrefs.GetFloat("clearTime" + mode.ToString() + level, 1000);
        var killCountString = killCountHighScore + " (" + headShotCountHighScore + ")";
        var containerCountString = containerCountHighScore.ToString() + " (" + containerHPHighScore.ToString("D2") + "%)";
        var clearTimeString = (int)(clearTimeHighScore / 60) + ":" + ((int)clearTimeHighScore % 60).ToString("D2");
        if(clearTimeHighScore == 1000) {
            killCountString = "- (-)";
            containerCountString = "- (-%)";
            clearTimeString = "-:--";
        }
        int container = 1; // コンテナの数
        int spawner = 1; // スポナーの数
        int maxEnemy = 50; // 倒さなければいけない敵の数 ( mode = 0 限定 )
        float timelimit = 60; // 制限時間 ( mode = 1 限定 )
        float interval = 5; // 敵の湧く間隔
        float enemyAttack = 0.01f; // 敵の攻撃力
        switch(level) {
            case 1:
                container = 1;
                spawner = 1;
                maxEnemy = 20;
                timelimit = 30;
                interval = 2;
                enemyAttack = 0.015f;
                break;
            case 2:
                container = 2;
                spawner = 1;
                maxEnemy = 25;
                timelimit = 35;
                interval = 1f;
                enemyAttack = 0.005f;
                break;
            case 3:
                container = 2;
                spawner = 2;
                maxEnemy = 25;
                timelimit = 40;
                interval = 2.5f;
                enemyAttack = 0.004f;
                break;
            case 4:
                /*
                container = 2;
                spawner = 2;
                maxEnemy = 20;
                timelimit = 50;
                interval = 3;
                enemyAttack = 0.02f;
                */
                container = 3;
                spawner = 2;
                maxEnemy = 30;
                timelimit = 50;
                interval = 1.5f;
                enemyAttack = 0.006f;
                break;
            case 5:
                container = 3;
                spawner = 2;
                maxEnemy = 40;
                timelimit = 60;
                interval = 1f;
                enemyAttack = 0.01f;
                break;
            case 6:
                container = 3;
                spawner = 2;
                interval = 1f;
                enemyAttack = 0.01f;
                break;
        }
        nodametassei.SetActive(false);
        zenkontena.SetActive(false);
        if(containerHPHighScore == 100) nodametassei.SetActive(true);
        else if(containerCountHighScore == container) zenkontena.SetActive(true);
        if(mode == 0 && level != 6) levelInfo.text = container + "\n" + spawner + "\n" + maxEnemy + "\n" + interval.ToString("f1") + "\n" + (enemyAttack * 100).ToString("f1");
        else if(mode == 0 && level == 6) levelInfo.text = container + "\n" + spawner + "\n" + "-" + "\n" + interval.ToString("f1") + "\n" + (enemyAttack * 100).ToString("f1");
        else if(mode == 1) levelInfo.text = container + "\n" + spawner + "\n" + (int)(timelimit / 60) + ":" + ((int)timelimit % 60).ToString("D2") + "\n" + interval.ToString("f1") + "\n" + (enemyAttack * 100).ToString("f1");
        highScore.text = "\n" + killCountString + "\n" + containerCountString + "\n" + clearTimeString + "\n\n";
    }
    public void HighScoreReset() {
        PlayerPrefs.SetInt("killCount" + mode.ToString() + selectLevel, 0);
        PlayerPrefs.SetInt("headShotCount" + mode.ToString() + selectLevel, 0);
        PlayerPrefs.SetInt("containerCount" + mode.ToString() + selectLevel, 0);
        PlayerPrefs.SetInt("containerHP" + mode.ToString() + selectLevel, 0);
        PlayerPrefs.SetFloat("clearTime" + mode.ToString() + selectLevel, selectLevel != 6 ? 1000 : 0);
        UpdateInfo(selectLevel);
    }
    private void endlessEnable() {
        bool _endless = true;
        for(int i = 1; i <= 5; i++) {
            var clearTimeHighScore = PlayerPrefs.GetFloat("clearTime0" + i, 1000);
            if(clearTimeHighScore == 1000) _endless = false;
        }
        endless = _endless;
    }
}
