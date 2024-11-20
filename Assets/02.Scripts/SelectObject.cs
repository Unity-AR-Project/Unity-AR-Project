using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectObject : MonoBehaviour
{
    private bool isTouched = false; // ������Ʈ�� ��ġ�Ǿ����� ���θ� Ȯ���ϴ� �÷���
    private GameObject selectedObj; // ���õ� ������Ʈ�� ������ ����
    [SerializeField] private Camera arCamera; // AR ī�޶� (Inspector���� �Ҵ�)

    [SerializeField] ARImageMultipleObjectsSpawner arImageMultipleObjectsSpawner;

    [SerializeField] LayerMask _selectMask;
    [SerializeField] LayerMask _groundMask;

    private Vector3 initialPosition; // �ʱ� ��ġ�� ������ ���� �߰�

    private void Start()
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera not assigned."); // AR ī�޶� �Ҵ���� �ʾ��� ��� ���� ���
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return; // ��ġ�� ������ �Լ� ����

        Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է��� ������
        Debug.LogWarning($"[{nameof(SelectObject)}] Touched.");


        // ��ġ�� ���۵� ��
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _selectMask)) // Ray�� Collider�� �浹�ߴ��� Ȯ��
            {
                if (hit.collider.name.Contains("corgi")) // �浹�� ������Ʈ �̸��� "corgi"�� ���ԵǾ����� Ȯ��
                {
                    selectedObj = hit.collider.gameObject; // ���õ� ������Ʈ�� ����
                    initialPosition = selectedObj.transform.position; // �ʱ� ��ġ ����
                    isTouched = true; // ������Ʈ�� ��ġ���� ǥ��
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected"); // ������Ʈ�� ���̾ ARSelected�� ����
                    Debug.LogWarning("object selected");
                }
            }
        }
        // ��ġ�� corgi ���� �� �ݶ��̴��� ��Ȱ��ȭ�մϴ�.
        if (isTouched)
        {
            selectedObj.GetComponent<Collider>().enabled = false; // ���õ� ��ü�� �ݶ��̴� ��Ȱ��ȭ
        }

        if (touch.phase == TouchPhase.Moved && isTouched) // ��ġ�� �̵� ���̰� ������Ʈ�� ���õǾ��� ��
        {
            Debug.LogWarning("object selected & touched");
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _groundMask)) // ��鿡 ���� Raycast ����
            {

                // Raycast hit�� �����ϰ� ó��
                //Vector3 hitPoint = new Vector3(hit.point.x, selectedObj.transform.position.y, hit.point.z);
                //selectedObj.transform.position = hitPoint;
                Debug.LogWarning($"Hit on drag {hit.collider.gameObject}");
                selectedObj.transform.position = hit.point; // ���õ� ������Ʈ�� ��� ��ġ�� �̵�
            }
        }

        // ��ġ�� ���� ��
        if (touch.phase == TouchPhase.Ended)
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<Collider>().enabled = true; // �ݶ��̴� ��Ȱ��ȭ
                isTouched = false; // ������Ʈ ���� ����
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // ��ġ�� ������ ������Ʈ�� ���̾ ARSelectable�� ����
                selectedObj.transform.position = initialPosition; // ������Ʈ�� �ʱ� ��ġ�� �ǵ�����
                selectedObj = null;
                Debug.LogWarning("object deselected");
            }
        }
    }
}
