using System;

public class Myatt : Attribute
{
    public int arrayLen;

    public Myatt(int len)
    {
        arrayLen = len;
    }
}