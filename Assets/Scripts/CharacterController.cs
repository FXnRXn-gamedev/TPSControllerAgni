using Unity.Cinemachine;
using UnityEngine;

namespace Agni
{
	public class CharacterController : MonoBehaviour
	{
		#region Variables
		[Header("Movement Settings")]
		[SerializeField] private float					moveSpeed				= 5f;
		[SerializeField] private float					acceleration			= 10f;
		[SerializeField] private float					groundFriction			= 5f;
		[SerializeField] private float					airFriction				= 1f;

		[Header("Jump/Gravity Settings")]
		[SerializeField] private float					jumpForce				= 7f;
		[SerializeField] private float					gravity					= 9.81f;

		[Header("Collision Settings")] 
		[SerializeField] private Transform				collisionOrigin;
		[SerializeField] private float					groundCheckDistance		= 0.1f;
		[SerializeField] private float					collisionCheckDistance	= 0.1f;
		[SerializeField] private float					characterHeight			= 2f;
		[SerializeField] private float					characterRadius			= 0.5f;
		[SerializeField] private LayerMask				collisionLayers;
		
		
		[Header("Cinemachine Settings")]
		public CinemachineCamera virtualCamera;
		private Transform cameraTransform;



		private Vector2				input;
		private bool				isGrounded;
		private Vector3				velocity;
		private Vector3				moveDirection;

		#endregion
		
		
		
		void Start()
		{
			InitializeCinemachineCamera();
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void InitializeCinemachineCamera()
		{
			if (virtualCamera != null)
			{
				cameraTransform = virtualCamera.transform;
			}
		}

		// Update is called once per frame
		void Update()
		{
			HandleInput();
			moveDirection = CalculateMovementDirection(input);
			
			Vector3 desiredVelocity = CalculateVelocity(moveDirection);
			Vector3 finalVelocity = HandleCollisions(desiredVelocity);

			velocity = finalVelocity;
			transform.position += velocity * Time.deltaTime;
			
		}
		
		
		
		
		//--------------------------------------------------------------------------------------------------------------
		private void HandleInput()
		{
			input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			
		}
		
		Vector3 CalculateMovementDirection(Vector2 input)
		{
			if (cameraTransform == null) return Vector3.zero;

			Vector3 forward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
			Vector3 right = cameraTransform.right;
			return (input.y * forward + input.x * right).normalized;
		}
		
		
		Vector3 CalculateVelocity(Vector3 moveDirection)
		{
			Vector3 newVelocity = velocity;
        
			// Horizontal movement
			Vector3 targetVelocity = moveDirection * moveSpeed;
			Vector3 horizontalVelocity = Vector3.MoveTowards(
				new Vector3(newVelocity.x, 0, newVelocity.z),
				targetVelocity,
				acceleration * Time.deltaTime
			);

			float currentFriction = isGrounded ? groundFriction : airFriction;
			horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, currentFriction * Time.deltaTime);

			// Vertical movement
			if (isGrounded)
			{
				if (Input.GetButtonDown("Jump"))
				{
					newVelocity.y = jumpForce;
					isGrounded = false;
				}
				else
				{
					newVelocity.y = Mathf.Max(newVelocity.y, 0);
				}
			}
			else
			{
				newVelocity.y -= gravity * Time.deltaTime;
			}

			newVelocity.x = horizontalVelocity.x;
			newVelocity.z = horizontalVelocity.z;

			return newVelocity;
		}
		
		Vector3 HandleCollisions(Vector3 desiredVelocity)
		{
			Vector3 adjustedVelocity = desiredVelocity;
			Vector3 moveDelta = desiredVelocity * Time.deltaTime;

			// Vertical collision
			if (moveDelta.y != 0)
			{
				float direction = Mathf.Sign(moveDelta.y);
				if (Physics.SphereCast(
					collisionOrigin.position,
					characterRadius,
					Vector3.up * direction,
					out RaycastHit hit,
					Mathf.Abs(moveDelta.y) + collisionCheckDistance,
					collisionLayers))
				{
					adjustedVelocity.y = 0;
					transform.position += Vector3.up * direction * (hit.distance - collisionCheckDistance);
				}
			}

			// Horizontal collision
			Vector3 horizontalDelta = new Vector3(moveDelta.x, 0, moveDelta.z);
			if (horizontalDelta.magnitude > 0)
			{
				if (Physics.SphereCast(
					collisionOrigin.position,
					characterRadius,
					horizontalDelta.normalized,
					out RaycastHit hit,
					horizontalDelta.magnitude + collisionCheckDistance,
					collisionLayers))
				{
					Vector3 projectedVelocity = Vector3.ProjectOnPlane(adjustedVelocity, hit.normal);
					adjustedVelocity = new Vector3(projectedVelocity.x, adjustedVelocity.y, projectedVelocity.z);
					transform.position += horizontalDelta.normalized * (hit.distance - collisionCheckDistance);
				}
			}

			return adjustedVelocity;
		}
		
		
		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(collisionOrigin.position + Vector3.down * (characterHeight / 2 - characterRadius), characterRadius);
		}
		
		
		
	} // NameSpace END
}


