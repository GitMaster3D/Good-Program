//#define DEBUGLIST

using System.Collections;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks.Dataflow;

public class Program
{
    public static void Main()
    {
        NotAList notAList = new NotAList();
        object A = 5;
        notAList.Add(A);
        notAList.Add(7);

        foreach ( var v in notAList)
        {
            Console.WriteLine(v);
        }

        Console.WriteLine();


        notAList.Remove(5, NotAList.RemoveBy.Refrence);

        for (int i = 0; i < notAList.Count; i++)
        {
            Console.WriteLine(notAList[i]);
        }
        Console.WriteLine();
        Console.WriteLine();


        Console.WriteLine("found Indexes");
        int[] ints = FindAll(new object[] { 
            4,
            "hahn",
            1, 
            0xFF, 
            "hahn", 
            new List<int>(),
            "hahn", 
            "zwölf", 
            new StringBuilder(), 
            0.23f,
            "hahn",
            0b10011101
        },
        "hahn"
        );

        for (int i = 0; i < ints.Length; i++)
        {
            Console.WriteLine(ints[i]);
        }
        Console.WriteLine("End Indexes");




        object[] arr = new object[] { 1, "llllll", 0b10101011, 0xFFF12, 0 };
        ChangeArrayValue(ref arr, 2, new Exception("Lidl"));
        ChangeArrayValue(ref arr, arr.Length - 1, ContainsValue(arr, 0));

        WriteArray(arr);

        Console.WriteLine(LargestValue(new int[] { 5, 2, 8, 4, 5 }));
        Console.WriteLine(ArrayOfLength10()[9] = 42);
        Console.WriteLine(FindIndex(new int[] { 5, 2, 1, 7 }, 1));
        Console.WriteLine(IncreaseArraySize(new object[] { 5, 2, 8, 4, 5 }).Length);

        Console.WriteLine("Remove index Array:");
        object[] arr_ = new object[] { 1, 5, 2, 6, 7, 3, 6 };
        arr_ = RemoveFromArray(arr_, 3);
        foreach(var v in arr_)
        {
            Console.WriteLine(v);
        }

        double[] inputs = new double[2];

    start:;
        try
        {
            Console.WriteLine("Input Base, then Exponent");

            float a = float.Parse(Console.ReadLine());
            float b = float.Parse(Console.ReadLine());

            foreach (var d in inputs)
            {
                if (d == double.PositiveInfinity || d == double.NegativeInfinity)
                    throw new Exception("Overflow while parsing");
            }

            inputs[0] = a;
            inputs[1] = b;
            
        }
        catch
        {
            Console.WriteLine("Error while parsing inputs");
            goto start;
        }

        try
        {
            double result = Math.Pow(inputs[0], inputs[1]);

            if (result == double.PositiveInfinity || result == double.NegativeInfinity)
                throw new Exception("Overflow");

            Console.WriteLine(result);
        }
        catch
        {
            Console.WriteLine("Error while calculating value");
        }

        Console.ReadKey();
    }

    public static object[] RemoveFromArray(object[] arr, int index)
    {
        if (index >= arr.Length) throw new Exception("index was higher than arr length");

        object[] newArr = new object[arr.Length - 1];

        int j = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            if (i == index) continue;
            else
            {
                newArr[j] = arr[i];
                j++;
            }
        }

