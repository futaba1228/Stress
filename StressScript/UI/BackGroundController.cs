using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �y�w�i�̃R���g���[���p�N���X�z
///     �w�i��3���A�J�������猩�؂ꂽ���荞��
/// </summary>
public class BackGroundController : MonoBehaviour
{

    // �w�i�̖���
    public int spriteCount = 1;  //Insprctor�������͂ł��܂�
    // �w�i����荞��
    float rightOffset = 1.6f;  //���������Ă�������
    float leftOffset = -0.6f;  //���������Ă�������

    Transform bgTfm;
    SpriteRenderer mySpriteRndr;
    float width;

    void Start()
    {
        bgTfm = transform;
        mySpriteRndr = GetComponent<SpriteRenderer>();
        width = mySpriteRndr.bounds.size.x;
    }


    void Update()
    {
        // ���W�ϊ�
        Vector3 myViewport = Camera.main.WorldToViewportPoint(bgTfm.position);

        // �w�i�̉�荞��(�J������X���v���X�����Ɉړ���)
        if (myViewport.x < leftOffset)
        {
            bgTfm.position += Vector3.right * (width * spriteCount);
        }
        // �w�i�̉�荞��(�J������X���}�C�i�X�����Ɉړ���)
        else if (myViewport.x > rightOffset)
        {
            bgTfm.position -= Vector3.right * (width * spriteCount);
        }
    }
}