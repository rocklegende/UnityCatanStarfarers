using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    public class SpacePointButtonScript : MonoBehaviour
    {
        public SpacePoint spacePoint;
        public GameObject referenceToInstance;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnClick()
        {

            // TODO; just send a notification that this spacepointbutton was clicked
            GameObject map = GameObject.Find("Map");
            map.GetComponent<MapScript>().OnSpacePointClicked(referenceToInstance, this.spacePoint);
        }
    }
}


