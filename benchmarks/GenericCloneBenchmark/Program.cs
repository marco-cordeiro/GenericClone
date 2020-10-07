using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using GenericClone;

namespace GenericCloneBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<CloningBenchmark>();
        }
    }

    [RPlotExporter]
    public class CloningBenchmark
    {
        private static readonly PocoClass Subject = CreateSubject();

        [Benchmark]
        public PocoClass ReflectionShallowClone() => Subject.ShallowClone1();

        [Benchmark]
        public PocoClass ReflectionShallowCloneWithCachedMethod() => Subject.ShallowClone2();

        [Benchmark]
        public PocoClass ReflectionDeepCloneWithComplexFieldTraversingMethod() => Subject.DeepClone1();

        [Benchmark]
        public PocoClass ReflectionDeepCloneWithFullFieldTraversingMethod() => Subject.DeepClone2();

        private static PocoClass CreateSubject()
        {
            return new PocoClass
            {
                IntProperty = 13, StringProperty = "Foo",
                ComplexProperty = new PocoClass
                {
                    IntProperty = 7,
                    StringProperty = "bar"
                },
                IntCollection = new List<int> {1, 3, 4},
                StringCollection = new List<string> {"foo", "bar"},
                ComplexCollection = new List<PocoClass>
                {
                    new PocoClass
                    {
                        IntProperty = 7,
                        StringProperty = "bar"
                    },
                    new PocoClass
                    {
                        IntProperty = 9,
                        StringProperty = "pub"
                    }
                }
            };
        }

        public class PocoClass
        {
            public int IntProperty { get; set; }
            public string StringProperty { get; set; }

            public PocoClass ComplexProperty { get; set; }

            public IEnumerable<int> IntCollection { get; set; }
            public IEnumerable<string> StringCollection { get; set; }
            public IEnumerable<PocoClass> ComplexCollection { get; set; }
        }
    }
}
