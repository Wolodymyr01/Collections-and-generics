using System;
using System.Linq;
using static System.Console;
using static System.Math;

namespace GCs
{
    public class Program
    {
        static void Main(string[] args)
        {
            var coll = new CollectionType<int>(new int[] { 1, 3, 7, 19, 31, 7, 87 });
            WriteLine("Linqed List example:\n\n" + coll.ToString());
            coll.RemoveAt(0); // remove 1
            coll.AddLast(7); // add 7 to the end
            coll.AddFirst(2); // add 2 to the begin
            coll.RemoveAll(7); // remove all 7
            coll.AddBefore(30, 3); // add 30 before 31
            coll.AddAfter(32, 4); // add 32 after 31
            WriteLine("remove the first element, add '7' to the end, add '2' to the begin, remove all '7', add '30' " +
                "before '31' and '32' after it\n" + coll.ToString());
            CollectionType<int> coll2 = new CollectionType<int>();
            coll.CopyTo(coll2);
            coll.Clear();
            coll.AddRange(new int[] { 2, 3, 4, 5, 6, 7, 8, 9 });
            coll.AddFirst(1);
            WriteLine("Copy all to another list, clear the old one and fill it again with natural numbers till 9\n" + coll.ToString());
            WriteLine("The new list has saved values that were copied, while the old was changed:\n" + coll2.ToString() + "\n\nTask (variant 1):\n");
            Triangle[][] tris = new Triangle[4][];
            CollectionType<Triangle>[] arr_coll = new CollectionType<Triangle>[4];
            Random r = new Random();
            int a, b, c, n = 1;
            for (int i = 0; i < tris.Length; i++)
            {
                tris[i] = new Triangle[r.Next(5, 15)];
                for (int j = 0; j < tris[i].Length; j++)
                {
                    a = r.Next(1, 20);
                    b = r.Next(1, 20);
                    c = (int)Sqrt(a * a + b * b - a * b * Cos(r.NextDouble()));
                    if (c == 0) c = a + b - 1;
                    try
                    {
                        tris[i][j] = new Triangle(a, b, c);
                    }
                    catch (ArgumentException)
                    {
                        tris[i][j] = new Triangle(3 * n, 4 * n, 5 * n);
                        n++;
                    }
                }
            }
            WriteLine("Non-sorted array:");
            foreach (var item in tris)
            {
                foreach (var sub in item)
                {
                    WriteLine(sub.ToString());
                }
            }
            WriteLine("\n Sorted collections");
            for (int i = 0; i < tris.Length; i++)
            {
                Array.Sort<Triangle>(tris[i]);
                WriteLine($"\n collection {i}\n");
                foreach (var item in tris[i])
                {
                    WriteLine(item.ToString());
                }
                arr_coll[i] = new CollectionType<Triangle>(tris[i]);
            }
            WriteLine("\n Enter n");
            n = Convert.ToInt32(ReadLine());
            // LINQ
            var n_col =
                from elem in arr_coll
                where elem.Length == n
                select elem;
            if (n_col.Count() == 0) WriteLine("There is no collection with such length");
            foreach (var item in n_col)
            {
                WriteLine(item.ToString());
            }
            var max = arr_coll.Aggregate((i1, i2) => i1.Length > i2.Length ? i1 : i2);
            var min = arr_coll.Aggregate((i1, i2) => i1.Length < i2.Length ? i1 : i2);
            WriteLine("Minimal collection:\n" + min.ToString());
            WriteLine("Maximal collection:\n" + max.ToString());
        }
    }
    public class Triangle : IComparable<Triangle>
    {
        public Triangle(float a, float b, float c)
        {
            if ((a + b) > c && (b + c) > a && (a + c) > b && (a > 0) && (b > 0) && (c > 0))
            {
                this.a = a; this.b = b; this.c = c;
            }
            else
            {
                throw new ArgumentException("Such triangle does not exist");
            }
        }
        public readonly float a, b, c;
        public float Length()
        {
            return a + b + c;
        }
        public float Square() // heron's method
        {
            float p = Length() / 2;
            return (float)Sqrt(p * (p - a) * (p - b) * (p - c));
        }
        public int CompareTo(Triangle tri)
        {
            if (tri.Square() == Square()) return 0;
            if (tri.Square() > Square()) return -1;
            else return 1;
        }
        public override bool Equals(object obj)
        {
            if ((obj as Triangle).Square() == Square()) return true;
            return false;
        }
        public override int GetHashCode()
        {
            return 3*a.GetHashCode() + 2*b.GetHashCode() + c.GetHashCode();
        }
        public override string ToString()
        {
            return $"triangle with sides: {a}, {b}, {c}, square: {Square()}";
        }
    }
    public class CollectionType<T>
    {
        public CollectionType()
        {

        }
        public CollectionType(T[] arr)
        {
            Length = arr.Length;
            if (arr.Length > 0)
            {
                first = new Item<T>();
                first.value = arr[0];
                var temp = first;
                for (int i = 1; i < arr.Length; i++)
                {
                    temp.Next = new Item<T>(arr[i], temp);
                    temp = temp.Next;
                }
            }
        }
        public T this[int index]
        {
            get
            {
                if (index == 0) return first.value;
                return Reach(index).value;
            }
            set
            {
                if (index == 0) first.value = value;
                Reach(index).value = value;
            }
        }
        Item<T> first = null;
        public int Length { get; protected set; } = 0;
        Item<T> Reach(int index)
        {
            var temp = first;
            if (temp == null) return null;
            for (int i = 0; i < index; i++)
            {
                temp = temp.Next;
            }
            return temp;
        }
        public void AddAfter(T item, int index)
        {
            var Ite = new Item<T>(item);
            var prev = Reach(index) ?? throw new IndexOutOfRangeException("This element does not exist");
            var next = prev.Next;
            Ite.Previous = prev; Ite.Next = next;
            prev.Next = Ite;
            next.Previous = Ite;
            Length++;
        }
        public void AddBefore(T item, int index)
        {
            var Ite = new Item<T>(item);
            var next = Reach(index) ?? throw new IndexOutOfRangeException("This element does not exist");
            var prev = next.Previous;
            Ite.Previous = prev; Ite.Next = next;
            prev.Next = Ite;
            next.Previous = Ite;
            Length++;
        }
        public void AddFirst(T item)
        {
            var temp = Reach(Length - 1);
            temp.Next = new Item<T>(temp.value, temp);
            while (temp != null)
            {
                temp.Next.value = temp.value;
                temp = temp.Previous;
            }
            first.value = item;
            Length++;
        }
        public void AddLast(T item)
        {
            var temp = Reach(Length - 1);
            temp.Next = new Item<T>(item, temp);
            Length++;
        }
        public void AddRange(T[] arr)
        {
            Length += arr.Length;
            var temp = Reach(Length - 1) ?? new Item<T>(arr[0]);
            if (first == null) first = temp;
            if (!temp.value.Equals(arr[0]))
            {
                temp.Next = new Item<T>(arr[0], temp);
                temp = temp.Next;
            }
            for (int i = 1; i < arr.Length; i++)
            {
                temp.Next = new Item<T>(arr[i], temp);
                temp = temp.Next;
            }
        }
        public void Clear()
        {
            first = null;
            Length = 0;
        }
        public void CopyTo(CollectionType<T> ct)
        {
            ct.first = first;
            ct.Length = Length;
        }
        public void RemoveAt(int index)
        {
            var item = Reach(index);
            if (index > 0)
            {
                var prev = item.Previous;
                if (Length - index > 1)
                {
                    var next = item.Next;
                    prev.Next = next;
                    next.Previous = prev;
                }
                else
                {
                    prev.Next = null;
                }
            }
            else
            {
                first = first.Next;
                first.Previous = null;
            }
            Length--;
        }
        public void RemoveAll(T item)
        {
            while (Remove(item)) ;
        }
        public bool Remove(T item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (item.Equals(this[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public void RemoveLast(T item)
        {
            for (int i = Length; i >= 0; i--)
            {
                if (item.Equals(this[i]))
                {
                    RemoveAt(i);
                    return;
                }
            }
        }
        public int Find(T item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i].Equals(item)) return i;
            }
            return -1;
        }
        public int FindLast(T item)
        {
            for (int i = Length - 1; i >= 0; i--)
            {
                if (this[i].Equals(item)) return i;
            }
            return -1;
        }
        public int Count(T item)
        {
            int cnt = 0;
            for (int i = 0; i < Length; i++)
            {
                if (this[i].Equals(item)) cnt++;
            }
            return cnt;
        }
        public bool Contains(T item)
        {
            for (int i = 0; i < Length; i++)
            {
                if (this[i].Equals(item)) return true;
            }
            return false;
        }
        public T[] ToArray()
        {
            T[] arr = new T[Length];
            var temp = first;
            for (int i = 0; i < Length; i++)
            {
                arr[i] = temp.value;
                temp = temp.Next;
            }
            return arr;
        }
        public override string ToString()
        {
            string s = null;
            for (int i = 0; i < Length; i++)
            {
                s += this[i].ToString() + "\n";
            }
            return s;
        }
    }
    public class Item<T>
    {
        public Item()
        {

        }
        public Item(T v, Item<T> p = null, Item<T> n = null)
        {
            value = v;
            Next = n;
            Previous = p;
        }
        public T value;
        public Item<T> Next { get; set; } = null;
        public Item<T> Previous { get; set; } = null;

    }
}