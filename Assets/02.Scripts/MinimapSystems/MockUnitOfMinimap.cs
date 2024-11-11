using UnityEngine;

namespace ARP.MinimapSystems
{
    public class MockUnitOfMinimap : IUnitOfMinimap
    {
        public MockUnitOfMinimap()
        {
            gps = new GameObject(nameof(MockGPS)).AddComponent<MockGPS>();
        }


        public IGPS gps { get; }
    }
}