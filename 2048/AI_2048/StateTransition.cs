namespace AI_2048
{
    public enum MoveDir { Left, Right, Up, Down, Self };

    public class StateTransition
    {
        public MoveDir dir;
		public Game nextState;
        public double rating = 0;

        public StateTransition(Game state, MoveDir dir)
		{
			this.dir = dir;
            this.nextState = state;
		}
    }
}
