using System;
using UnityEngine;
using UnityStandardAssets.Cameras;
namespace UnityStandardAssets.SceneUtils
{
    public class PlaceTargetWithMouse : MonoBehaviour
    {
        public static PlaceTargetWithMouse Instance;
        public float surfaceOffset = 1.5f;
        public PlayerController setTargetOn;
        private BasePlayer Player;
        private bool CanPlaceTarget = true;
        public bool CanMove = true;
        public float Timer = 0;
        public FreeLookCam CamRef;
        RaycastHit hit;
        private GameObject TargetMarker;
        //public bool RunningToNpc;
        private void Start()
        {
            Instance = this;
            Player = GameObject.Find("Player").GetComponent<BasePlayer>();
            setTargetOn.HidePoint += HidePoint;
            TargetMarker = GameObject.Find("TargetMarker");
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Timer += Time.deltaTime;
                if (Timer >= 0.11f)
                {
                    CanPlaceTarget = false;
                    CamRef.CanRotate = true;
                }
                else
                {
                    CanPlaceTarget = true;
                }
            }

            if (!Input.GetMouseButtonUp(0) || setTargetOn.InventoryOpen || setTargetOn.SkillPanelOpen || !CanMove)
            {
                return;
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out hit) || hit.collider.tag == "Unclickable")
            {
                return;
            }


            if (hit.collider.tag == "NPC" && CanPlaceTarget)
            {

                if (Player.CurrentTarget != null && hit.collider.gameObject == Player.CurrentTarget.gameObject)
                {
                    setTargetOn.agent.isStopped = false;
                    setTargetOn.SetTarget(hit.collider.gameObject.transform);
                    //RunningToNpc = true;
                    HidePoint();
                }
                else if (hit.collider.GetComponent<IClickable>() != null)
                {
                    //RunningToNpc = false;
                    hit.collider.GetComponent<IClickable>().Clicked();
                    if(hit.collider.GetComponent<BaseNPC>().IsEnemy)
                    setTargetOn.GetDistance();
                    else
                        setTargetOn.agent.stoppingDistance = 2;
                }
                /*
                if (hit.collider.GetComponent<IClickable>() != null)
                    hit.collider.GetComponent<IClickable>().Clicked();
                setTargetOn.agent.isStopped = false;
                setTargetOn.SetTarget(hit.collider.gameObject.transform);
                */
            }
            else if(hit.collider.tag == "Pickup" && CanPlaceTarget)
            {
                setTargetOn.agent.stoppingDistance = 0.7f;
                //RunningToNpc = false;
                TargetMarker.transform.position = new Vector3(0, -1, 0);
                TargetMarker.transform.parent = null;
                setTargetOn.agent.isStopped = false;
                Player.CurrentTarget = null;
                setTargetOn.SetTarget(hit.collider.gameObject.transform);
            }
            else if (CanPlaceTarget)
            {
                //RunningToNpc = false;
                TargetMarker.transform.position = new Vector3(0, -1, 0);
                TargetMarker.transform.parent = null;
                Player.CurrentTarget = null;
                transform.position = hit.point + hit.normal * surfaceOffset;
                setTargetOn.agent.stoppingDistance = 0.2f;
                if (setTargetOn != null)
                {
                    setTargetOn.agent.isStopped = false;                    
                    //setTargetOn.SendMessage("SetTarget", transform);
                    setTargetOn.SetTarget(transform);
                }
            }
            Timer = 0;
        }
        
        public void AttackTarget()
        {
            setTargetOn.agent.isStopped = false;
            setTargetOn.SetTarget(Player.CurrentTarget.transform);
            //RunningToNpc = true;
            HidePoint();
        
        }
        
        private void HidePoint()
        {
            transform.position = new Vector3(0, -10, 0);
        }
        public void ClickedButton()
        {
            CanPlaceTarget = false;
        }
        public void UnClickedButton()
        {
            CanPlaceTarget = true;
        }

    }
}

