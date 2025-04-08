using UnityEngine;

namespace Agni
{
	public class CharacterController : MonoBehaviour
	{
		#region Variables
		[Header("Movement Settings")]
		public float moveSpeed = 5f;
		public float acceleration = 10f;
		
		
		[Header("Cinemachine Settings")]
		public CinemachineVirtualCamera virtualCamera;
		public float cameraTopRigHeight = 4f;
		public float cameraRadius = 2f;
		public Vector2 cameraYClamp = new Vector2(-20, 80);

		private Vector3 velocity;
		private bool isGrounded;
		private Transform cameraTransform;
		private Cinemachine3rdPersonFollow thirdPersonFollow;
		private float currentCameraX;
		private float currentCameraY;
		

		#endregion
		
		
		
		void Start()
		{
        
		}

		// Update is called once per frame
		void Update()
		{
        
		}
	}
}


