using System;
using System.Collections;

namespace QueueMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new UseModel();
            w.Initialize(null);

            var j1 = w.Action1(100);
            var j2 = w.Action1(101);
            var j3 = w.Action1Int(102);
            var j4 = w.Action2();
            var j5 = w.Action2();
            var j6 = w.Action2();
            var j7 = w.Action2();
            Console.WriteLine("--wait start--");
            j7.Wait();
            Console.WriteLine("--wait end--");
            var j8 = w.Action1(103);

            Console.WriteLine("-end-");

            Console.ReadKey();
            w.Dispose();
        }
    }

    public class UseModel : Worker
    {
        public TestModel Model { get; set; } = new TestModel();
        TestController1 C1;
        TestController2 C2;

        IEnumerable WatchTimer()
        {
            while (true)
            {
                var ret = (DateTime.Now.Millisecond % 100) == 0;
                if (ret)
                {
                    Console.WriteLine("fire");
                }
                yield return ret;
            }
        }

        public override void Initialize(object sender)
        {
            C1 = Model.GetService<TestController1>();
            C2 = Model.GetService<TestController2>();
            base.Initialize(sender);
        }

        public Job Action1(int value) => C1.Action1(value);
        public Job Action1Int(int value) => C1.Action2(value);
        public Job Action2() => C2.Action2();

        public override void Dispose()
        {
            base.Dispose();
        }
    }

    public class TestModel : Worker
    {
        public int value1 { get; set; }
        public int value2 { get; set; }

    }

    public class TestController1 : Service<TestModel>
    {
        public TestController1(TestModel model) : base(model)
        {
        }

        public Job Action1(int value) => Invoke(() =>
        {
            Console.WriteLine($"-{value}-");
        });

        public Job Action2(int value) => Interrupt(() =>
        {
            Console.WriteLine($"={value}=");
        });

        /// <summary>
        /// Invoke4種類
        /// </summary>
        /// <returns></returns>
        public Job ActionAA() => Invoke(() =>
        {
            return;
        });

        public Job ActionAB() => Invoke((progress, token) =>
        {
            return;
        });

        public Job<int> ActionBA() => Invoke(() =>
        {
            return 1;
        });

        public Job<int> ActionBB() => Invoke((progress, token) =>
        {
            return 2;
        });

    }

    public class TestController2 : Service<TestModel>
    {
        int value = 1000;
        public TestController2(TestModel model) : base(model)
        {
        }

        public Job Action2() => Invoke(() =>
        {
            Console.WriteLine($"-{value++}-");
        });
    }
}
