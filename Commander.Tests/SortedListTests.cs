using Commander.Core.Util;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Commander.Tests
{
    public class SortedListTests
    {
        [Fact]
        public void AddsInSortedOrder()
        {
            var elements = new int[] { 54, 3, 5, 34, 23, 1, 2, 9, 8, 39, -23, 4, 2 };
            var elementsSorted = new List<int>(elements);
            elementsSorted.Sort();
            var sortedList = new SortedList<int>();
            sortedList.AddRange(elements);
            sortedList.Should().Equal(elementsSorted);
        }

        [Fact]
        public void AddsWithCustomComparer()
        {
            var comparer = Comparer<int>.Create((x, y) => -x.CompareTo(y));
            var elements = new int[] { 54, 3, 5, 34, 23, 1, 2, 9, 8, 39, -23, 4, 2 };
            var elementsSorted = new List<int>(elements);
            elementsSorted.Sort();
            elementsSorted.Reverse();
            var sortedList = new SortedList<int>(comparer);
            sortedList.AddRange(elements);
            sortedList.Should().ContainInOrder(elementsSorted);
        }
    }
}