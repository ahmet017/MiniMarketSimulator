using Newtonsoft.Json.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class PlayerController : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool Pickup;
		public bool MoveObj;
		public bool Click;
		public bool RightClick;
        public bool Phone;
		public bool Open;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnPickup(InputValue value)
		{
			PickupInput(value.isPressed);
		}

        public void OnMoveObj(InputValue value)
        {
            MoveObjInput(value.isPressed);
        }
        public void OnClick(InputValue value)
        {
			ClickInput(value.isPressed);
        }
        public void OnRightClick(InputValue value)
        {
            RightClickInput(value.isPressed);
        }
        public void OnOpen(InputValue value)
        {
			OpenInput(value.isPressed);
		}
#endif
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void PickupInput(bool newPickupState)
		{
			Pickup = newPickupState;
		}
        public void MoveObjInput(bool newMoveObjState)
        {
            MoveObj = newMoveObjState;
        }
        public void ClickInput(bool newClickState)
        {
            Click = newClickState;
        }
        public void RightClickInput(bool newRightClickState)
        {
            RightClick = newRightClickState;
        }
        public void OpenInput(bool newOpenState)
        {
            Open = newOpenState;
        }
        public void VirtualPickupInput(bool virtualPickupState)
        {
            PickupInput(virtualPickupState);
        }
        public void VirtualClickInput(bool virtualClickState)
        {
            ClickInput(virtualClickState);
        }

        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}