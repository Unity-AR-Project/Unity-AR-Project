using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

public class BallSpawner : MonoBehaviour
{
    [SerializeField] XRInputValueReader<Vector2> _tapStartPositionInput = new XRInputValueReader<Vector2>("Tap Start Position");
    //[SerializeField] InputActionReference _tapStartPositionInput;
    [SerializeField] GameObject _ballPrefab;
    [SerializeField] Transform _xrCamera;

    private void Start()
    {
        _tapStartPositionInput.inputActionReference.action.started += OnTapStartPositionInputStarted;
        //_tapStartPositionInput.inputActionReference.action.started += (context) =>
        //{
        //    Vector2 tapPosition = context.ReadValue<Vector2>();

        //    Debug.Log($"Tapped {tapPosition}");

        //    GameObject ball = Instantiate(_ballPrefab, _xrCamera.position + _xrCamera.forward * 0.5f, _xrCamera.rotation);
        //    ball.GetComponent<Rigidbody>().AddForce(_xrCamera.forward * 500f, ForceMode.Force);
        //};
    }

    private void OnTapStartPositionInputStarted(InputAction.CallbackContext context)
    {
        Vector2 tapPosition =  context.ReadValue<Vector2>();

        Debug.Log($"Tapped {tapPosition}");

        GameObject ball = Instantiate(_ballPrefab, _xrCamera.position + _xrCamera.forward * 0.5f, _xrCamera.rotation);
        ball.GetComponent<Rigidbody>().AddForce(_xrCamera.forward * 500f, ForceMode.Force);
        // ForceMode
        // Force : F(힘) = m(질량) * a(가속도) //질량이 높을수록 가속도가 낮아지는 일반적인 힘
        // Acceleration : a(가속도) //질량과 관계없이 가속도 설정
        // Impulse : I(충격량) = F(힘) * t(시간) = m(질량) * a(가속도) * t(시간) = m(질량) * v(속도) //질량이 높을수록 속도가 낮아지도록 충격량 부여
        // VelocityChange : v(속도) //질량과 관계없이 속도 설정
    }
}