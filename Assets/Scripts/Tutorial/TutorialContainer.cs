using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialContainer : MonoBehaviour
{
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private int ID;
    [SerializeField] private GameObject HPbar;
    [SerializeField] private Image HPbarUI;
    [SerializeField] private MeshRenderer normal, broke;
    [SerializeField] private GameObject particle;
    public float HP;
    private bool destroy = false;
    private Vector2 size;

    private void Start()
    {
        HP = 1;
        size = HPbarUI.rectTransform.sizeDelta;
        HPbarUI.color = Color.white;
        normal.enabled = true;
        broke.enabled = false;
    }

    private void Update()
    {
        if(HP >= 1) HPbarUI.color = Color.white;
        if(HP < 0.1f) HP = 0.1f;
        var HP_ = Mathf.Clamp01(HP);
        HPbar.transform.localPosition = new Vector3(0, (HP_ / 2) - 0.5f, 0);
        HPbar.transform.localScale = new Vector3(1, HP_, 1);
        HPbarUI.rectTransform.localPosition = new Vector3(-(size.x / 2) + ((HP_ * size.x * 0.5f)), 0, 0);
        HPbarUI.rectTransform.sizeDelta = new Vector2(size.x * HP_, size.y);
        if(HP_ < 0.2f) HPbarUI.color = Color.red;
        else if(HP_ < 0.5f) HPbarUI.color = new Color(1, 172f / 255f, 0);
        if(HP < 0 && !destroy) {
            this.tag = "Untagged";
            destroy = true;
            HPbar.transform.parent.gameObject.SetActive(false);
            normal.enabled = false;
            broke.enabled = true;
            particle.SetActive(true);
            //tutorialManager.ContainerDestory(ID);
        }
        //Debug.Log(ID + ", " + GetEnemyCount());
    }
    
    public int GetEnemyCount()
    {
        LayerMask mask = ~(Physics.AllLayers - 1);
        Collider[] collisions = Physics.OverlapBox(this.transform.position, new Vector3(5, 2.2f, 5), Quaternion.Euler(0, this.transform.eulerAngles.y, 0), mask);
        return collisions.Length > 2 ? collisions.Length - 2 : 0;
    }
}