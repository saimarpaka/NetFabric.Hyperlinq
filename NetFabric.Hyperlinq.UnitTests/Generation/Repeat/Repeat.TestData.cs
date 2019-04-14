using System.Collections.Generic;
using Xunit;

namespace NetFabric.Hyperlinq.UnitTests
{
    public static partial class TestData
    {
        public static TheoryData<int, int, IReadOnlyList<int>> Repeat =>
            new TheoryData<int, int, IReadOnlyList<int>> 
            {
                { 0, 0, new int[] {} },
                { 0, 1, new int[] { 0 } },
                { 0, 5, new int[] { 0, 0, 0, 0, 0 } },
                { 5, 5, new int[] { 5, 5, 5, 5, 5 } },
            };

        public static TheoryData<int, int, int, IReadOnlyList<int>> RepeatSkip =>
            new TheoryData<int, int, int, IReadOnlyList<int>> 
            {
                { 1, 5, -1, new int[] { 1, 1, 1, 1, 1 } },
                { 1, 5, 0, new int[] { 1, 1, 1, 1, 1 } },
                { 1, 5, 2, new int[] { 1, 1, 1 } },
                { 1, 5, 10, new int[] { } },
            };

        public static TheoryData<int, int, int, IReadOnlyList<int>> RepeatTake =>
            new TheoryData<int, int, int, IReadOnlyList<int>> 
            {
                { 1, 5, -1, new int[] { } },
                { 1, 5, 0, new int[] { } },
                { 1, 5, 2, new int[] { 1, 1 } },
                { 1, 5, 10, new int[] { 1, 1, 1, 1, 1 } },
            };

        public static TheoryData<int, int, int, int, IReadOnlyList<int>> RepeatSkipTake =>
            new TheoryData<int, int, int, int, IReadOnlyList<int>> 
            {
                { 1, 5, 0, 0, new int[] { } },
                { 1, 5, -1, 0, new int[] { } },
                { 1, 5, 0, -1, new int[] { } },
                { 1, 5, -1, -1, new int[] { } },
                { 1, 5, 0, 2, new int[] { 1, 1 } },
                { 1, 5, 0, 10, new int[] { 1, 1, 1, 1, 1 } },
                { 1, 5, 2, 2, new int[] { 1, 1 } },
                { 1, 5, 2, 10, new int[] { 1, 1, 1 } },
            };
    }
}
