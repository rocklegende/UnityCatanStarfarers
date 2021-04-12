using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace com.onebuckgames.UnityStarFarers
{
    public interface InstantiationStrategy
    {
        GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null); 
    }


    public class DevelopmentInstantiationStrategy : InstantiationStrategy
    {
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return GameObject.Instantiate(prefab, position, rotation);
        }
    }

    public class ProductionInstantiationStrategy : InstantiationStrategy
    {
        public GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            return PhotonNetwork.Instantiate(string.Format("Prefabs/{0}", prefab.name), position, rotation, group, data);
        }
    }

    public class SFEnvironment : MonoBehaviour
    {

        public static SFEnvironment Instance;
        public InstantiationStrategy instantiationStrategy;
        // Start is called before the first frame update
        void Start()
        {
            instantiationStrategy = new ProductionInstantiationStrategy();
            Instance = this;
        }

        public InstantiationStrategy GetInstantiationStrategy()
        {
            return instantiationStrategy;
        }

        public void SetInstantiationStrategy(InstantiationStrategy strategy)
        {
            this.instantiationStrategy = strategy;
        }
    }
}

