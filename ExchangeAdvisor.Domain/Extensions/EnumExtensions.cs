using System;

namespace ExchangeAdvisor.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static T GetRandomValue<T>(bool shouldUseZeroValue = false) where T : Enum
        {
            return GetRandomValue<T>(new Random());
        }
        
        public static T GetRandomValue<T>(Random random, bool shouldUseZeroValue = false) where T : Enum
        {
            var values = GetValues<T>();
            var minValueIndex = shouldUseZeroValue ? 0 : 1;
            var maxValueIndex = values.Length;
            var randomValueIndex = random.Next(minValueIndex, maxValueIndex);

            return values[randomValueIndex];
        }

        private static T[] GetValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static T ToEnum<T>(string enumValueName) where T : Enum
        {
            return (T)Enum.Parse(typeof(T), enumValueName);
        }
    }
}
