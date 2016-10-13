using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WinFormData.Tests
{
    [TestFixture]
    public class TestSymbolPlayGround
    {
        [Test]
        public void FluentMethod()
        {
            var temp = new TestSymbol("C").SayGoGoGo().WaitSeconds(5).SayNoNoNo().MustFail().SayGoGoGo().Try(ed => ed.C);
        }
    }
    
    public class Temp
    {
        public string A { get; set; }
        public string B { get; set; }
        public TestSymbol C { get; set; }
    }

    public class TestSymbol
    {
        private string Symbol { get; set; }
        private string Msg { get; set; }

        private Tstatus Status
        {
            get
            {
                if (Msg != null)
                {
                    return Tstatus.Bad;
                }
                return Tstatus.Good;
            }
        }

        public TestSymbol(string name)
        {
            Symbol = name;
        }

        public TestSymbol SayGoGoGo()
        {
            if (Status == Tstatus.Bad) return this;
            Console.WriteLine("gogogo");
            return this;
        }

        public TestSymbol SayNoNoNo()
        {
            if (Status == Tstatus.Bad) return this;
            Console.WriteLine("nonono");
            return this;
        }

        public TestSymbol WaitSeconds(int second)
        {
            if (Status == Tstatus.Bad) return this;
            Console.WriteLine("waiting for {0} seconds...", second);
            Thread.Sleep(second * 1000);
            return this;
        }

        public TestSymbol MustFail()
        {
            Msg = "Fail Did nooooo.....";
            Console.WriteLine("Fail did");
            return this;
        }

        public TestSymbol Try(Expression<Func<Temp, TestSymbol>> exp)
        {
            var temp2 = Path(exp,true);
            var temp3 = Path(exp, false);

            foreach (var item in exp.Parameters)
            {
                var s = item;
            }
            return this;
        }

        private  string Path(Expression<Func<Temp,TestSymbol>> expression, bool includeType)
        {
            var propertyExpression = expression.Body;
            MemberExpression memberExpression;
            var builder = new StringBuilder();
            do
            {
                memberExpression = propertyExpression as MemberExpression;

                if (memberExpression == null) { break; }

                if (!(memberExpression.Member is PropertyInfo || memberExpression.Member is FieldInfo))
                {
                    throw new InvalidOperationException("ExpressionHelper:Property/field access expected");
                }
                if (builder.Length > 0)
                {
                    builder.Insert(0, ".");
                }
                builder.Insert(0, memberExpression.Member.Name);
                if (includeType)
                {
                    builder.Insert(0, ".");

                    var declaringType = memberExpression.Member.DeclaringType;
                    string name="";

                    if (!(declaringType.IsInterface || declaringType.IsAbstract))
                        name = declaringType.Name;

                    builder.Insert(0, name);
                }
                propertyExpression = memberExpression.Expression;

            } while (memberExpression != null);

            return builder.ToString();
        }
    }
}
