using System.Collections;
using ProjectWorlds.DataStructures.Lists;
using ProjectWorlds.DataStructures.Stacks;
using ProjectWorlds.DataStructures.Trees;
using ProjectWorlds.UnitTests;

#if UNITY_STANDALONE
using UnityEngine;

public class UnitTester : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 30;

        // Create te log directory
        string logDir = "D:/ProjectWorldLogs/ProjectWorldUnitTestLogs_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "/";
        System.IO.Directory.CreateDirectory(logDir);
        Debug.Log("Log directory: " + logDir);

        ListTester<ArrayList<int>> arrayListTester = new ListTester<ArrayList<int>>("Array List Tester", logDir);
        ListTester<LinkedList<int>> linkedListTester = new ListTester<LinkedList<int>>("Linked List Tester", logDir);
        CircularListTester circularListTester = new CircularListTester("Circular List Tester", logDir);
        OrderedListTester orderedListTester = new OrderedListTester("Ordered List Tester", logDir);
        OrderedListTester_Comparable orderedListTester_Comparable = new OrderedListTester_Comparable("Ordered List Tester Comparable", logDir);
        SkipListTester skipListTester = new SkipListTester("Skip List Tester", logDir);
        StackTester<Stack<int>> stackTester = new StackTester<Stack<int>>("Stack Tester", logDir);
        TreeTester<BinaryTree<int>> binaryTreeTester = new TreeTester<BinaryTree<int>>("Binary Tree Tester", logDir);
        TreeTester<AvlTree<int>> avlTreeTester = new TreeTester<AvlTree<int>>("AVL Tree Tester", logDir);
        TreeTester<BTree<int>> bTreeTester = new TreeTester<BTree<int>>("B Tree Tester", logDir);

        arrayListTester.StartTests(8);
        linkedListTester.StartTests(8);
        circularListTester.StartTests(8);
        orderedListTester.StartTests(8);
        orderedListTester_Comparable.StartTests(8);
        skipListTester.StartTests(8);
        stackTester.StartTests(8);
        binaryTreeTester.StartTests(8);
        avlTreeTester.StartTests(8);
        bTreeTester.StartTests(8);
    }
}

#endif
