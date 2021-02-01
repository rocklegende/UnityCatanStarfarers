using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    public class SpacePointButtonScript : SFView
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
            app.Notify(SFNotification.spacepoint_selected, this, new object[] { spacePoint, referenceToInstance });
        }
    }
}


