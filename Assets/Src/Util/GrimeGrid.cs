using System.Collections.Generic;
using UnityEngine;

public class GrimeGrid
{
    private List<ulong> data;
    private List<byte> setData;
    
    public GrimeGrid(int length, bool initiallySet)
    {
        int dataLength = (length + 63) / 64;
        data = new List<ulong>(dataLength);
        setData = new List<byte>(dataLength);

        int lastBitCount = length % 64;
        
        Debug.Assert(dataLength <= byte.MaxValue);
        
        ulong initialValue = (initiallySet ? ulong.MaxValue : 0);
        for (int i = 0; i < dataLength; i++) {
            if (i == dataLength - 1) {
                // Extra bits need to be zeroed
                initialValue = 0;
                if (initiallySet) {
                    for (int k = 0; k < lastBitCount; k++) {
                        initialValue |= (1ul << k);
                    }
                }
            }
            
            data.Add(initialValue);
            if (initialValue != 0) {
                setData.Add((byte)i);
            }
        }
    }

    public void AddGrime(int index)
    {
        // O(1) if any cell in same data index is not grimey 
        // O(nlgn) if all cells in same data index are not grimey. where n is 64x smaller than grid length
        int dataIndex = index / 64;
        int dataBitIndex = index % 64;

        if (this.data[dataIndex] == 0) {
            setData.Add((byte)dataIndex);
            setData.Sort();
        }
        
        this.data[dataIndex] |= (1ul << dataBitIndex);
    }

    public void RemoveGrid(int index)
    {
        // O(1) if any other cell in same data index is grimey
        // O(n) if all other cells in same data index are not grimey. where n is 64x smaller than grid length
        int dataIndex = index / 64;
        int dataBitIndex = index % 64;

        bool wasAlreadyZero = this.data[dataIndex] == 0;
        this.data[dataIndex] &= (ulong.MaxValue ^ (1ul << dataBitIndex));
        
        if (this.data[dataIndex] == 0 && !wasAlreadyZero) {
            int dataSetIndex = setData.BinarySearch((byte)dataIndex);
            Debug.Assert(dataSetIndex != -1);

            setData.RemoveAt(dataSetIndex);
        }
    }

    public int GetRandomSetIndex()
    {
        // O(1) always
        if (setData.Count <= 0) {
            return -1;
        }
        int randomSetIndex = Random.Range(0, this.setData.Count);
        ushort randomDataIndex = this.setData[randomSetIndex];

        ulong value = this.data[randomDataIndex];
        int bitIndex = 63;
        while ((value & (1ul << bitIndex)) != (1ul << bitIndex)) {
            
            bitIndex--;
            Debug.Assert(bitIndex >= 0);
        }

        int ret = ((int)randomDataIndex) * 64 + bitIndex;
        return ret;
    }
}
