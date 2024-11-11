using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 식별자 명명 방식
// PascalCase : 대문자 시작, 단어별로 첫 알파벳 대문자
// camelCase : 소문자 시작, 단어별 첫 알파벳 대문자
// snake_case : 단어 사이 _ 사용
// UPPER_SNAKE_CASE : 단어 사이 _, 모든 문자 대문자

// HungarianNotion : iNum, fHeight // 사용 안함
// m_~~ : 멤버변수 명시법 // C#은 멤버변수가 아닌 글로벌한 변수가 없기 때문에 사용하지 않음


//비트 마스크
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
    private List<Vector3> _positions = new List<Vector3>(512); // reserving ... 인스턴스화 및 가비지컬렉션의 부하를 최수화하기위한 용량확보

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
