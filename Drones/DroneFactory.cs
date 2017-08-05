﻿using System.Collections;
using System.Collections.Generic;
using Drones.DroneTypes;
using Drones.Pattern;
using Launcher;
using UnityEngine;

namespace Drones
{
    public class DroneFactory : MonoBehaviour
    { 
        //attach gameobjects
        private const string BouncingDrone = "BouncingDrone";
        private const string FlyingBouncingDrone = "FlyingBouncingDrone";
        private const string FlyingOneWayDrone = "FlyingOneWayDrone";
        private const string FlyingBouncingMine = "FlyingBouncingMine";
        private const string BouncingMine = "Bouncing Mine";
        private const string FlyingOneWayMine = "FlyingOneWayMine";

        public GameObject BouncingDronePrefab;
        public GameObject OneWayDronePrefab;

        public Material GreyMaterial;
        public Material BlueMaterial;
        public Material RedMaterial;
        public Material GoldenMaterial;
        public Material MagentaMaterial;
        public Material DarkGreenMaterial;
        public Material CyanMaterial;
        public Material BrightGreenMaterial;

        public Dictionary<DroneType,string> SetDroneType = new Dictionary<DroneType,string>();
        public Dictionary<DroneColor, Material> SetDroneMaterial = new Dictionary<DroneColor, Material>();

        private void Awake()
        {
            SetDroneType[DroneType.BouncingDrone] = BouncingDrone;
            SetDroneType[DroneType.FlyingBouncingDrone] = FlyingBouncingDrone;
            SetDroneType[DroneType.FlyingOneWayDrone] = FlyingOneWayDrone;
            SetDroneType[DroneType.FlyingBouncingMine] = FlyingBouncingMine;
            SetDroneType[DroneType.BouncingMine] = BouncingMine;
            SetDroneType[DroneType.FlyingOneWayMine] = FlyingOneWayMine;

            SetDroneMaterial[DroneColor.Grey] = GreyMaterial;
            SetDroneMaterial[DroneColor.Blue] = BlueMaterial;
            SetDroneMaterial[DroneColor.Red] = RedMaterial;
            SetDroneMaterial[DroneColor.Golden] = GoldenMaterial;
            SetDroneMaterial[DroneColor.Magenta] = MagentaMaterial;
            SetDroneMaterial[DroneColor.DarkGreen] = DarkGreenMaterial;
            SetDroneMaterial[DroneColor.Cyan] = CyanMaterial;
            SetDroneMaterial[DroneColor.BrightGreen] = BrightGreenMaterial;
        }

        public List<GameObject> SpawnDrones(IDrone drone, int droneCount = 1, bool isAdded = false, Area area = new Area(), StartPositionDelegate posDelegate = null)
        {
            var drones = new List<GameObject>();

            for (var i = 0; i < droneCount; i++)
            {
                var newDrone = drone.CreateDroneInstance(this, isAdded, area, posDelegate);
                if (newDrone != null)
                {
                    drone.ConfigureDrone(newDrone, this);
                    newDrone.AddComponent<DroneManager>();
                }
                drones.Add(newDrone);
            }

            return drones;
        }

        public void SetPattern(IPattern pattern, IDrone drone, Area area = new Area(), StartPositionDelegate posDelegate = null)
        {
            pattern.SetPattern(this, drone, area, posDelegate);
        }

        public void AddPattern( IPattern pattern, GameObject parentDrone, IDrone addedDrone, Area area = new Area())
        {
            pattern.AddPattern(this, parentDrone, addedDrone, area);
        }

        public void AddDrones(IDrone drone, float delay, Area area = new Area(), StartPositionDelegate posDelegate = null)
        {
            StartCoroutine(GenerateDrones(drone, delay, area, posDelegate));
        }

        private IEnumerator GenerateDrones(IDrone drone, float delay, Area area, StartPositionDelegate posDelegate)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                SpawnDrones(drone, isAdded: true, area: area, posDelegate: posDelegate);
            }
        }

        public void SpawnAndAddDrones(IDrone drone, int droneCount, float delay, Area area = new Area(), StartPositionDelegate posDelegate = null)
        {
            SpawnDrones(drone, droneCount, area: area, posDelegate: posDelegate);
            AddDrones(drone, delay, area, posDelegate);
        }
    }

    // Boundaries in which new drones can be spawned
    public struct Area
    {
        public float LeftBoundary;
        public float RightBoundary;
        public float TopBoundary;
        public float BottomBoundary;
    }
}
