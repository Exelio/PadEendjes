using View;
using Utils;
using UnityEngine;

namespace Model
{
    public class PlayerEngine
    {
        public bool IsGrounded { get => _queryConfig.IsGrounded; }

        private S_PlayerStats _stats;

        private Vector3 _velocity;

        private readonly Vector3 _cameraForwardVector;

        private readonly EnvironmentQueryConfig _queryConfig;
        private readonly EnvironmentQuery _query;

        public PlayerEngine(PlayerView view)
        {
            _stats = view.Stats;
            _queryConfig = view.QueryConfig;
            _cameraForwardVector = Camera.main.transform.forward;

            _query = new EnvironmentQuery(_queryConfig);
        }

        public void FixedPlayerUpdate()
        {
            _queryConfig.Origin = _stats.Transform.position;
            _queryConfig.Radius = _stats.Collider.radius / 2;

            _query.FixedUpdate(_stats.Transform.position + _stats.Rigidbody.centerOfMass);

            Commit();
        }

        public void ApplyGravity()
        {
            _velocity.y -= _stats.Gravity * Time.deltaTime;
        }

        public void ApplyJump()
        {
            _velocity = new Vector3(_velocity.x, Mathf.Sqrt(2 * _stats.JumpHeight), _velocity.z);
        }

        public void ApplyJumpMovement(float horizontal, float vertical)
        {
            _velocity.x += horizontal * Time.deltaTime * _stats.JumpMovementScalar;
            _velocity.z += vertical * Time.deltaTime * _stats.JumpMovementScalar;
        }

        public void ApplyMovement(float horizontal, float vertical)
        {
            _velocity.x = horizontal;
            _velocity.z = vertical;
        }

        public void ApplyIdle()
        {
            _velocity = Vector3.zero;
        }

        private void Commit()
        {
            _stats.Rigidbody.velocity = _velocity * _stats.Speed;
        }

        public void ApplyRotation(float horizontal, float vertical)
        {
            if (horizontal != 0 || vertical != 0)
            {
                Vector3 rotationInput = new Vector3(horizontal, 0f, vertical);
                Vector3 forwardDirection = new Vector3(_cameraForwardVector.x, 0f, _cameraForwardVector.z);

                Quaternion forwardRotation = Quaternion.LookRotation(forwardDirection.normalized);

                Vector3 relativeRotation = forwardRotation * rotationInput;

                _stats.Transform.rotation = Quaternion.Lerp(_stats.Transform.localRotation, Quaternion.LookRotation(relativeRotation), _stats.RotationSpeed);
            }
        }
    }
}