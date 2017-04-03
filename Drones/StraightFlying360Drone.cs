﻿using System.Collections;
using Assets.Scripts.SLA.Levels;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Drones
{
    class StraightFlying360Drone : ADrone
    {
        protected readonly int NumRays;
        protected readonly bool IsTop;
        protected readonly float? Delay;
        protected readonly Vector3? Position;

        public StraightFlying360Drone(float speed, float size, Color color, int numRays, bool isTop, float? delay = null, Vector3? position = null) : base(speed, size, color)
        {
            NumRays = numRays;
            IsTop = isTop;
            Delay = delay;
            Position = position;
        }

        public override GameObject CreateDroneInstance(DroneFactory factory, bool isAdded)
        {
            factory.StartCoroutine(Generate360Drones(factory));
            return null;
        }

        private IEnumerator Generate360Drones(DroneFactory factory)
        {
            var clockwise = true;
            var startRotation = 0f;
            var position = Position ?? (IsTop
                ? DroneStartPosition.GetRandomTopSector(Size, BoundariesSLA.FlyingSla)
                : DroneStartPosition.GetRandomBottomSector(Size, BoundariesSLA.FlyingSla));
            
            // If delay is not null, the drones will go out in a fan motion.  If it is null, all rays will go out at the same time
            if (Delay != null)
            {
                clockwise = (IsTop) ?  position.x >= 0 : position.x < 0;
                startRotation = (position.x < 0) ? 90f : -90f;
            }

            for (var i = 0; i < NumRays; i++)
            {
                // spawn new drone in set position, direction and dronespeed
                var rotation = startRotation + (clockwise ? 1 : -1) * (360f * i / NumRays);
                var drone = Object.Instantiate(factory.FlyingOnewayDrone, position, Quaternion.Euler(0, rotation, 0));
                ConfigureDrone(drone);

                if (Delay != null)
                    yield return new WaitForSeconds(Delay.Value);
            }
        }
    }
}