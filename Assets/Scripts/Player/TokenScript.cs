using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space
{
    public class TokenScript : SFController
    {

        public GameObject shipPrefab;
        public GameObject colonyBasePrefab;
        public GameObject tradeShipPrefab;
        public GameObject spacePortPrefab;

        GameObject tokenInstance;

        public GameObject tokenGameObject;
        public Token tokenModel;
        // Start is called before the first frame update
        void Start()
        {

        }

        GameObject TokenToPrefab(Token token)
        {
            if (token.id == new ShipToken().id)
            {
                return shipPrefab;
            }
            else if (token.id == new ColonyBaseToken().id)
            {
                return colonyBasePrefab;
            }
            else if (token.id == new TradeBaseToken().id)
            {
                return tradeShipPrefab;
            }
            else if (token.id == new SpacePortToken().id)
            {
                return spacePortPrefab;
            }
            else
            {
                return new GameObject();
            }
        }

        public void Draw()
        {
            // draw token plus attached token if needed
            Vector3 position = tokenModel.GetUnityPosition();
            if (position != null)
            {
                GameObject prefab = TokenToPrefab(tokenModel);
                tokenInstance = Instantiate(prefab, position, Quaternion.identity);
                tokenInstance.transform.position = new Vector3(position.x, position.y, Constants.TOKEN_LAYER);
                tokenInstance.transform.parent = this.transform;

                if (tokenModel.attachedToken != null)
                {
                     //set at same position
                    GameObject prefabAttached = TokenToPrefab(tokenModel.attachedToken);
                    GameObject tokenInstanceAttached = Instantiate(prefabAttached, position, Quaternion.identity);
                    tokenInstanceAttached.transform.position = new Vector3(position.x, position.y, Constants.TOKEN_LAYER - 1);
                    tokenInstanceAttached.transform.parent = tokenInstance.transform;
                }
            }
        }

        public void Redraw()
        {
            GameObject.Destroy(tokenInstance);
            Draw();
        }

        private void OnMouseDown()
        {
            OnClick();
        }

        public void OnClick()
        {
            app.Notify(SFNotification.token_was_selected, gameObject, new object[] { tokenModel, tokenInstance });
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void MoveTo(SpacePoint point)
        {
            tokenInstance.transform.position = new Vector3(point.ToUnityPosition().x, point.ToUnityPosition().y, Constants.TOKEN_LAYER);
        }

        public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
        {
            switch(p_event_path)
            {
                case SFNotification.token_data_changed:
                    Redraw();
                    break;
                        
            }
        }
    }
}

