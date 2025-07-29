namespace DeathFortUnoCard.Scripts.Trapped.AISimulation
{
    public class SuicideAISimulation : AISimulation
    {
        protected override void InitializeDices()
        {
            diceValues = new int[6];
        }

        public override IntOrBool EvaluateResult()
        {
            Shake();

            var count = 0;

            for (int i = 0; i < diceValues.Length; i++)
            {
                if (diceValues[i] == selectedValue)
                    count++;
            }

            return IntOrBool.FromInt(count);
        }
    }
}