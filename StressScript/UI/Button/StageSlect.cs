using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSlect : MonoBehaviour
{
    private GameManager gameManager;

    [SerializeField]
    private GameObject stage2;

    [SerializeField]
    private GameObject stage3;

    [SerializeField]
    private GameObject noStage2;

    [SerializeField]
    private GameObject noStage3;

    public bool IsStage2;


    public bool IsStage3;

    [SerializeField]
    private int stageProgress;

    private StageSave stageSave;

    // Start is called before the first frame update
    void Start()
    {
        stageSave = GetComponent<StageSave>();

        stageProgress = PlayerPrefs.GetInt("NowStage", 1);

        if (stageProgress >= 1)
        {
            stage2.SetActive(true);

            stage3.SetActive(true);

            IsStage2 = false;

            IsStage3 = false;
        }
        if (stageProgress >= 2)
        {
            stage2.SetActive(false);

            noStage2.SetActive(true);
        }
        if (stageProgress >= 3)
        {
            stage3.SetActive(false);
            noStage3.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            stage2.SetActive(false);
            noStage2.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            stage3.SetActive(false);
            noStage3.SetActive(true);
        }
    }
}