        return newArr;
    }

    public static int[] FindAll(object[] obj, object value)
    {
        NotAList indexes = new NotAList();

        for (int i = 0; i < obj.Length; i++)
        {
            if (obj[i] == value) indexes.Add(i);
        }

        int[] result = new int[indexes.Count];
        for (int i = 0; i < indexes.Count; i++)
        {
            result[i] = (int)indexes[i];
        }
        return result;
    }

    public class NotAList : IEnumerable, IEnumerator
    {
        object[] items;
        public int Count
        {
            get 
            {
                return GetCount();
            }
        }
        private int position = -1;

        public bool MoveNext()
        {
            position++;
            return position < Count;
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current
        {
            get
            {
                return items[position];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }

        public object this[int index]
        {
            get => GetItem(index);
            set => SetItem(index, value);
        }

        public NotAList(uint startItems = 16)
        {
            if (startItems < 1) throw new Exception("Cannot be initialized with value smaller than 1");

            this.items = new object[startItems];

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new Empty();
            }
        }

        public void Flush(int startSize = 16)
        {
            items = new object[startSize];
        }

        public void RemoveAt(int index)
        {
            items = RemoveFromArray(items, index);
        }

        public void Remove(object item, RemoveBy removeBy = RemoveBy.Refrence, RemoveMode mode = RemoveMode.All)
        {
            for (int i = 0; i < items.Length; i++)
            {
#if DEBUGLIST
                Console.WriteLine("Valuecheck " + items[i].Equals(item));
                Console.WriteLine("Valuecheck as int " + Convert.ToInt32(items[i].Equals(item)));
                Console.WriteLine("Removemode " + (int)removeBy);
                Console.WriteLine("Final Valuecheck " + (((Convert.ToInt32(items[i].Equals(item)) & (int)removeBy)) == 1));

                Console.WriteLine("RefrenceCheck " + Convert.ToInt32((items[i] == item)));
                Console.WriteLine("added Remove mode " + Convert.ToString(((int)removeBy) & (0b00000100)));
                Console.WriteLine("Final refrencecheck " + ((Convert.ToInt32((items[i] == item)) + ((int)removeBy & 0b00000100)) == 5));
                Console.WriteLine();
                Console.WriteLine();
#endif
                if ((((Convert.ToInt32(items[i].Equals(item)) & (int)removeBy)) == 1) //Check by value
                    || (Convert.ToInt32(items[i] == item) + ((int)removeBy & 0b00000100)) == 5) //Check by refrence
                {
                    items = RemoveFromArray(items, i);
                    i--;

                    if (mode == RemoveMode.FirstInstance) return;
                }
            }
        }

        public enum RemoveMode
        {
            All = 0,
            FirstInstance = 1
        }

        public enum RemoveBy
        {
            Value = 0b00000001,
            Refrence = 0b00000100
        }

        private static object[] RemoveFromArray(object[] arr, int index)
        {
            if (index >= arr.Length) throw new Exception("index was higher than arr length");

            object[] newArr = new object[arr.Length - 1];

            int j = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (i == index) continue;
                else
                {
                    newArr[j] = arr[i];
                    j++;
                }
            }

            return newArr;
        }

        public object[] ToArray()
        {
            object[] arr = new object[items.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = items[i];
            }
            return arr;
        }

        private int GetCount()
        {
            int count = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is Empty)
                {
                    return count;
                }
                count++;
            }
            return count;
        }

        private void SetItem(int index, object item)
        {
            if (index < Count)
            {
                items[index] = item;
                return;
            }
            throw new Exception("Index out of bounds");
        }

        private object GetItem(int index)
        {
            if (!(items[index] is Empty))
            {
                return items[index];
            }
            else
            {
                throw new Exception("Outside of bounds");
            }
        }

        public void Add(object item)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] is Empty)
                {
                    items[i] = item;
                    return;
                }
            }

            int i_ = items.Length;
            object[] items_ = new object[items.Length * 2];
            for (int i = 0; i < items.Length; i++)
            {
                items_[i] = items[i];
            }

            for (int i = i_; i < items_.Length; i++)
            {
                if (items_[i] == null) items_[i] = new Empty();
            }

            items = items_;
            Add(item);
        }

        private struct Empty
        {}
    }



    public static void WriteArray(object[] obj)
    {
        foreach(var o in obj)
        {
            Console.WriteLine(o);
        }
    }

    public static int FindIndex(int[] arr, int value)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == value)
                return i;
        }

        return -1;
    }

    public static object[] IncreaseArraySize(object[] arr)
    {
        object[] newArr = new object[arr.Length + 1];

        for (int i = 0; i < arr.Length; i++)
        {
            newArr[i] = arr[i];
        }

        return newArr;
    }

    public static void ChangeArrayValue(ref object[] arr, int index, object value)
    {
        arr[index] = value;
    }

    public static bool ContainsValue(object[] arr, object value)
    {
        return arr.Contains(value);
    }

    public static int LargestValue(int[] arr)
    {
        int largestValue = int.MinValue;

        foreach (var item in arr)
        {
            largestValue = item > largestValue ? item : largestValue;
        }

        return largestValue;
    }

    public static int[] ArrayOfLength10()
    {
        return new int[10];
    }



    public static List<List<int>> large;
    public static unsafe void ManyExceptions()
    {
        large = new List<List<int>>();

        for (int i = 0; i < 100; i++)
        {
            Thread t = new Thread(() =>
            {
                List<int> coolList = new List<int>();
                while (true)
                {
                    coolList.Add(1);

                    if (coolList.Count % 100000 == 0)
                    {
                        lock (large)
                        {
                            large.Add(coolList);
                        }
                    }
                }
            });
            t.Start();
        }
    }
}