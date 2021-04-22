namespace ZyncAudio
{
    public static class IntegerExtensions
    {

        /// <summary>
        /// Computes the Modulo for the given <paramref name="dividend"/> and <paramref name="divisor"/>.
        /// </summary>
        /// <remarks>This implementation differs from using the C# `%` (Remainder) operator as this implementation works on negative numbers too.</remarks>
        public static int Mod(this int dividend, int divisor)
        {
            return dividend < 0 ? ((dividend + 1) % divisor + divisor - 1) : (dividend % divisor);
        }

    }
}
