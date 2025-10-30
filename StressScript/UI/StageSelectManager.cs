using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class StageSelectManager : MonoBehaviour
{
    // �{�^����z��Ƃ��Ē�`
    [SerializeField] private Button[] _stageButton;
    void Start()
    {
        // �X�e�[�W�̃N���A�����擾
        int stageUnlock = PlayerPrefs.GetInt("StageUnlock", 1);

        // �X�e�[�W�{�^���̕\���E��\���̐ݒ�
        for (int i = 0; i < _stageButton.Length; i++)
        {
            if (i < stageUnlock)
                _stageButton[i].interactable = true;
            else
                _stageButton[i].interactable = false;
        }
    }
    public void StageSelect(int stage)
    {
        // �󂯎��������(stage)�̃X�e�[�W�����[�h����
        SceneManager.LoadScene(stage);
    }

    private void Update()
    {
        if(Input.GetKeyDown("joystick button 1"))
        {
            SceneManager.LoadScene("Title");
        }
    }
}