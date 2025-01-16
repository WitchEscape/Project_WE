using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Pen : MonoBehaviour
{
    //액티브 이벤트로 리팩토링 예정

    public Transform tip;
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.01f, 0.1f)] public float penWidth = 0.01f;
    public Color[] penColors;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;
    public XRGrabInteractable interactable;

    public InputActionProperty leftTrigger;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private bool isDrawing;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];

        interactable.selectEntered.AddListener(OnGrabbed);
        interactable.selectExited.AddListener(OnReleased);
    }

    private void OnDestroy()
    {
        interactable.selectEntered.RemoveListener(OnGrabbed);
        interactable.selectExited.RemoveListener(OnReleased);
    }

    private void Update()
    {
        if (isDrawing)
        {
            Draw();
        }

        if (leftHand && leftTrigger.action.WasPressedThisFrame())
        {
            SwitchColor();
        }

        //Debug.Log($"TipPos : {tip.transform.position}");
        //Debug.Log($"LinePos : {currentDrawing.transform.position}");
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isDrawing = true;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isDrawing = false;
        currentDrawing = null;
    }

    private void Draw()
    {
        //Debug.Log($"Draw");
        if (currentDrawing == null)
        {
            //Debug.Log($"if");
            index = 0;
            currentDrawing = new GameObject("Line").AddComponent<LineRenderer>();
            currentDrawing.material = drawingMaterial;


            Color colorWithAlpha = new Color(
                penColors[currentColorIndex].r,
                penColors[currentColorIndex].g,
                penColors[currentColorIndex].b,
                1.0f
            );

            currentDrawing.startColor = colorWithAlpha;
            currentDrawing.endColor = colorWithAlpha;
            currentDrawing.startWidth = penWidth;
            currentDrawing.endWidth = penWidth;
            currentDrawing.positionCount = 1;
            currentDrawing.SetPosition(0, tip.position);
        }
        else
        {
            //Debug.Log($"else");
            var currentPos = currentDrawing.GetPosition(index);
            if (Vector3.Distance(currentPos, tip.position) > 0.01f)
            {
                index++;
                currentDrawing.positionCount = index + 1;
                currentDrawing.SetPosition(index, tip.position);
            }
        }
    }


    private void SwitchColor()
    {
        if (currentColorIndex == penColors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        tipMaterial.color = penColors[currentColorIndex];
    }
}
