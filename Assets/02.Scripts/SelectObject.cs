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
    [SerializeField] LayerMask _targetMask;  // ARProp ���̾� ����ũ (���� ���̾�)

    private Vector3 initialPosition; // �ʱ� ��ġ�� ������ ���� �߰�
    private float treeDetectionDistance = 2f; // ������ �󸶳� ��������� �������� ����
    private bool isCorgiOnTree = false; // �ڱⰡ ������ �����Ǿ����� Ȯ���ϴ� ����

    private void Start()
    {
        if (arCamera == null)
        {
            Debug.LogError("[Debug] : AR Camera not assigned."); // AR ī�޶� �Ҵ���� �ʾ��� ��� ���� ���
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return; // ��ġ�� ������ �Լ� ����

        Touch touch = Input.GetTouch(0); // ù ��° ��ġ �Է��� ������
        Debug.LogWarning($"[Debug] : {nameof(SelectObject)}] Touched.");


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
                    Debug.LogWarning($"[Debug] : corgi Layer : {LayerMask.LayerToName(selectedObj.layer)}");
                }
            }
        }

        // ��ġ�� �̵� ���̰� ������Ʈ�� ���õǾ��� ��
        if (touch.phase == TouchPhase.Moved && isTouched)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;

            // Raycast�� Ground�� ���� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                selectedObj.transform.position = hit.point;
                //// Raycast�� �浹�� ������Ʈ�� "TargetTree"��� �̸��� ���
                //if (hit.collider.gameObject.name == "TargetTree")
                //{
                //    Debug.LogWarning("[Debug] : TargetTree hit!");
                //    // ������ ��ġ�� ���� ������ �����ɴϴ�
                //    Vector3 treePosition = hit.collider.gameObject.transform.position;
                //    Vector3 treeForward = hit.collider.gameObject.transform.forward;

                //    // �ڱ⸦ ������ ���鿡 ������ŵ�ϴ� (treeDetectionDistance�� ���� �Ÿ�)
                //    selectedObj.transform.position = treePosition + treeForward * treeDetectionDistance;

                //    // �ڱ⸦ ������ �ڽ����� �����Ͽ� ���� ���·� ����ϴ�
                //    selectedObj.transform.SetParent(hit.collider.transform);

                //    // �ڱⰡ ������ �����Ǿ����� ǥ��
                //    isCorgiOnTree = true;

                //    // �α� ��� (������)
                //    Debug.LogWarning("[Debug] : Corgi has been fixed to the front of the tree!");
                //}
                //else
                //{
                //    // ������ �ƴ� �ٸ� ������Ʈ���� �ڱ⸦ �����Ӱ� �̵�
                //    selectedObj.transform.position = hit.point;
                //}
            }
        }

        // ��ġ�� ���� ��
        if (touch.phase == TouchPhase.Ended)
        {
            if (selectedObj != null)
            {
                isTouched = false; // ������Ʈ ���� ����
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // ��ġ�� ������ ������Ʈ�� ���̾ ARSelectable�� ����
                selectedObj.transform.position = initialPosition; // ������Ʈ�� �ʱ� ��ġ�� �ǵ�����
                selectedObj.transform.SetParent(null); // �θ� �����Ͽ� �����Ӱ� �̵�
                selectedObj = null;
                Debug.LogWarning("[Debug] : object deselected");
            }
        }
    }
}