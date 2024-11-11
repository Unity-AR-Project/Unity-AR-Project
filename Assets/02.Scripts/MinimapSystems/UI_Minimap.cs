using UnityEngine;
using UnityEngine.UI;

/*
 * 전처리기 : 컴파일 전에 처리할 내용들을 다루는 기능
 * 전처리기 if : 조건을 비교하여 참인 내용만 컴파일에 포함시킴
 */
namespace ARP.MinimapSystems
{
    public class UI_Minimap : MonoBehaviour
    {
        private IGPS _gps => _unit.gps;

        private IUnitOfMinimap _unit;
        [SerializeField] float _zoom = 4;
        [SerializeField] Vector2 _size = new Vector2(512f, 512f);
        private GoogleMapInterface _googleMapInterface;
        [SerializeField] RawImage _map;

        private void Awake()
        {
            _googleMapInterface = new GameObject("GoogleMapInterface").AddComponent<GoogleMapInterface>();   
        }

        private void Start()
        {
#if UNITY_EDITOR
            _unit = new MockUnitOfMinimap();
#elif UNITY_ANDROID
            _unit = new UnitOfMinimap();
#endif
            RefreshMap();
        }

        private void OnEnable()
        {
            //RefreshMap();
        }

        private void RefreshMap()
        {
            _googleMapInterface.LoadMap(_gps.latitude, _gps.longitude, _zoom, _size, (texture) => _map.texture = texture);
        }
    }
}