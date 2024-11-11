using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// �ĺ��� ��� ���
// PascalCase : �빮�� ����, �ܾ�� ù ���ĺ� �빮��
// camelCase : �ҹ��� ����, �ܾ ù ���ĺ� �빮��
// snake_case : �ܾ� ���� _ ���
// UPPER_SNAKE_CASE : �ܾ� ���� _, ��� ���� �빮��

// HungarianNotion : iNum, fHeight // ��� ����
// m_~~ : ������� ��ù� // C#�� ��������� �ƴ� �۷ι��� ������ ���� ������ ������� ����


//��Ʈ ����ũ
public enum State
{
    Idle,
    Jump,
    Attack,
    Crouch
}

[Flags]
public enum States
{
    Idle = 0 << 0,
    Jump = 1 << 0,
    Attack = 1 << 1,
}

public class LineDrawer : MonoBehaviour
{
    [SerializeField] InputActionReference _dragCurrentPosition;
    [SerializeField] private Camera _xrCamera;
    [SerializeField] private float _drawOffsetZ = 0.5f;
    [SerializeField] private float _drawMinDistance = 0.01f;
    [SerializeField] private float _lineRendererWidth = 0.01f;
    private LineRenderer _lineRenderer;
    private List<Vector3> _positions = new List<Vector3>(512); // reserving ... �ν��Ͻ�ȭ �� �������÷����� ���ϸ� �ּ�ȭ�ϱ����� �뷮Ȯ��

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = _lineRendererWidth;
        _lineRenderer.endWidth = _lineRendererWidth;
        _lineRenderer.positionCount = 0;
    }

    private void Start()
    {
        _dragCurrentPosition.action.performed += OnDrag;
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        Vector2 dragPosition = context.ReadValue<Vector2>();
        Vector3 worldPosition = _xrCamera.ScreenToWorldPoint(new Vector3(dragPosition.x, dragPosition.y, _drawOffsetZ));

        if (_positions.Count == 0)
        {
            AddPositionToLineRenderer(worldPosition);
        }
        else
        {
            if(Vector3.Distance(_positions[_positions.Count - 1], worldPosition) >= _drawMinDistance)
            {
                AddPositionToLineRenderer(worldPosition);
            }
        }
    }

    // O(1)
    private void AddPositionToLineRenderer(Vector3 position)
    {
        _positions.Add(position);
        _lineRenderer.positionCount = _positions.Count;
        int index = _lineRenderer.positionCount - 1;
        _lineRenderer.SetPosition(index, position);
    }

    // O(n)
    //private void RefreshLineRenderer()
    //{
    //    _lineRenderer.positionCount = _positions.Count;

    //    for (int i = 0; i < _positions.Count; i++)
    //    {
    //        _lineRenderer.SetPosition(i, _positions[i]);
    //    }
    //}
}
