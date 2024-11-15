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
        if (touch.phase == TouchPhase.Began && !isCorgiOnTree)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _selectMask)) // Ray�� Collider�� �浹�ߴ��� Ȯ��
            {
                if (hit.collider.name.Contains("corgi")) // �浹�� ������Ʈ �̸��� "corgi"�� ���ԵǾ����� Ȯ�� //���߿� Target���� �̸� ���ؾ߰ڴ�.
                {
                    selectedObj = hit.collider.gameObject; // ���õ� ������Ʈ�� ����
                    initialPosition = selectedObj.transform.position; // �ʱ� ��ġ ����
                    isTouched = true; // ������Ʈ�� ��ġ���� ǥ��
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected"); // ������Ʈ�� ���̾ ARSelected�� ����
                    Debug.LogWarning($"[Debug] : corgi Layer : {LayerMask.LayerToName(selectedObj.layer)}");
                }
            }
        }

        // ��ġ�� �̵� ���̰� ������Ʈ�� ���õǾ�����, ������ �������� ���� ���
        if (touch.phase == TouchPhase.Moved && isTouched && !isCorgiOnTree)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // ��ġ ��ġ�κ��� Ray ����
            RaycastHit hit;

            // Raycast�� Ground�� ���� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {

                // Raycast�� �浹�� ������Ʈ�� "TargetTree"��� �̸��� ���
                if (hit.collider.gameObject.name == "TargetTree")
                {
                    Debug.LogWarning("[Debug] : TargetTree hit!");

                    // �ڱ⸦ ������ ���鿡 ������ŵ�ϴ�
                    selectedObj.transform.localPosition = new Vector3(0.39f, 0.35f, 0.29f); // ���� ��ġ�� ����

                    // �α׷� ��ġ Ȯ��
                    Debug.LogWarning($"[Debug] : Corgi new position: {selectedObj.transform.position}");

                    // �ڱⰡ ������ �����Ǿ����� ǥ��
                    isCorgiOnTree = true;

                    // �α� ��� (������)
                    Debug.LogWarning("[Debug] : Corgi has been fixed to the front of the tree!");

                }
                else
                {
                    // ������ �ƴ� �ٸ� ������Ʈ���� �ڱ⸦ �����Ӱ� �̵�
                    selectedObj.transform.position = hit.point;
                    Debug.LogWarning($"[Debug] : Corgi moved to ground at position: {hit.point}");
                }
            }
        }

        // ��ġ�� ���� ��
        if (touch.phase == TouchPhase.Ended)
        {
            if (selectedObj != null)
            {
                isTouched = false; // ������Ʈ ���� ����
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // ��ġ�� ������ ������Ʈ�� ���̾ ARSelectable�� ����
                
                // �ڱⰡ ������ ������ ���¶�� ��ġ�� �ʱ�ȭ���� ����
                if (!isCorgiOnTree)
                {
                    selectedObj.transform.position = initialPosition; // ������Ʈ�� �ʱ� ��ġ�� �ǵ�����
                    Debug.LogWarning("[Debug] : Corgi reset to initial position");
                }
                else
                {
                    Debug.LogWarning("[Debug] : Corgi remains at fixed position in tree");
                }

                selectedObj = null;
                Debug.LogWarning("[Debug] : object deselected");
            }
        }
    }
}