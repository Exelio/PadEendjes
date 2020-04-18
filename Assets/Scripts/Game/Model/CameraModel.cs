using View;
using UnityEngine;
using System;

namespace Model
{
    public enum CameraStates
    {
        ThirdPersonState, ThirdPersonTransition, FirstPersonTransition, FirstPersonState
    }

    public class CameraModel
    {
        private CameraView _view;
        private CameraVariables _cameraVariables;
        private CameraStates _cameraState;

        private bool _hasChanged = false;
        public bool HasChanged { get => _hasChanged; set { _hasChanged = value; Transition(); } }

        private float _mouseX;
        public float MouseX { get => _mouseX; set { _mouseX = value; } }

        private float _mouseY;
        public float MouseY { get => _mouseY; set { _mouseY = value; } }

        public CameraModel(CameraView view)
        {
            _view = view;
            _cameraVariables = _view.Variables;

            InitializePosition();
        }

        private void InitializePosition()
        {
            _view.Variables.Camera.transform.localPosition = _view.Variables.ThirdPersonCameraTransform.localPosition;
            _cameraState = CameraStates.ThirdPersonState;
            Cursor.lockState = CursorLockMode.Confined;
        }

        public void Update()
        {
            if (_cameraState == CameraStates.ThirdPersonState)
            {                
                LookAtPivot();

                if (_cameraVariables.Target.Stats.Rigidbody.velocity.magnitude > 0.05f)
                    ApplyThirdPersonRotation();

                else IdleRotation();
            }
            if (_cameraState == CameraStates.FirstPersonState) 
            {
                FirstPersonRotation();
            }

            if (_cameraState == CameraStates.FirstPersonTransition || _cameraState == CameraStates.ThirdPersonTransition)
                Transition();

            ApplyFollow();
        }

        public void IdleRotation()
        {
            var angle = Quaternion.AngleAxis(_mouseX * _cameraVariables.MouseRotationSpeed, Vector3.up);
            _view.Transform.rotation = new Quaternion(_view.Transform.rotation.x, _view.Transform.rotation.y + angle.y, _view.Transform.rotation.z, _view.Transform.rotation.w);
        }

        public void FirstPersonRotation()
        {
            var angleHor = _mouseX * _cameraVariables.MouseRotationSpeed;
            var angleVer = _mouseY * _cameraVariables.MouseRotationSpeed;

            Vector3 cameraForward = _cameraVariables.Camera.transform.forward;
            Vector3 playerForward = _cameraVariables.Target.transform.forward;

            float cameraVerticalAngle = Mathf.Sin(cameraForward.y) * Mathf.Rad2Deg;
            float cameraHorizontalAngle = -Mathf.Sin(cameraForward.x) * Mathf.Rad2Deg;

            float playerHorizontalAngle = -Mathf.Sin(playerForward.x) * Mathf.Rad2Deg;

            if (angleVer > 0 && cameraVerticalAngle > _cameraVariables.VerticalLookAngle.x)
                _view.Transform.Rotate(Vector3.right, angleVer, Space.Self);
            else if (angleVer < 0 && cameraVerticalAngle < _cameraVariables.VerticalLookAngle.y)
                _view.Transform.Rotate(Vector3.right, angleVer, Space.Self);

            var horizontalCheckangleMIN = _cameraVariables.HorizontalLookAngle.x + playerHorizontalAngle;
            var horizontalCheckangleMAX = _cameraVariables.HorizontalLookAngle.y + playerHorizontalAngle;            

            if(angleHor > 0 && cameraHorizontalAngle > horizontalCheckangleMIN)
                _view.Transform.Rotate(Vector3.up, angleHor, Space.World);
            else if(angleHor < 0 && cameraHorizontalAngle < horizontalCheckangleMAX)
                _view.Transform.Rotate(Vector3.up, angleHor, Space.World);

            //if (cameraHorizontalAngle < _cameraVariables.HorizontalLookAngle.x + playerHorizontalAngle)
            //    _view.transform.Rotate(Vector3.up, Vector3.Normalize(new Vector3(playerHorizontalAngle, 0, 0)).x * _cameraVariables.SlerpSpeed, Space.World);
            //else if (cameraHorizontalAngle > _cameraVariables.HorizontalLookAngle.y + playerHorizontalAngle)
            //    _view.transform.Rotate(Vector3.up, Vector3.Normalize(new Vector3(playerHorizontalAngle, 0, 0)).x * _cameraVariables.SlerpSpeed, Space.World);
        }

        public void ApplyThirdPersonRotation()
        {
            var slerp = Quaternion.Slerp(_view.Transform.rotation, _cameraVariables.Target.transform.rotation, _cameraVariables.SlerpSpeed);
            _view.Transform.rotation = slerp;
        }

        public void ApplyFollow()
        {
            _view.Position = Vector3.Lerp(_view.Position, _cameraVariables.Target.transform.position, _cameraVariables.SlerpSpeed);
            _view.Position += _cameraVariables.Offset;
        }

        public void Transition()
        {
            Vector3 cameraPos = _cameraVariables.Camera.transform.position;

            switch (_cameraState)
            {
                case CameraStates.ThirdPersonState:
                    if (_hasChanged)
                    {
                        _cameraState = CameraStates.FirstPersonTransition;
                        _hasChanged = false;
                    }
                    break;
                case CameraStates.FirstPersonState:
                    if (_hasChanged)
                    {
                        _cameraState = CameraStates.ThirdPersonTransition;
                        _hasChanged = false;
                    }
                    break;
                case CameraStates.FirstPersonTransition:
                    cameraPos = Vector3.Lerp(cameraPos, _cameraVariables.FirstPersonCameraTransform.position, _cameraVariables.TransitionSpeed * Time.deltaTime);
                    if (Vector3.Distance(cameraPos, _cameraVariables.FirstPersonCameraTransform.position) <= 0.1f)
                        _cameraState = CameraStates.FirstPersonState;
                    break;
                case CameraStates.ThirdPersonTransition:
                    cameraPos = Vector3.Lerp(cameraPos, _cameraVariables.ThirdPersonCameraTransform.position, _cameraVariables.TransitionSpeed * Time.deltaTime);
                    if (Vector3.Distance(cameraPos, _cameraVariables.ThirdPersonCameraTransform.position) <= 0.1f)
                        _cameraState = CameraStates.ThirdPersonState;
                    break;
            }

            _cameraVariables.Camera.transform.position = cameraPos;
        }

        public void LookAtPivot()
        {
            _cameraVariables.Camera.transform.LookAt(_view.Transform);
        }
    }
}