using Model;

namespace State
{
    public abstract class PlayerStates
    {
        protected PlayerBehaviour P_PlayerBehaviour;

        public PlayerStates(PlayerBehaviour playerBehaviour) => P_PlayerBehaviour = playerBehaviour;

        public virtual void FixedUpdate()
        {
            Idling();
            Moving();
            Jumping();
            Falling();
        }

        //Character States
        public virtual void Idling() { }
        public virtual void Moving() { }
        public virtual void Jumping() { }
        public virtual void Falling() { }
    }
}