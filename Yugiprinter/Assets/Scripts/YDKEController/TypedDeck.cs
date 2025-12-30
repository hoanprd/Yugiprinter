using System;
using System.Collections.Generic;

// Chuyển sang Class để dễ quản lý dữ liệu phức hợp
[Serializable]
public class TypedDeck
{
    public List<int> main = new List<int>();
    public List<int> extra = new List<int>();
    public List<int> side = new List<int>();
}