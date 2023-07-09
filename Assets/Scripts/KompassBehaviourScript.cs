using System;
using UnityEngine;
using UnityEngine.Serialization;

public class KompassBehaviourScript : MonoBehaviour
{
    [SerializeField] private Transform pointerContainer;
    
    [FormerlySerializedAs("prefabStation")]
    [SerializeField] private GameObject prefabBone;
    
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private GameState gameState;

    [Header("Debug")]
    
    [SerializeField] private CollectibleBone[] listOfBoneObj;
    [SerializeField] private GameObject[] listOfBonePointer;
    
    private void OnEnable()
    {
        if(gameState == null) gameState = FindObjectOfType<GameState>();
    }

    public void UpdateList(CollectibleBone[] listOfBoneObjs)
    {
        listOfBoneObj = listOfBoneObjs;
        
        CleanPointer();
        CreatePointer();
    }

    private void CleanPointer()
    {
        if (listOfBonePointer != null)
        {
            for (int i = 0; i < listOfBonePointer.Length; i++)
            {
                GameObject obj = listOfBonePointer[i];
                Destroy(obj);
            }
            listOfBonePointer = Array.Empty<GameObject>();
        }
    }

    private void CreatePointer()
    {
        listOfBonePointer = new GameObject[listOfBoneObj.Length];
        for (int i = 0; i < listOfBoneObj.Length; i++)
        {
            CollectibleBone dumb = listOfBoneObj[i].GetComponentInChildren<CollectibleBone>();
            listOfBonePointer[i] = Instantiate(prefabBone, pointerContainer);
            listOfBonePointer[i].SetActive(true);
            
            if (dumb != null)
            {
                Vector2 pos = circleCollider2D.ClosestPoint(dumb.transform.position);
                listOfBonePointer[i].transform.position = pos;
            }
        }
    }

    private void UpdatePointer()
    {
        for (int i = 0; i < listOfBoneObj.Length; i++)
        {
            CollectibleBone dumb = listOfBoneObj[i];
            Vector3 pos = circleCollider2D.ClosestPoint(dumb.transform.position);
            if (circleCollider2D.OverlapPoint(dumb.transform.position)) {
                listOfBonePointer[i].SetActive(false);
            } else {
                listOfBonePointer[i].SetActive(true);
            }
            pos.z = -8;
            listOfBonePointer[i].transform.position = pos;
            listOfBonePointer [i].transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)(dumb.transform.position - pos));
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdatePointer();
    }
}
