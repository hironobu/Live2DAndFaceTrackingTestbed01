using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARKit;
using Live2D.Cubism.Core;

public class FaceTracking : MonoBehaviour
{
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private GameObject avatarPrefab;
    
    private CubismModel _live2DModel;
    private ARKitFaceSubsystem _faceSubsystem;
    
    private CubismParameter _faceAngleX;
    private CubismParameter _faceAngleY;
    private CubismParameter _faceAngleZ;
    
    private float _faceAngleXValue1;
    private float _faceAngleYValue1;
    private float _faceAngleZValue1;
    
    private void Start()
    {
        Application.targetFrameRate = 60;
        _live2DModel = avatarPrefab.GetComponent<CubismModel>();
        
        AssociateCubismParametersFromModel(_live2DModel);
    }

    private void OnEnable()
    {
        faceManager.facesChanged += OnFaceChanged;
    }

    private void OnDisable()
    {
        faceManager.facesChanged -= OnFaceChanged;
    }
    
    private void LateUpdate()
    {
        _faceAngleX.Value = _faceAngleXValue1;
        _faceAngleY.Value = _faceAngleYValue1;
        _faceAngleZ.Value = _faceAngleZValue1;
    }
    
    private void OnFaceChanged(ARFacesChangedEventArgs eventArgs)
    {
        if (eventArgs.updated.Count != 0)
        {
            var arFace = eventArgs.updated[0];
            if (arFace.trackingState == TrackingState.Tracking && (ARSession.state > ARSessionState.Ready))
            {
                UpdateFaceTransformValuesFromARFace(arFace);
                UpdateBlendShapeFromARFace(arFace);
            }
        }
    }
    
    private void AssociateCubismParametersFromModel(CubismModel model)
    {
        _faceAngleX = model.Parameters[0];
        _faceAngleY = model.Parameters[1];
        _faceAngleZ = model.Parameters[2];
    }

    private void UpdateFaceTransformValuesFromARFace(ARFace arFace)
    {
        var faceRotation = arFace.transform.rotation;
        
        float a = -1f, b = 2f, c = -2f;
        
        _faceAngleXValue1 = a * MathSupports.NormalizeAngle(faceRotation.eulerAngles.y);
        _faceAngleYValue1 = b * MathSupports.NormalizeAngle(faceRotation.eulerAngles.x);
        _faceAngleZValue1 = c * MathSupports.NormalizeAngle(faceRotation.eulerAngles.z);
    }
    
    private void UpdateBlendShapeFromARFace(ARFace arFace)
    {
        _faceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
        using var blendShapesARKit = _faceSubsystem.GetBlendShapeCoefficients(arFace.trackableId, Allocator.Temp);
        foreach (var featureCoefficient in blendShapesARKit)
        {
            Debug.Log("blendShapeLocation: " + featureCoefficient.blendShapeLocation.ToString());
        }
    }
}

