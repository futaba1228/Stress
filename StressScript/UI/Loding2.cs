using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading2 : MonoBehaviour
{
    //[SerializeField] GameObject OneReder;

    //[SerializeField] GameObject TwoReder;

    //[SerializeField] GameObject ThreeReder;

    [SerializeField] int LoadTime;
    float time, remaining;

    private void Start()
    {
        //OneReder.SetActive(false);
        //TwoReder.SetActive(false);
        //ThreeReder.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        time += Time.deltaTime;
        remaining = LoadTime - (int)time;

        //if (remaining == 3)
        //{
        //    OneReder.SetActive(true);
        //}

        //if (remaining == 2)
        //{
        //    TwoReder.SetActive(true);
        //}

        //if (remaining == 1)
        //{
        //    ThreeReder.SetActive(true);
        //}

        if (remaining <= 0)
        {
            SceneManager.LoadScene("Stage2");
        }
    }
}