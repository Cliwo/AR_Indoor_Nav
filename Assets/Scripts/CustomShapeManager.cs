using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.iOS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

/*========================================
 * Main Class for Managing Markers
======================================== */

public class CustomShapeManager : MonoBehaviour
{
	public const string DEFAULT_SHAPE_NAME = "DEFAULT";
	public NavController navController;

	public List<GameObject> ShapePrefabs = new List<GameObject> ();
	[HideInInspector]
    public List<ShapeInfo> shapeInfoList = new List<ShapeInfo>();
	[HideInInspector]
    public List<GameObject> shapeObjList = new List<GameObject>();

	private GameObject lastShape;

	public bool shapesLoaded = false;
    //-------------------------------------------------
    // All shape management functions (add shapes, save shapes to metadata etc.
    //-------------------------------------------------

    public void AddShape(Vector3 shapePosition, Quaternion shapeRotation, bool isDestination)
    {
		int typeIndex = 0;//sphere
		if (isDestination) typeIndex = 1;//diamond

        ShapeInfo shapeInfo = new ShapeInfo();
        shapeInfo.px = shapePosition.x;
        shapeInfo.py = shapePosition.y;
        shapeInfo.pz = shapePosition.z;
        shapeInfo.qx = shapeRotation.x;
        shapeInfo.qy = shapeRotation.y;
        shapeInfo.qz = shapeRotation.z;
        shapeInfo.qw = shapeRotation.w;
        shapeInfo.label = DEFAULT_SHAPE_NAME;
		shapeInfo.shapeType = typeIndex.GetHashCode();
        shapeInfoList.Add(shapeInfo);

        GameObject shape = ShapeFromInfo(shapeInfo);
        shapeObjList.Add(shape);
    }

	public void AddDestinationShape (string label) { 
		//change last waypoint to diamond
		ShapeInfo lastInfo = shapeInfoList [shapeInfoList.Count - 1];
		lastInfo.shapeType = 1.GetHashCode ();
		lastInfo.label = label;
		GameObject shape = ShapeFromInfo(lastInfo);
		shape.GetComponent<DiamondBehavior> ().Activate (true);
		//destroy last shape
		Destroy (shapeObjList [shapeObjList.Count - 1]);
		//add new shape
		shapeObjList.Add (shape);
	}

    public GameObject ShapeFromInfo(ShapeInfo info)
    {
		GameObject shape;
		Debug.Log("Create : " +info.label + " , Type : " + info.shapeType);
		Vector3 position = new Vector3 (info.px, info.py, info.pz);
		//if loading map, change waypoint to arrow
		if (SceneManager.GetActiveScene ().name == "ReadMap" && info.shapeType == 0) {
			shape = Instantiate (ShapePrefabs [2]);
		} else {
			shape = Instantiate (ShapePrefabs [info.shapeType]);
		}
		if (shape.GetComponent<Node> () != null) {
			shape.GetComponent<Node> ().pos = position;
            Debug.Log(position);
		}
		shape.tag = "waypoint";
		shape.transform.position = position;
		shape.transform.rotation = new Quaternion(info.qx, info.qy, info.qz, info.qw);
		shape.transform.localScale = new Vector3(.3f, .3f, .3f);

		return shape;
    }

    public void ClearShapes()
    {
        Debug.Log("CLEARING SHAPES!!!!!!!");
        foreach (var obj in shapeObjList)
        {
            Destroy(obj);
        }
        shapeObjList.Clear();
        shapeInfoList.Clear();
    }


    public JObject Shapes2JSON()
    {
        ShapeList shapeList = new ShapeList();
        shapeList.shapes = new ShapeInfo[shapeInfoList.Count];
        for (int i = 0; i < shapeInfoList.Count; i++)
        {
            shapeList.shapes[i] = shapeInfoList[i];
        }

        return JObject.FromObject(shapeList);
    }

    public void LoadShapesJSON(JToken mapMetadata, string selectedDestination)
    {
		if (!shapesLoaded) {
			shapesLoaded = true;
            Debug.Log("LOADING SHAPES>>>");
			if (mapMetadata is JObject && mapMetadata ["shapeList"] is JObject) {
				ShapeList shapeList = mapMetadata ["shapeList"].ToObject<ShapeList> ();
				if (shapeList.shapes == null) {
					Debug.Log ("no shapes dropped");
					return;
				}

				foreach (var shapeInfo in shapeList.shapes) {
					Debug.Log(shapeInfo.label + " is loaded");
					if (shapeInfo.label != selectedDestination && shapeInfo.shapeType == 1)
					{
						Debug.Log("Change type : " + shapeInfo.label);
						shapeInfo.shapeType = 0;
					}

					shapeInfoList.Add (shapeInfo);
					GameObject shape = ShapeFromInfo (shapeInfo);
					shapeObjList.Add (shape);
				}

				if (navController != null) {
					navController.InitializeNavigation ();
				}
			}
		}
    }
}
