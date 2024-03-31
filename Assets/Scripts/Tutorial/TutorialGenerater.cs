using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGenerater : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject container;
    [SerializeField] private TutorialEnemy enemyPrefab;
    [SerializeField] private float interval;
    private float coolTime;
    private int mode;
    public Vector3 destination;
    public bool generate = false;
    
    private void Start()
    {
        destination = container.transform.position;
        coolTime = interval - 1;
        mode = 0;
    }

    private void Update()
    {
        if(tutorialManager.stop) {
            var childCount = this.transform.childCount;
            for(int i = 0; i < childCount; i++) {
                Destroy(this.transform.GetChild(i).gameObject);
            }
            return;
        }
        if(generate) {
            if(coolTime > interval) {
                Spawn();
                coolTime = 0;
            }
            coolTime += Time.deltaTime;
        }
    }
    
    public void Spawn()
    {
        var enemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity, this.transform);
        enemy.SetPlayerPosition(player);
        enemy.SetContainerPosition(destination);
        enemy.tutorialManager = tutorialManager;
    }
}
