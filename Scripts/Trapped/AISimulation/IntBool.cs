namespace DeathFortUnoCard.Scripts.Trapped.AISimulation
{
    [System.Serializable]
    public readonly struct IntOrBool
    {
        public enum ValueType {None, Int, Bool}
        
        public ValueType Type { get; }
        private readonly int _intValue;
        private readonly bool _boolValue;

        public int IntValue => Type == ValueType.Int ? _intValue : throw new System.InvalidOperationException();
        public bool BoolValue => Type == ValueType.Bool ? _boolValue : throw new System.InvalidOperationException();

        private IntOrBool(int value)
        {
            _intValue = value;
            _boolValue = default;
            Type = ValueType.Int;
        }

        private IntOrBool(bool value)
        {
            _intValue = default;
            _boolValue = value;
            Type = ValueType.Bool;
        }

        public static IntOrBool FromInt(int value) => new(value);
        public static IntOrBool FromBool(bool value) => new(value);

        public bool TryGetInt(out int value)
        {
            value = _intValue;
            return Type == ValueType.Int;
        }

        public bool TryGetBool(out bool value)
        {
            value = _boolValue;
            return Type == ValueType.Bool;
        }

        public override string ToString()
            => Type switch
            {
                ValueType.Int => _intValue.ToString(),
                ValueType.Bool => _boolValue.ToString(),
                _ => "None"
            };
    }
}