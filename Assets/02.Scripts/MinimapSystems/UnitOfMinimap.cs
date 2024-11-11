using UnityEngine;

namespace ARP.MinimapSystems
{
    public class UnitOfMinimap : IUnitOfMinimap
    {
        public UnitOfMinimap()
        {
            gps = new GameObject(nameof(GPS)).AddComponent<GPS>();
        }


        public IGPS gps { get; }
    }
}