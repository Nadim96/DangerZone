using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Utility;

namespace Assets.Scripts.NPCs
{
    public enum StateType
    {
        Zero, 	//freeze
        One, 	//ragdollstate
        Two		//animState
    }
    /// <summary>
    /// Handles the ragdoll of a npc
    /// </summary>
    public class Ragdoll02 : MonoBehaviour
    {
        public HitMessage HitMessage;
		public NPC _npc;
        public Animator anim;

        public StateType stateType;
        public bool ragFull;    //Activate ragdoll completely
		public bool blend;     //Activate Skin > Anim in Slerp (smooth transition)
        public bool IsGettingUp;

        private List<GameObject> _animBones;
        private List<GameObject> _skinBones; //This hierarchy also contains the Colliders the player can hit in prefab 
        private List<GameObject> _ragBones; //contains rigidbodies en colliders for the ragdoll

        private List<GameObject> _upperBodyRag;
		[HideInInspector]
		public List<GameObject> _legsRag;

        // Use this for initialization
        void Start()
        {
            anim = GetComponentInChildren<Animator>();
			_npc = GetComponent<NPC> ();

            _animBones = new List<GameObject>();
            _skinBones = new List<GameObject>();
            _ragBones = new List<GameObject>();

            _upperBodyRag = new List<GameObject>();
            _legsRag = new List<GameObject>();

            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
            {
                if (child.tag == "AnimBones")
                    _animBones.Add(child.gameObject);
                else if (child.tag == "SkinBones")
                    _skinBones.Add(child.gameObject);
                else if (child.tag == "RagBones")
                {
                    _ragBones.Add(child.gameObject);

                    Rigidbody rbody = child.GetComponent<Rigidbody>();
                    if (rbody != null)
                    {
                        rbody.isKinematic = true;
                        rbody.detectCollisions = false;
                    }
                }

                stateType = StateType.Two;
            }

            if (_ragBones.SingleOrDefault(obj => obj.name == "Spine1"))
            {
                GameObject temp = _ragBones.SingleOrDefault(obj => obj.name == "Spine1");
                List<Rigidbody> bones = temp.GetComponentsInChildren<Rigidbody>().ToList();
                foreach (Rigidbody bone in bones)
                {
                    _upperBodyRag.Add(bone.gameObject);
                }
            }

            string[] legs = { "LThighTwist", "RThighTwist" };

            foreach (string t in legs)
            {
                if (_ragBones.SingleOrDefault(obj => obj.name == t))
                {
                    GameObject temp = _ragBones.SingleOrDefault(obj => obj.name == t);
                    List<Rigidbody> bones = temp.GetComponentsInChildren<Rigidbody>().ToList();
                    foreach (Rigidbody bone in bones)
                    {
                        _legsRag.Add(bone.transform.gameObject);
                    }
                }
            }
        }

        public void RagOn(HitMessage hitmessage)
        {
            HitMessage = hitmessage;
            stateType = StateType.One;

            //set ragbones position
            _ragBones[0].transform.localPosition = _skinBones[0].transform.localPosition;

            for (int i = 0; i < _ragBones.Count; i++)
            {
                _ragBones[i].transform.localRotation = _skinBones[i].transform.localRotation;
            }

            if (!ragFull)
            {
                BlendBack(); //initiate blend

                //naam geraakte bone overeen komt met naam in deze lijst(object van ragbones).. 
                //hoewel de collider op skinbones zit maar zelfde naam heeft werkt dit ook. Bug misschien maar het werkt!
                if (_upperBodyRag.Any(obj => obj.name == HitMessage.HitObject.name))
                    SetKinimatic(_upperBodyRag);
                else if (_legsRag.Any(obj => obj.name == HitMessage.HitObject.name))
                    SetKinimatic(_legsRag);
            }
            else
            {
                if (_ragBones.Any(obj => obj.name == HitMessage.HitObject.name))
                {
                    foreach (GameObject rb in _ragBones)
                    {
                        Rigidbody rbody = rb.GetComponent<Rigidbody>();

                        if (rbody != null)
                        {
                            rbody.isKinematic = false;
                            rbody.detectCollisions = true;
                        }
                    }
                }
                stateType = StateType.One;
            }
            ApplyForce();
        }

        public void SetKinimatic(List<GameObject> gameobject)
        {
            foreach (GameObject rb in gameobject)
            {
                Rigidbody rbody = rb.GetComponent<Rigidbody>();

                if (rbody != null)
                {
                    rbody.isKinematic = false;
                }
            }
        }
        public void BlendBack()
        {
            StartCoroutine(WaitBlend());
        }

        IEnumerator WaitBlend()
        {
            yield return new WaitForSeconds(0.3f);
            if (ragFull) yield break; //stop blend if death is activated


            stateType = StateType.Zero;

            blend = true;

            yield return new WaitForSeconds(0.8f);
            if (ragFull) yield break;

            foreach (GameObject child in _ragBones)
            {
                Rigidbody rbody = child.GetComponent<Rigidbody>();
                if (rbody != null)
                {
                    rbody.isKinematic = true;
                    rbody.detectCollisions = false;
                }
            }
            blend = false;
            stateType = StateType.Two;
        }

        public void ApplyForce()
        {
            if (_ragBones.Any(obj => obj.name == HitMessage.HitObject.name))
            {
                Rigidbody addForceTo = _ragBones.SingleOrDefault(obj => obj.name == HitMessage.HitObject.name).GetComponent<Rigidbody>();
                //up is forward
                //forward is up
                																		//Up
				addForceTo.AddForceAtPosition(HitMessage.Source.TransformDirection(Vector3.up) * HitMessage.ImpactForce, HitMessage.PointOfImpact);
                addForceTo.AddForceAtPosition(HitMessage.Source.TransformDirection(Vector3.forward) * HitMessage.ImpactForce, HitMessage.PointOfImpact);
            }
        }

        public IEnumerator GetUp()
        {
            ragFull = false;
            stateType = StateType.Zero;

            blend = true;
            bool faceUp = Vector3.Dot(-_skinBones[0].GetComponent<Transform>().right, Vector3.up) > 0f;

            anim.SetFloat("BackFront", faceUp ? 0 : 1);
            anim.SetTrigger("GetUp");

            yield return new WaitForSeconds(6);

            if (!GetComponent<NPC>().IsAlive)
               yield break;

			blend = false;
            stateType = StateType.Two;
            IsGettingUp = false;
            GetComponent<NPC>().NavMeshAgent.enabled = true;
        }

        void LateUpdate()
        {
			if (stateType == StateType.Two)
            {
                _skinBones[0].transform.localPosition = _animBones[0].transform.transform.localPosition;

                for (int i = 0; i < _skinBones.Count; i++)
                {
                    _skinBones[i].transform.localRotation = _animBones[i].transform.localRotation;
                }
            }
            if (stateType == StateType.One)
            {
                _skinBones[0].transform.localPosition = _ragBones[0].transform.transform.localPosition;

                for (int i = 0; i < _skinBones.Count; i++)
                {
                    _skinBones[i].transform.localRotation = _ragBones[i].transform.localRotation;
                }
            }
			if (blend)
            {
                _skinBones[0].transform.localPosition = Vector3.Lerp(_skinBones[0].transform.localPosition, _animBones[0].transform.localPosition, 0.1f);
                for (int i = 0; i < _skinBones.Count; i++)
                {
                    _skinBones[i].transform.localRotation = Quaternion.Slerp(_skinBones[i].transform.localRotation, _animBones[i].transform.localRotation, 0.1f);
                }
            }
        }
    }
}