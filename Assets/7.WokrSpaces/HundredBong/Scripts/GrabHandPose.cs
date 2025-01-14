using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VisualScripting;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrabHandPose : MonoBehaviour
{
    public float poseTransitionDuration = 0.2f;
    public HandData rightHandPose;
    public HandData leftHandPose;

    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;



    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);

        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
    }

    public void SetupPose(BaseInteractionEventArgs arg)
    {
        //물체를 잡을 때 호출됨

        if (arg.interactorObject is XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.anim.enabled = false;

            if (handData.handType == HandData.HandModelType.Right)
            {
                //현재 잡은 손의 데이터와 미리 정해둔 오른손의 데이터
                SetHandDataValues(handData, rightHandPose);
            }
            else if (handData.handType == HandData.HandModelType.left)
            {
                //현재 잡은 손의 데이터와 미리 정해둔 왼손의 데이터
                SetHandDataValues(handData, leftHandPose);
            }

            StartCoroutine(SetHandDataCoroutine(handData, finalHandPosition, finalHandRotation, finalFingerRotations, startingHandPosition, startingHandRotation, startingFingerRotations));
        }
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        //이미 현재 손의 데이터를 알고있고, 원래상태로 되돌리려면 starting 위치, 회전값을 사용하면 되므로 추가로 HandData를 받지 않음

        if (arg.interactorObject is XRDirectInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.anim.enabled = true;

            StartCoroutine(SetHandDataCoroutine(handData, startingHandPosition, startingHandRotation, startingFingerRotations, finalHandPosition, finalHandRotation, finalFingerRotations));
        }
    }

    public void SetHandDataValues(HandData h1, HandData h2)
    {
        //현재 손의 데이터와 목표 손의 데이터를 비교해서 시작 위치, 회전값과 최종 위치, 회전값을 각각 변수에 저장함
        //추후 손 포즈를 변화시키는데 사용됨


        //현재 잡은 손의 위치값 및 손의 스케일을 고려하여 h1.root.localPosition.x / h1.root.localScale.x 같은 계산으로 스케일링을 반영함
        startingHandPosition = new Vector3(h1.root.localPosition.x / h1.root.localScale.x,
            h1.root.localPosition.y / h1.root.localScale.y, h1.root.localPosition.z / h1.root.localScale.z);
        finalHandPosition = new Vector3(h2.root.localPosition.x / h2.root.localScale.x,
            h2.root.localPosition.y / h2.root.localScale.y, h2.root.localPosition.z / h2.root.localScale.z);

        //현재 잡은 손의 회전값을 저장함
        startingHandRotation = h1.root.localRotation;

        //목표로 하는 손의 회전값을 저장함
        finalHandRotation = h2.root.localRotation;

        //현재 잡은 손가락 관절 회전 값을 저장함
        startingFingerRotations = new Quaternion[h1.fingerBones.Length];

        //목표로 하는 각 손가락 관절 회전 값을 저장함
        finalFingerRotations = new Quaternion[h1.fingerBones.Length];

        //for문을 통해 각 손가락 관절 회전 값을 저장함
        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }

    }

    public void SendHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }

    private IEnumerator SetHandDataCoroutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation,
        Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        //변화시킬 손의 데이터, 목표 포즈의 손 위치, 목표 포즈의 손 회전, 목표 포즈의 손가락 관절 배열,
        //현재 포즈의 손 위치, 현재 포즈의 손 회전, 현재 포즈의 손가락 관절 배열을 인자로 받음
        //현재 손의 포즈에서, 목표로 하고 싶은 손의 포즈로 부드럽게 변하게 해줌

        float timer = 0f;
        while (timer < poseTransitionDuration)
        {
            //현재 위치에서 목표 위치로 이동 및 회전 실행
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);

            //변화시킬 손의 데이터에 위치 및 회전값 전달
            h.root.localPosition = p;
            h.root.localRotation = r;

            //반복문을 통해 손가락 관절도 같이 전달함
            for (int i = 0; i < newBonesRotation.Length; i++)
            {
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
            }

            timer += Time.deltaTime;

            yield return null;
        }
    }


#if UNITY_EDITOR
    //MenuItem : 유니티 에디터 상단 메뉴바에 커스텀 메뉴를 추가해줌
    //선택하면 현재 선택된 오브젝트에서 GrabHandPose 컴포넌트를 찾아서 오른손 포즈를 기준으로 왼손 포즈를 대칭으로 만들어줌
    [MenuItem("Tools/Mirror Selected Right Grab Pos")]
    public static void MirrorRightPose()
    {
        Debug.Log("Mirror Right Pose");
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    }
#endif

    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror)
    {
        //PoseUsedToMirror : 대칭의 기준이 되는 손 포즈 데이터 (오른손)
        //PoseToMirror : 대칭된 결과를 저장할 손 포즈 데이터 (왼손)

        //손의 위치를 x축 반전해서 대칭 좌표를 만들어줌
        Vector3 mirroredPosition = poseUsedToMirror.root.localPosition;
        mirroredPosition.x *= -1;
        
        //손의 회전값을 y축과 z축을 반전하여 대칭된 회전값을 만들어줌
        //쿼터니언의 반전을 통해 방향이 반대가 되도록 조정함
        Quaternion mirroredQuaternion = poseUsedToMirror.root.localRotation;
        mirroredQuaternion.y *= -1;
        mirroredQuaternion.z *= -1;

        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.localRotation = mirroredQuaternion;

        //손가락 관절 회전값은 그대로 복사함
        for (int i = 0; i < poseUsedToMirror.fingerBones.Length; i++)
        {
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }
    }
}