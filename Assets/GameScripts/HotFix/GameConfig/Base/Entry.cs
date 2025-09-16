using Newtonsoft.Json;

namespace GameConfig
{
    public readonly struct Int2IntVal
    {
        [JsonConstructor]
        public Int2IntVal(int k, int v)
        {
            K = k;
            V = v;
        }

        public int K { get; }
        public int V { get; }
        
        public override string ToString()
        {
            return $"Int2IntVal {{ k = {K}, v = {V} }}";
        }
        
    }
    public readonly struct Int2FloatVal
    {
        [JsonConstructor]
        public Int2FloatVal(int k, float v)
        {
            K = k;
            V = v;
        }

        public int K { get; }
        public float V { get; }
        
        public override string ToString()
        {
            return $"Int2FloatVal {{ k = {K}, v = {V} }}";
        }
        
    }
    public readonly struct Int2LongVal
    {
        [JsonConstructor]
        public Int2LongVal(int k, long v)
        {
            K = k;
            V = v;
        }

        public int K { get; }
        public long V { get; }
        
        public override string ToString()
        {
            return $"Int2LongVal {{ k = {K}, v = {V} }}";
        }
        
    }
}