using UnityEngine;

namespace ARP.MinimapSystems
{
    public class MockGPS : MonoBehaviour, IGPS
    {
        public float latitude => 37.5138649f;

        public float longitude => 127.0295296f;

        public float altitude => 0f;
    }
}