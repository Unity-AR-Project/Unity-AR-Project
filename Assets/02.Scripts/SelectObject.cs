using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectObject : MonoBehaviour
{
    private bool isTouched = false; // ������Ʈ�� ��ġ�Ǿ����� ���θ� Ȯ���ϴ� �÷���
    private GameObject selectedObj; // ���õ� ������Ʈ�� ������ ����
    [SerializeField] private Camera arCamera; // AR ī�޶� (Inspector���� �Ҵ�)

    ARImageMultipleObjectsSpawner arImageMultipleObjectsSpawner;

    [SerializeField] LayerMask _selectedMask;
    [SerializeField] LayerMask _groundMask;

    private void Start()
    {
        arImageMultipleObjectsSpawner = GetComponent<ARImageMultipleObjectsSpawner>();
        if (arCamera == null)
        {
            Debug.LogError("AR Camera not assigned."); // AR ī�޶� �Ҵ���� �ʾ��� ��� ���� ���
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return; // ��ġ�� ������ �Լ� ����

        Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է��� ������

        if (touch.phase == TouchPhase.Began) // ��ġ�� ���۵� ��
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _selectedMask)) // Ray�� Collider�� �浹�ߴ��� Ȯ��
            {
                if (hit.collider.name.Contains("corgi")) // �浹�� ������Ʈ �̸��� "corgi"�� ���ԵǾ����� Ȯ��
                {
                    selectedObj = hit.collider.gameObject; // ���õ� ������Ʈ�� ����
                    isTouched = true; // ������Ʈ�� ��ġ���� ǥ��
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected"); // ������Ʈ�� ���̾ ARSelected�� ����
                }
            }
        }

        if (touch.phase == TouchPhase.Moved && isTouched) // ��ġ�� �̵� ���̰� ������Ʈ�� ���õǾ��� ��
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _groundMask)) // ��鿡 ���� Raycast ����
            {

                arImageMultipleObjectsSpawner.isHandling = true;
                selectedObj.transform.position = hit.point; // ���õ� ������Ʈ�� ��� ��ġ�� �̵�
            }
        }

        if (touch.phase == TouchPhase.Ended) // ��ġ�� ���� ��
        {
            isTouched = false; // ������Ʈ ���� ����
            arImageMultipleObjectsSpawner.isHandling = false;
            if (selectedObj != null)
            {
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // ��ġ�� ������ ������Ʈ�� ���̾ ARSelectable�� ����
            }
        }
    }
}
