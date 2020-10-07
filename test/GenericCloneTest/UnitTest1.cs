using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using GenericClone;
using Xunit;

namespace GenericCloneTest
{
    public class UnitTest1
    {
        [Fact]
        public void ShallowClone_Should_Create_Shallow_Copy()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                ComplexProperty = new PocoClass
                {
                    IntProperty = 7,
                    StringProperty = "bar"
                }
            };

            var clone = instance.ShallowClone1();

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexProperty.Should().BeSameAs(instance.ComplexProperty);
        }         
        
        [Fact]
        public void DeepClone_With_ComplexFieldTraversing_Should_Clone_ReferenceTypes()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                ComplexProperty = new PocoClass
                {
                    IntProperty = 7,
                    StringProperty = "bar"
                }
            };

            var clone = instance.DeepClone1();

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexProperty.Should().NotBeSameAs(instance.ComplexProperty);
        }

        [Fact]
        public void DeepClone_With_FullFieldTraversing_Should_Clone_ReferenceTypes()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                ComplexProperty = new PocoClass
                {
                    IntProperty = 7,
                    StringProperty = "bar"
                }
            };

            var clone = instance.DeepClone2();

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexProperty.Should().NotBeSameAs(instance.ComplexProperty);
        }

        [Fact]
        public void DeepClone_With_ComplexFieldTraversing_Should_Clone_ValueType_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                IntCollection = new List<int>{ 1, 3, 4}
            };

            var clone = instance.DeepClone1();

            clone.Should().BeEquivalentTo(instance);
            clone.IntCollection.Should().NotBeSameAs(instance.IntCollection);
        }

        [Fact]
        public void DeepClone_With_FullFieldTraversing_Should_Clone_ValueType_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                IntCollection = new List<int> { 1, 3, 4 }
            };

            var clone = instance.DeepClone2();

            clone.Should().BeEquivalentTo(instance);
            clone.IntCollection.Should().NotBeSameAs(instance.IntCollection);
        }
        
        [Fact]
        public void DeepClone_With_ComplexFieldTraversing_Should_Clone_String_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                StringCollection = new List<string>{ "foo", "bar"}
            };

            var clone = instance.DeepClone1();

            clone.Should().BeEquivalentTo(instance);
            clone.StringCollection.Should().NotBeSameAs(instance.StringCollection);
        }

        [Fact]
        public void DeepClone_With_FullFieldTraversing_Should_Clone_String_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                StringCollection = new List<string> { "foo", "bar" }
            };

            var clone = instance.DeepClone2();

            clone.Should().BeEquivalentTo(instance);
            clone.StringCollection.Should().NotBeSameAs(instance.StringCollection);
        }
        
        [Fact]
        public void DeepClone_With_ComplexFieldTraversing_Should_Clone_Reference_Value_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
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

            var clone = instance.DeepClone1();

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexCollection.Should().NotBeSameAs(instance.ComplexCollection);
            clone.ComplexCollection.Should()
                .NotContain(x => instance.ComplexCollection.Any(n => ReferenceEquals(x, n)));
        }

        [Fact]
        public void DeepClone_With_FullFieldTraversing_Should_Clone_Reference_Value_Collections()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
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

            var clone = instance.DeepClone2();

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexCollection.Should().NotBeSameAs(instance.ComplexCollection);
            clone.ComplexCollection.Should()
                .NotContain(x => instance.ComplexCollection.Any(n => ReferenceEquals(x, n)));
        }

        [Fact]
        public void Test2()
        {
            var instance = new PocoClass
            {
                IntProperty = 13,
                StringProperty = "foo",
                ComplexProperty = new PocoClass
                {
                    IntProperty = 7,
                    StringProperty = "bar"
                }
            };

            var clone = instance.ShallowClone1();

            var fields = typeof(PocoClass).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            
            foreach (var field in fields)
            {
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                    continue;

                var value = field.GetValue(instance);
                if (value is null)
                    continue;

                field.SetValue(clone, value.ShallowClone1());
            }

            clone.Should().BeEquivalentTo(instance);
            clone.ComplexProperty.Should().NotBeSameAs(instance.ComplexProperty);
        }

        [Fact]
        public void CloneGenericList()
        {
            var list = new List<string> {"foo", "bar"};

            var clone = list.DeepClone1() as List<string>;

            clone.Should().NotBeSameAs(list);
            clone.Should().Contain(list);
        }
        
        private class PocoClass
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
