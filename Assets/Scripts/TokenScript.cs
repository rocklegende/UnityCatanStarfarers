using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    public class TokenScript : MonoBehaviour
    {

        public GameObject token;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MoveTo(SpacePoint point)
        {
            Logger.log("moving piece");
            Logger.log(token);
            token.transform.position = new Vector3(point.ToUnityPosition().x, point.ToUnityPosition().y, Constants.TOKEN_LAYER);
        }
    }
}

