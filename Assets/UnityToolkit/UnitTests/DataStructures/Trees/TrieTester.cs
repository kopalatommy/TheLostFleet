using GalacticBoundStudios.DataScribes.Managed.Trees;
using ProjectWorlds.Testing;

namespace ProjectWorlds.UnitTests
{
    public class TrieTester : TesterBase
    {
        public TrieTester(string testerName, string resultsDir=null, bool verbose=false) : base(testerName, resultsDir, verbose)
        {

        }

        public override System.Type TestType { get { return typeof(TrieTester); } }

        #region Add Tests

        [RunTest(true)]
        public bool AddTest_EmptyTree()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_NonEmptyTree()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("World");

            return trie.Count == 2;
        }

        [RunTest(true)]
        public bool AddTest_Duplicate()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("Hello");

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool AddTest_EmptyString()
        {
            Trie trie = new Trie();
            trie.Add("");

            return trie.Count == 0;
        }

        [RunTest(true)]
        public bool AddTest_NullString()
        {
            Trie trie = new Trie();
            trie.Add(null);

            return trie.Count == 0;
        }

        #endregion // Add Tests

        #region Contains Tests

        [RunTest(true)]
        public bool ContainsTest_EmptyTree()
        {
            Trie trie = new Trie();

            return !trie.Contains("Hello");
        }

        [RunTest(true)]
        public bool ContainsTest_1Item()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return trie.Contains("Hello");
        }

        [RunTest(true)]
        public bool ContainsTest_2Items()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("World");

            return trie.Contains("Hello") && trie.Contains("World");
        }

        [RunTest(true)]
        public bool ContainsTest_DoesNotContain()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return !trie.Contains("World");
        }

        [RunTest(true)]
        public bool ContainsTest_EmptyString()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return !trie.Contains("");
        }

        [RunTest(true)]
        public bool ContainsTest_NullString()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return !trie.Contains(null);
        }

        [RunTest(true)]
        public bool ContainsTest_Substring()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            return !trie.Contains("He");
        }

        #endregion // Contains Tests

        #region Remove Tests

        [RunTest(true)]
        public bool RemoveTest_EmptyTree()
        {
            Trie trie = new Trie();
            trie.Remove("Hello");

            return trie.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_1Item()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Remove("Hello");

            return trie.Count == 0;
        }

        [RunTest(true)]
        public bool RemoveTest_2Items()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("World");
            trie.Remove("Hello");

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_DoesNotContain()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Remove("World");

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_EmptyString()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Remove("");

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_NullString()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Remove(null);

            return trie.Count == 1;
        }

        [RunTest(true)]
        public bool RemoveTest_Substring()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Remove("He");

            return trie.Count == 1;
        }

        #endregion // Remove Tests

        #region Clear Tests

        [RunTest(true)]
        public bool ClearTest_EmptyTree()
        {
            Trie trie = new Trie();
            trie.Clear();

            return trie.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_1Item()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Clear();

            return trie.Count == 0;
        }

        [RunTest(true)]
        public bool ClearTest_2Items()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("World");
            trie.Clear();

            return trie.Count == 0;
        }

        #endregion // Clear Tests

        #region Enumerator Tests

        [RunTest(true)]
        public bool EnumeratorTest_EmptyTree()
        {
            Trie trie = new Trie();
            foreach (string s in trie) {
                return false;
            }

            return true;
        }

        [RunTest(true)]
        public bool EnumeratorTest_1Item()
        {
            Trie trie = new Trie();
            trie.Add("Hello");

            foreach (string s in trie) {
                return s == "Hello";
            }

            return false;
        }

        [RunTest(true)]
        public bool EnumeratorTest_2Items()
        {
            Trie trie = new Trie();
            trie.Add("Hello");
            trie.Add("World");

            int count = 0;
            foreach (string s in trie) {
                if (s == "Hello" || s == "World") {
                    count++;
                }
            }

            return count == 2;
        }

        #endregion // Enumerator Tests
    }
}