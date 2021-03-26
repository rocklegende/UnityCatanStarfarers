using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    public class SpacePointButtonScript : SFView
    {
        public SpacePoint spacePoint;
        public GameObject referenceToInstance;
        public GameObject showText;
        // Start is called before the first frame update
        void Start()
        {
            if (GameConstants.ShowTextAboveSpacePointButtons)
            {
                showText.GetComponent<TextMesh>().text = spacePoint.coordinates.q + ":" + spacePoint.coordinates.r + ":" + spacePoint.vertexNumber;
            }
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


