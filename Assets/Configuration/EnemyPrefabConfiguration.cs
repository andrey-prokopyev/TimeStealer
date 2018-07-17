using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

namespace Configuration
{
    [Serializable]
    public class EnemyPrefabConfiguration
    {
        public GameObject[] Prefabs;

        public IDictionary<string, GameObject> PrefabDictionary
        {
            get { return this.Prefabs.ToDictionary(p => p.name, p => p); }
        }
    }
}