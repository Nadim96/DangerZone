using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.NPCs
{
    /// <summary>
    /// Handle the losing of limbs from Dummy's
    /// </summary>
    public class DetachableLimbs : MonoBehaviour {

        public HitMessage hitMessage;

        // -->Insert ParticleSystem(Sparks/Blood) 
        public GameObject sparkFX;
        // -->Insert Limb prefabs
        public GameObject L_armLimb;
        public GameObject R_armLimb;
        public GameObject L_legLimb;
        public GameObject R_legLimb;
        public GameObject _headLimb;

        private List<GameObject> _skinBones;
        private List<Transform> _armLeft;
        private List<Transform> _armRight;
        private List<Transform> _legLeft;
        private List<Transform> _legRight;
        private List<Transform> _head;

        private bool leftArm;
        private bool rightArm;
        private bool leftLeg;
        private bool rightLeg;
        private bool head;

        //voorkomen dat er meerdere Limbs spawnen uit 1 Limb
        private bool lArmInstantiate;
        private bool rArmInstantiate;
        private bool lLegInstantiate;
        private bool rLegInstantiate;
        private bool headInstantiate;

        //Ragbones Collider
        private List<GameObject> _ragBonesCol;
        private List<Collider> _armLCol;
        private List<Collider> _armRCol;
        private List<Collider> _legLCol;
        private List<Collider> _legRCol;
        private List<Collider> _headCol;

        void Awake () 
        {
            _skinBones = new List<GameObject> ();
            _armLeft = new List<Transform> ();
            _armRight = new List<Transform> ();
            _legLeft = new List<Transform> ();
            _legRight = new List<Transform> ();
            _head = new List<Transform> ();

            _ragBonesCol = new List<GameObject> ();
            _armLCol = new List<Collider> ();
            _armRCol = new List<Collider> ();
            _legLCol = new List<Collider> ();
            _legRCol = new List<Collider> ();
            _headCol = new List<Collider> ();

            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>()) 
            {
                if (child.CompareTag ("SkinBones")) 
                {
                    _skinBones.Add (child.gameObject);
                }

                if (child.CompareTag ("RagBones")) 
                {
                    if (child.GetComponent<Collider> () != null) 
                    {
                        _ragBonesCol.Add (child.gameObject);
                    }
                }
            }
			
            string[] findLimbs = new string[] { "LUpArmTwist", "RUpArmTwist", "LThighTwist", "RThighTwist", "Head" };
            for (int i = 0; i < findLimbs.Length; i++) 
            {
                //Skin Bones
                if (_skinBones.Where (obj => obj.name == findLimbs[i]).SingleOrDefault ()) 
                {
                    GameObject temp = _skinBones.Where (obj => obj.name == findLimbs[i]).SingleOrDefault ();
                    List<Collider> tempCol = new List<Collider> ();
			

                    //SkinBones with Colliders
                    if (temp.name == findLimbs [0]) 
                    {
                        tempCol = temp.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol)
                        {
                            _armLeft.Add (Col.transform);
                        }	
                    }
                    if (temp.name == findLimbs [1]) 
                    {
                        tempCol = temp.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol)
                        {
                            _armRight.Add (Col.transform);
                        }	
                    }
                    if (temp.name == findLimbs [2]) 
                    {
                        tempCol = temp.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol)
                        {
                            _legLeft.Add (Col.transform);
                        }	
                    }
                    if (temp.name == findLimbs [3]) 
                    {
                        tempCol = temp.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol)
                        {
                            _legRight.Add (Col.transform);
                        }
                    }
                    if (temp.name == findLimbs [4]) 
                    {
                        tempCol = temp.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol)
                        {
                            _head.Add (Col.transform);
                        }
                    }

					
                }	
                //RagBonesWith Collider
                if (_ragBonesCol.Where (obj => obj.name == findLimbs [i]).SingleOrDefault ()) 
                {
                    GameObject temp2 = _ragBonesCol.Where (obj => obj.name == findLimbs[i]).SingleOrDefault();
                    List<Collider> tempCol2 = new List<Collider> ();
			
                    if(temp2.name == findLimbs[0])
                    {
                        tempCol2 = temp2.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol2)
                        {
                            _armLCol.Add (Col.GetComponent<Collider>());
                        }
                    }
                    if(temp2.name == findLimbs[1])
                    {
                        tempCol2 = temp2.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol2)
                        {
                            _armRCol.Add (Col.GetComponent<Collider>());
                        }
                    }
                    if(temp2.name == findLimbs[2])
                    {
                        tempCol2 = temp2.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol2)
                        {
                            _legLCol.Add (Col.GetComponent<Collider>());
                        }
                    }
                    if(temp2.name == findLimbs[3])
                    {
                        tempCol2 = temp2.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol2)
                        {
                            _legRCol.Add (Col.GetComponent<Collider>());
                        }
                    }
                    if(temp2.name == findLimbs[4])
                    {
                        tempCol2 = temp2.GetComponentsInChildren<Collider>().ToList ();

                        foreach (Collider Col in tempCol2)
                        {
                            _headCol.Add (Col.GetComponent<Collider>());
                        }
                    }
                }
            }

            if (_skinBones.Where (obj => obj.name == "LUpArmTwist").SingleOrDefault ()) 
            {
			
                //	List<Transform> parts = temp.GetComponentInChildren<Collider> ().transform.ToList ();
            }
        }

        /// <summary>
        /// Detects limb that was hit and set the scale to 0 to create illusion arm is gone
        /// </summary>
        /// <param name="hitMessage"></param>
        private void LoseLimb (HitMessage hitMessage) 
        {
            this.hitMessage = hitMessage;

            if (hitMessage.HitObject != null) 
            {
                if (_armLeft.Any (obj => obj.name == hitMessage.HitObject.name)) 
                {
                    _armLeft [0].transform.localScale = Vector3.zero;
                    leftArm = true;
                }
                else if (_armRight.Any (obj => obj.name == hitMessage.HitObject.name)) 
                {
                    _armRight [0].transform.localScale = Vector3.zero;
                    rightArm = true;
                }	
                else if (_legLeft.Any (obj => obj.name == hitMessage.HitObject.name)) 
                {
                    _legLeft [0].transform.localScale = Vector3.zero;
                    leftLeg = true;
                }
                else if (_legRight.Any (obj => obj.name == hitMessage.HitObject.name)) 
                {
                    _legRight [0].transform.localScale = Vector3.zero;
                    rightLeg = true;
                }
                else if (_head.Any (obj => obj.name == hitMessage.HitObject.name)) 
                {
                    _head [0].transform.localScale = Vector3.zero;
                    head = true;
                }

                InstantiateObj ();// tijdelijk booleans moeten uitgezet worden na instantiate
            }

        }

        /// <summary>
        /// Instantiate Sparks & Limbs
        /// </summary>
        private void InstantiateObj()
        {
            if (leftArm) 
            {
                GameObject _spark = Instantiate (sparkFX);
                _armLCol [0].enabled = false;
                //		foreach (Collider col in _armLCol) 
                //		{
                //			col.enabled = false;
                //		}

                _spark.transform.parent = _armLeft [0].transform;
                _spark.transform.localScale = new Vector3 (1, 1, 1);
                _spark.transform.localEulerAngles = new Vector3 (0,0,0);
                _spark.transform.localPosition = _armLeft [0].transform.localPosition;

                if (lArmInstantiate == false) {
                    GameObject Limb = Instantiate (L_armLimb);
                    lArmInstantiate = true;
			

                    Transform[] _Children = Limb.GetComponentsInChildren<Transform> ();

                    List<GameObject> tempL = new List<GameObject> ();

                    foreach (Transform tr in _Children) {
                        if (tr.GetComponent<Collider> () != null) {
                            tempL.Add (tr.gameObject);
                        }
                    }

                    Limb.transform.position = _armLeft [0].transform.position;
                    Limb.transform.rotation = _armLeft [0].transform.rotation;

                    GameObject Limb02 = tempL [1];
                    Limb02.transform.position = _armLeft [1].transform.position;
                    Limb02.transform.rotation = _armLeft [1].transform.rotation;

                    if (Limb != null)
                        ApplyForceSeveredLimb (Limb, Limb02);

                    leftArm = false;
                }
            } 
            else if (rightArm)
            {
                GameObject _spark = Instantiate (sparkFX);
                _armRCol [0].enabled = false;
                //	foreach (Collider col in _armRCol) 
                //	{
                //		col.enabled = false;
                //	}
                _spark.transform.parent = _armRight [0].transform;
                _spark.transform.localScale = new Vector3 (1, 1, 1);
                _spark.transform.localEulerAngles = new Vector3 (0,0,0);
                _spark.transform.forward = -_armRight [0].transform.forward;
                _spark.transform.localPosition = _armRight [0].transform.localPosition;


                if (!rArmInstantiate)
                {
                    GameObject Limb = Instantiate (R_armLimb);
                    rArmInstantiate = true;
                    Transform[] _Children = Limb.GetComponentsInChildren<Transform> ();

                    List<GameObject> tempL = new List<GameObject> ();

                    foreach (Transform tr in _Children) {
                        if (tr.GetComponent<Collider> () != null) {
                            tempL.Add (tr.gameObject);
                        }
                    }

                    Limb.transform.position = _armRight [0].transform.position;
                    Limb.transform.rotation = _armRight [0].transform.rotation;

                    GameObject Limb02 = tempL [1];
                    Limb02.transform.position = _armRight [1].transform.position;
                    Limb02.transform.rotation = _armRight [1].transform.rotation;

                    if (Limb != null)
                        ApplyForceSeveredLimb (Limb, Limb02);

                    rightArm = false;
                }
            } 
            else if (leftLeg) 
            {
                GameObject _spark = Instantiate (sparkFX);
                _legLCol [0].enabled = false;

                //	foreach (Collider col in _legLCol) 
                //	{
                //		col.enabled = false;
                //	}
                _spark.transform.parent = _legLeft [0].transform;
                _spark.transform.localScale = new Vector3 (1, 1, 1);
                _spark.transform.localEulerAngles = new Vector3 (0,0,0);
                _spark.transform.forward = -_legLeft [0].transform.right;
                _spark.transform.localPosition = _legLeft [0].transform.localPosition;

                if (!lLegInstantiate) {
                    GameObject Limb = Instantiate (L_legLimb);

                    lLegInstantiate = true;
                    Transform[] _Children = Limb.GetComponentsInChildren<Transform> ();

                    List<GameObject> tempL = new List<GameObject> ();

                    foreach (Transform tr in _Children) {
                        if (tr.GetComponent<Collider> () != null) {
                            tempL.Add (tr.gameObject);
                        }
                    }

                    Limb.transform.position = _legLeft [0].transform.position;
                    Limb.transform.rotation = _legLeft [0].transform.rotation;

                    GameObject Limb02 = tempL [1];
                    Limb02.transform.position = _legLeft [1].transform.position;
                    Limb02.transform.rotation = _legLeft [1].transform.rotation;

                    if (Limb != null)
                        ApplyForceSeveredLimb (Limb, Limb02);

                    leftLeg = false;
                }
            } 
            else if (rightLeg) 
            {
                GameObject _spark = Instantiate (sparkFX);
                _legRCol [0].enabled = false;
                //		foreach (Collider col in _legRCol) 
                //		{
                //			col.enabled = false;
                //		}
                _spark.transform.parent = _legRight [0].transform;
                _spark.transform.localScale = new Vector3 (1, 1, 1);
                _spark.transform.localEulerAngles = new Vector3 (0,0,0);
                _spark.transform.forward = -_legRight [0].transform.right;
                _spark.transform.localPosition = _legRight [0].transform.localPosition;

                if (!rLegInstantiate) {
                    GameObject Limb = (GameObject)Instantiate (R_legLimb);
                    rLegInstantiate = true;
                    Transform[] _Children = Limb.GetComponentsInChildren<Transform> ();

                    List<GameObject> tempL = new List<GameObject> ();

                    foreach (Transform tr in _Children) {
                        if (tr.GetComponent<Collider> () != null) {
                            tempL.Add (tr.gameObject);
                        }
                    }

                    Limb.transform.position = _legRight [0].transform.position;
                    Limb.transform.rotation = _legRight [0].transform.rotation;

                    GameObject Limb02 = tempL [1];
                    Limb02.transform.position = _legRight [1].transform.position;
                    Limb02.transform.rotation = _legRight [1].transform.rotation;

                    if (Limb != null)
                        ApplyForceSeveredLimb (Limb, Limb02);

                    rightLeg = false;
                }
            }
            else if (head) 
            {

                GameObject _spark = Instantiate (sparkFX);
                _headCol [0].enabled = false;
                //		foreach (Collider col in _headCol) 
                //		{
                //			col.enabled = false;
                //		}
                _spark.transform.parent = _head [0].transform;
                _spark.transform.localScale = new Vector3 (1, 1, 1);
                _spark.transform.localPosition = _head [0].transform.localPosition;
                _spark.transform.forward = -_head [0].transform.right;
                _spark.transform.localEulerAngles = new Vector3 (0,-110.3f,0);
		
                if (!headInstantiate) {
                    GameObject Limb = Instantiate (_headLimb);
                    headInstantiate = true;
                    Transform[] _Children = Limb.GetComponentsInChildren<Transform> ();


                    GameObject Limb02 = _Children [1].gameObject;// bullshit

                    Limb.transform.position = _head [0].transform.position;
                    Limb.transform.rotation = _head [0].transform.rotation;

                    if (Limb != null)
                        ApplyForceSeveredLimb (Limb, Limb02);

                    head = false;
                }
            }
        }

        /// <summary>
        /// Past Force Toe op de Limbs die geinstantiate zijn
        /// </summary>
        /// <param name="limb"></param>
        /// <param name="limb02"></param>
        void ApplyForceSeveredLimb(GameObject limb, GameObject limb02)
        {
		
            //	RaycastHit hit = Raycast01.hit;
            //	Vector3 _storedHitLoc = hit.point;

            Vector3 _dir = hitMessage.Source.forward.normalized;


            //Past de richting aan waar de Limbs naartoe Vliegen(Force) ivbm visueel effect
            if (limb.transform.name == "LThighTwist(Clone)" || limb.transform.name == "LUpArmTwist(Clone)") {
                _dir.x -= 0.5f;
            }
            else if(limb.transform.name != "Head(Clone)")
            {
                _dir.x += 0.5f;
            }
			
            if (limb.transform.name != "Head(Clone)")
            {
                _dir.y += 0.0f;
            } 
            else
            {
                _dir.y += 0.5f;
            }
            Vector3 _rot = new Vector3 (1, 20, 15);
	
            if (hitMessage.HitObject.name + "(Clone)" == limb.transform.name) 
            {
                limb.GetComponent<Rigidbody> ().AddForceAtPosition (_dir * 300, hitMessage.PointOfImpact);
                limb.GetComponent<Rigidbody> ().AddTorque (_rot * 1 );
				
            }
            else if(hitMessage.HitObject.name == limb02.transform.name)
            {
                limb02.GetComponent<Rigidbody> ().AddForceAtPosition (_dir * 300, hitMessage.PointOfImpact);
                limb02.GetComponent<Rigidbody> ().AddTorque (_rot * 1 );
            }

        }
    }
}
