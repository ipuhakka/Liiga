using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Liiga;

namespace LiigaTest
{
    [TestFixture]
    class MatchQuery_tests
    {
        [Test]
        public void test_addSubQuery_empty_string()
        {
            /*empty string should not add anything to list.*/
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery("");
            Assert.AreEqual(0, mq.get_sub_queries().Count);
        }

        [Test]
        public void test_addSubQuery_null_string()
        {
            /*empty string should not add anything to list.*/
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery(null);
            Assert.AreEqual(0, mq.get_sub_queries().Count);
        }

        [Test]
        public void test_addSubQuery()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery("test");
            Assert.AreEqual(1, mq.get_sub_queries().Count);
        }

        [Test]
        public void test_clearSubQueries()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery("test");
            mq.clearSubQueries();
            Assert.AreEqual(0, mq.get_sub_queries().Count);
        }

        [Test]
        public void test_getQueryString()
        {
            MatchQuery mq = new MatchQuery();
            mq.addSubQuery("test ");
            mq.addSubQuery("test 2 ");
            mq.addSubQuery("test 3 ");
            string expected = "SELECT * FROM matches WHERE test AND test 2 AND test 3 ;";
            Assert.AreEqual(expected, mq.getQueryString());
        }
    }
}
