using Boxsun.Math;
using UnityEngine;

namespace InputHandling
{
    public class InputHandler
    {
        public IImpulseCommand YCommand;

        public IDirectionalCommand LeftStickCommand;
        public IDirectionalCommand RightStickCommand;

        public void Update()
        {
            if (Input.GetButtonDown("YButton"))
                YCommand?.Execute();

            float horizontalLeft = Input.GetAxis("LStickHorizontal");
            float verticalLeft = Input.GetAxis("LStickVertical");

            float horizontalRight = Input.GetAxis("RStickHorizontal");
            float verticalRight = Input.GetAxis("RStickVertical");

            Vector2 directionLeft = new Vector2(horizontalLeft, verticalLeft);
            Vector2 directionRight = new Vector2(horizontalRight, verticalRight);

            directionLeft = directionLeft.sqrMagnitude > 1 ? directionLeft.normalized : directionLeft;

            LeftStickCommand?.Execute(MathB.Vector2Conversion(directionLeft.x, directionLeft.y));
            RightStickCommand?.Execute(MathB.Vector2Conversion(directionRight.x, directionRight.y));
        }
    }
}