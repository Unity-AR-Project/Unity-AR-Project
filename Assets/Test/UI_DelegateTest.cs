using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ARP.Test
{
    //�������̽��� �ٸ� ��ü�� ��ȣ�ۿ� �� �� �ִ� ��ɵ��� �����ϴ� �߻� �����̹Ƿ�
    //������� ���������� public��
    public interface IAvailable
    {
        bool this[int index] { get; }
        bool IsAvailable { get; }
        void Use();
        event Action<bool> OnIsAvailableChanged; //event�� �Ⱥ����� �׳� �����̹Ƿ� ����� �� �� ����.
    }

    //Serialization : �ν��Ͻ��� string/byte[] ������ �������� �����ͼ����� ��ȯ
    //Deserialization : string/byte[] ������ �������� �����ͼ��� ���α׷��� Ư�� Ÿ�� �ν��Ͻ��� ��ȯ
    public class UI_DelegateTest : MonoBehaviour
    {
        //SerializeField Attribute : ��������� ���� �����͸� ����ȭ�Ͽ� UnityEditor�� �ν�����â�� �����Ű�� Ư��
        //����ϴ� ���� : ĸ��ȭ�� (priavte/protected) ��������� Editor���� �⺻������ ����ȭ���� �ʱ� ����
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _toggleTarget;
        public Predicate<Vector3> match;
        public Action<int, float> action;
        public Func<long ,long, int> func;

        private void Awake()
        {
            //_button.onClick += ?? //�Ϲ����� C# event delegate�� �����ϴ� ����
            //_button.onClick.AddListener(??); //Unity���� �����ϴ� UnityAction ��ü�� �����ϴ� ����
            _button.onClick.AddListener(ToggleTarget);
            //���ٽ�
            //_button.onClick.AddListener(() => _toggleTarget.SetActive(_toggleTarget.activeSelf == false));

            match += IsOrigin;
            match += (position) => position == Vector3.zero;


            if (match.Invoke(Vector3.zero))
            {

            }

            action += LogSum;
            action += (a, b) => Debug.Log(a + b); ;
            action.Invoke(3, 4.2f);

            func += Sum;
            func += (a, b) =>
            {
                int tmp = (int)(a + b);
                return tmp;
            };
        }

        void ToggleTarget()
        {
            _toggleTarget.SetActive(_toggleTarget.activeSelf == false);
        }

        private bool IsOrigin(Vector3 position)
        {
            return position == Vector3.zero;
        }
        //(position) => position == Vector3.zero;


        //���ٽ��� C#���� �ζ����Լ��� ������ �� ���.
        //�ζ��� �Լ� : ���� �ڵ���ο� �ٷ� �����ϴ� �Լ�
        //�ζ��� �Լ� ���� ���� : �Լ�������� ����, ū �ǹ̾��� �Լ����� �������� ���� �����Ͽ� �ڵ� �������� ������(������ �Լ��϶���)

        private void LogSum(int a, float b)
        {
            Debug.Log(a + b);
        }
        //(a, b) => Debug.Log(a + b);

        private int Sum(long a, long b)
        {
            int tmp = (int)(a+b);
            return tmp;
        }
        //(a,  b) =>
        //{
        //    int tmp = (int)(a + b);
        //    return tmp;
        //}
    }
}