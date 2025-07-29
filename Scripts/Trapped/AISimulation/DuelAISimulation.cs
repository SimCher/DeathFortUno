namespace DeathFortUnoCard.Scripts.Trapped.AISimulation
{
    public class DuelAISimulation : AISimulation
    {
        protected override void InitializeDices()
        {
            diceValues = new int[3];
        }

        public override IntOrBool EvaluateResult()
        {
            Shake();

            for (int i = 0; i < diceValues.Length; i++)
            {
                if (diceValues[i] == selectedValue)
                {
                    return IntOrBool.FromBool(true);
                }
            }

            return IntOrBool.FromBool(false);
        }
    }
}