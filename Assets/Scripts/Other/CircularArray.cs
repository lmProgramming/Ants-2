using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PseudoCircularArray<T> : List<T>
{
    private int pointer = 0;
    public int Length { get; private set; }

    public PseudoCircularArray(int length) : base(length)
    {
        for (int i = 0; i < length; i++)
        {
            Add(default);
        }
        Length = length;
    }

    public void ReplaceLast(T value)
    {
        this[pointer] = value;
        pointer = (pointer + 1) % Length;
    }

    public void FillWith(T value)
    {
        for (int i = 0; i < Length; i++)
        {
            this[i] = value;
        }
    }
}
