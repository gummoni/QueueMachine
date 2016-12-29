using System;
using System.Threading.Tasks;

namespace QueueMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var w = new TestModel();

            var p1 = w.GetService<TestController1>();
            var p2 = w.GetService<TestController2>();

            w.Initialize(null);

            p1.Action1();
            p1.Action1();
            p1.Action1();
            p1.Action2();
            p2.Action2();
            p2.Action2();
            p2.Action2();

            Console.WriteLine("-end-");

            Console.ReadKey();
        }
    }

    public class TestModel : Worker
    {
        public int value1 { get; set; }
        public int value2 { get; set; }
    }

    public class TestController1 : ServiceBase<TestModel>
    {
        public TestController1(TestModel model) : base(model)
        {
        }

        public void Action1() => Invoke(() =>
        {
            Console.WriteLine("-1-");
        });

        public void Action2() => Interrupt(() =>
        {
            Console.WriteLine("-2-");
        });
    }

    public class TestController2 : ServiceBase<TestModel>
    {
        public TestController2(TestModel model) : base(model)
        {
        }

        public void Action1() => Invoke(() =>
        {
            Console.WriteLine("-A-");
        });

        public void Action2() => Invoke(() =>
        {
            Console.WriteLine("-B-");
        });
    }
}
