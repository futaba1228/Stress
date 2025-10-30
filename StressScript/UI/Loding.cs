using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] float LoadTime;

    [SerializeField]
    float time, remaining;

    private void Start()
    {
        time = 0;
        remaining = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        time += Time.deltaTime;
        LoadTime -= Time.deltaTime;
        remaining = LoadTime - (int)time;

        if (LoadTime <= 0)
        {
            SceneManager.LoadScene("Stage1");
        }
    }
}