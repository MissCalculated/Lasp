using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Lasp
{
    // Extension methods for NativeArray/NativeSlice <-> ReadOnlySpan conversion
    static class SpanNativeArraySliceExtensions
    {
        public unsafe static NativeSlice<T>
          GetNativeSlice<T>(this ReadOnlySpan<T> span, int offset, int stride)
          where T : unmanaged
        {
            fixed (void* ptr = &span.GetPinnableReference())
            {
                var headPtr = (T*)ptr + offset;
                var strideInByte = sizeof(T) * stride;
                var elementCount = span.Length / stride - offset / stride;

                var slice =
                  NativeSliceUnsafeUtility.ConvertExistingDataToNativeSlice<T>
                  (headPtr, strideInByte, elementCount);

              #if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeSliceUnsafeUtility.SetAtomicSafetyHandle
                  (ref slice, AtomicSafetyHandle.GetTempUnsafePtrSliceHandle());
              #endif

                return slice;
            }
        }

        public unsafe static NativeSlice<T>
          GetNativeSlice<T>(this ReadOnlySpan<T> span)
          where T : unmanaged
          => GetNativeSlice(span, 0, 1);
    }

    // Extension methods for List<T>
    static class ListExtensions
    {
        // Find and retrieve an entry with removing it
        public static T FindAndRemove<T>(this List<T> list, Predicate<T> match)
        {
            var index = list.FindIndex(match);
            if (index < 0) return default(T);
            var res = list[index];
            list.RemoveAt(index);
            return res;
        }
    }

    // Math utility functions
    static class MathUtils
    {
        // Decibel (full scale) calculation
        // Reference level (full scale sin wave) = 1/sqrt(2)
        public static float dBFS(float p)
          => 20 * math.log10(p / 0.7071f + 1.5849e-13f);
    }
}
