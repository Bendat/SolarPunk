namespace Utils
{
    static class ComparisonUtil
    {
        public static bool IsNull<T>(T toCheck)
        {
            return toCheck == null;
        }

        public static bool NotNull<T>(T toCheck)
        {
            return !IsNull(toCheck);
        }
    }
}
