using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem;

    [Tooltip("ボタンが選択されたら、true")]
    public bool select_f = false;

    public Image humanSoul;

    GameObject selectObj;

    //SEPlayer sePlay;

    private void Start()
    {
        //sePlay = GameObject.Find("SEPlayer").GetComponent<SEPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (select_f == false)
        {
            try
            {
                selectObj = eventSystem.currentSelectedGameObject.gameObject;
                if(this.gameObject == selectObj)
                {
                    humanSoul.enabled = true;
                    select_f = true;
                    
                }
            }
            catch
            {

            }
        }
        else
        {
            try
            {
                selectObj = eventSystem.currentSelectedGameObject.gameObject;
                if (this.gameObject != selectObj)
                {
                    humanSoul.enabled = false;
                    select_f = false;
                }
            }
            catch
            {

            }
        }
    }
}
