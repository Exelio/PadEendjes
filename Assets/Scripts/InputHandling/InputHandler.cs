using Boxsun.Math;
using UnityEngine;

namespace InputHandling
{
    public class InputHandler
    {
        public IImpulseCommand ACommand;

        public IDirectionalCommand RightStickCommand;
        public IDirectionalCommand LeftStickCommand;

        public void Update()
        {
            if (Input.GetButtonDown("AButton"))
                ACommand?.Execute(true);

            float horizontalLeft = Input.GetAxis("LStickHorizontal");
            float verticalLeft = Input.GetAxis("LStickVertical");

            Vector2 directionLeft = new Vector2(horizontalLeft, verticalLeft);

            directionLeft = directionLeft.sqrMagnitude > 1 ? directionLeft.normalized : directionLeft;

            LeftStickCommand?.Execute(MathB.Vector2Conversion(directionLeft.x, directionLeft.y));
        }
    }
}