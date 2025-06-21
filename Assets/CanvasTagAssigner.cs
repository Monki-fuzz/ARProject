using UnityEngine;
using UnityEngine.UI;

public class CanvasTagAssigner : MonoBehaviour
{
    public GameObject modelObject;

    public GameObject CanvasPopUp;
    public GameObject ModulePopUp;
    public GameObject LabelPopUp;
    public GameObject ApplicationPopUp;

    public Button ButtonInfo;
    public Button ButtonLabelling;

    private GameObject mainObject; // <-- new reference
    private bool assigned = false;
    private Camera mainCamera;

    void Start()
    {
        ButtonInfo.onClick.AddListener(OnInfoClicked);
        ButtonLabelling.onClick.AddListener(OnLabellingClicked);

        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!assigned)
        {
            modelObject = GameObject.FindGameObjectWithTag("modelObject");
            if (modelObject != null)
            {
                AssignCanvases();
                assigned = true;
            }
        }

        UpdateFacingDirection();
    }

    void AssignCanvases()
    {
        Transform[] children = modelObject.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in children)
        {
            if (child.CompareTag("CanvasPopUp"))
                CanvasPopUp = child.gameObject;
            else if (child.CompareTag("ModulePopUp"))
                ModulePopUp = child.gameObject;
            else if (child.CompareTag("LabelPopUp"))
                LabelPopUp = child.gameObject;
            else if (child.CompareTag("ApplicationPopUp"))
                ApplicationPopUp = child.gameObject;
            else if (child.CompareTag("mainObject"))
                mainObject = child.gameObject; 
        }

        if (CanvasPopUp) CanvasPopUp.SetActive(false);
        if (ModulePopUp) ModulePopUp.SetActive(false);
        if (LabelPopUp) LabelPopUp.SetActive(false);
        if (ApplicationPopUp) ApplicationPopUp.SetActive(false);

        Debug.Log("Canvas assignment complete.");
    }

    void OnInfoClicked()
    {
        bool shouldShow = !(CanvasPopUp?.activeSelf == true &&
                            ModulePopUp?.activeSelf == true &&
                            ApplicationPopUp?.activeSelf == true);

        if (CanvasPopUp) CanvasPopUp.SetActive(shouldShow);
        if (ModulePopUp) ModulePopUp.SetActive(shouldShow);
        if (ApplicationPopUp) ApplicationPopUp.SetActive(shouldShow);

        Debug.Log("Info Button Clicked");
    }

    void OnLabellingClicked()
    {
        bool shouldShow = !(LabelPopUp?.activeSelf == true);

        if (LabelPopUp) LabelPopUp.SetActive(shouldShow);
    }

    void UpdateFacingDirection()
    {
        if (mainCamera == null) return;

        FaceIfActive(CanvasPopUp);
        FaceIfActive(ModulePopUp);
        FaceIfActive(ApplicationPopUp);
        // FaceIfActive(LabelPopUp); // optional

        if (mainObject && mainObject.activeInHierarchy)
        {
            Vector3 direction = mainCamera.transform.position - mainObject.transform.position;
            mainObject.transform.rotation = Quaternion.LookRotation(direction);
            mainObject.transform.Rotate(0, 0, 0, Space.Self);
        }

    }

    void FaceIfActive(GameObject target)
    {
        if (target && target.activeInHierarchy)
        {
            Vector3 direction = mainCamera.transform.position - target.transform.position;
            target.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
