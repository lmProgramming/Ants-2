using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class JobUtils
{
    unsafe public static NativeArray<T> GetNativeVertexArrays<T>(NativeArray<T> array, T[] sourceArray) where T : struct
    {
        GCHandle handle = GCHandle.Alloc(sourceArray, GCHandleType.Pinned);
        IntPtr sourcePointer = handle.AddrOfPinnedObject();

        void* destinationPointer = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);

        UnsafeUtility.MemCpy(destinationPointer, (void*)sourcePointer, sourceArray.Length * UnsafeUtility.SizeOf<T>());

        handle.Free();

        return array;
    }

    unsafe public static NativeArray<T> GetNativeVertexArrays<T>(NativeArray<T> array, T[] sourceArray, int startIndex, int endIndex) where T : struct
    {
        int length = endIndex - startIndex;
        if (length <= 0 || startIndex < 0 || endIndex > sourceArray.Length)
            throw new ArgumentException("Invalid range specified." + length + " " + startIndex + " " + endIndex + " " + sourceArray.Length);

        GCHandle handle = GCHandle.Alloc(sourceArray, GCHandleType.Pinned);
        IntPtr sourcePointer = IntPtr.Add(handle.AddrOfPinnedObject(), startIndex * UnsafeUtility.SizeOf<T>());

        void* destinationPointer = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);

        UnsafeUtility.MemCpy(destinationPointer, (void*)sourcePointer, length * UnsafeUtility.SizeOf<T>());

        handle.Free();

        return array;
    }

    //unsafe public static NativeArray<Vector3> GetNativeVertexArrays(NativeArray<Vector3> verts, Vector3[] vertexArray)
    //{
    //    // pin the mesh's vertex buffer in place...
    //    fixed (void* vertexBufferPointer = vertexArray)
    //    {
    //        // ...and use memcpy to copy the Vector3[] into a NativeArray<floar3> without casting. whould be fast!
    //        UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(verts),
    //            vertexBufferPointer, vertexArray.Length * (long)UnsafeUtility.SizeOf<Vector3>());
    //    }
    //    // we only hve to fix the .net array in place, the NativeArray is allocated in the C++ side of the engine and
    //    // wont move arround unexpectedly. We have a pointer to it not a reference! thats basically what fixed does,
    //    // we create a scope where its 'safe' to get a pointer and directly manipulate the array

    //    return verts;
    //}

    //unsafe public static NativeArray<Vector2> GetNativeVertexArrays(NativeArray<Vector2> verts, Vector2[] vertexArray)
    //{
    //    // pin the mesh's vertex buffer in place...
    //    fixed (void* vertexBufferPointer = vertexArray)
    //    {
    //        // ...and use memcpy to copy the Vector3[] into a NativeArray<floar3> without casting. whould be fast!
    //        UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(verts),
    //            vertexBufferPointer, vertexArray.Length * (long)UnsafeUtility.SizeOf<Vector2>());
    //    }
    //    // we only hve to fix the .net array in place, the NativeArray is allocated in the C++ side of the engine and
    //    // wont move arround unexpectedly. We have a pointer to it not a reference! thats basically what fixed does,
    //    // we create a scope where its 'safe' to get a pointer and directly manipulate the array

    //    return verts;
    //}

    //unsafe public static NativeArray<int> GetNativeVertexArrays(NativeArray<int> verts, int[] vertexArray)
    //{
    //    // pin the mesh's vertex buffer in place...
    //    fixed (void* vertexBufferPointer = vertexArray)
    //    {
    //        // ...and use memcpy to copy the Vector3[] into a NativeArray<floar3> without casting. whould be fast!
    //        UnsafeUtility.MemCpy(NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(verts),
    //            vertexBufferPointer, vertexArray.Length * (long)UnsafeUtility.SizeOf<int>());
    //    }
    //    // we only hve to fix the .net array in place, the NativeArray is allocated in the C++ side of the engine and
    //    // wont move arround unexpectedly. We have a pointer to it not a reference! thats basically what fixed does,
    //    // we create a scope where its 'safe' to get a pointer and directly manipulate the array

    //    return verts;
    //}
}
