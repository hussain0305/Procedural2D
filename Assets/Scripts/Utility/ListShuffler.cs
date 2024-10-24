using System;
using System.Collections.Generic;

public static class ListShuffler
{
    private static Random random = new Random();
    
    public static int[] GenerateRandomIndexArray(int size)
    {
        int[] indices = new int[size];
        for (int i = 0; i < size; i++)
        {
            indices[i] = i;
        }
        for (int i = 0; i < size; i++)
        {
            int j = random.Next(i, size);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        return indices;
    }
}