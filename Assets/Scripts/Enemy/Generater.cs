using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generater : MonoBehaviour
{
    [SerializeField] private manager _manager;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] containers;
    [SerializeField] private EnemyController enemyPrefab;
    [SerializeField] private float interval;
    [SerializeField] private int DebugLog;
    private float coolTime;
    private int mode, maxEnemies;
    public Vector3 destination;
    
    private void Start()
    {
        Invoke("SetDestination", 0.1f);
        interval = manager.spawnInterval;
        coolTime = interval - 0.2f;
        mode = manager.mode;
        maxEnemies = manager.maxEnemies;
    }

    private void Update()
    {
        if(_manager.stop) {
            var childCount = this.transform.childCount;
            for(int i = 0; i < childCount; i++) {
                this.transform.GetChild(i).GetComponent<EnemyController>().stop = true;
            }
            return;
        }
        if(coolTime > interval && manager.enemyCount < maxEnemies) {
            if(((manager.spawnCount < manager.maxEnemy || manager.endless) && mode == 0) || (mode == 1 && manager.timelimit > 0)) Spawn();
            coolTime = 0;
        }
        coolTime += Time.deltaTime;
    }
    
    private void Setup()
    {
        int element = 0;
        float distance = 1000;
        for(int i = 0; i < containers.Length; i++) {
            if(containers[i].activeSelf) {
                var _distance = Vector3.Distance(containers[i].transform.position, this.transform.position);
                if(distance > _distance) {
                    element = i;
                    distance = _distance;
                }
            }
        }
        destination = containers[element].transform.position;
    }
    
    public void Spawn()
    {
        SetDestination();
        var enemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity, this.transform);
        enemy.SetPlayerPosition(player);
        enemy.SetContainerPosition(destination);
        manager.enemyCount++;
        manager.spawnCount++;
    }
    
    public void SetDestination()
    {
        Vector2 min = new Vector2(0, 1000);
        var _containerCount = containerCount();
        var _playerDistance = playerDistance();
        for(int i = 0; i < manager.containerCount; i++) {
            if(_manager.containerEnable[i]) {
                if(!(_containerCount == manager.containerCount && i == _playerDistance && Random.Range(0, 3) < 1)) {
                    var enemies = containers[i].GetComponent<Container>().GetEnemyCount();
                    if(min.y > enemies) {
                        min = new Vector2(i, enemies);
                    }
                }
            }
        }
        destination = containers[(int)min.x].transform.position;
        DebugLog = (int)min.x;
    }
    
    private int playerDistance()
    {
        var distance = 100f;
        int num = 0;
        for(int i = 0; i < manager.containerCount; i++) {
            var _distance = Vector3.Distance(player.position, containers[i].transform.position);
            if(distance > _distance) {
                num = i;
                distance = _distance;
            }
        }
        return num;
    }
    
    private int containerCount()
    {
        var result = 0;
        for(int i = 0; i < manager.containerCount; i++) {
            if(_manager.containerEnable[i]) result++;
        }
        return result;
    }
}
