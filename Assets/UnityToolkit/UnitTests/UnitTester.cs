using System.Collections;
using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Stacks;
using GalacticBoundStudios.DataScribes.Managed.Trees;
using ProjectWorlds.DataStructures.Unsafe.Lists;
using ProjectWorlds.PermutationNoise;
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

        ListTester<UnsafeArrayList<int>> unsafeArrayListTester = new ListTester<UnsafeArrayList<int>>("Unsafe Array List Tester", logDir);
        ListTester<ArrayList<int>> arrayListTester = new ListTester<ArrayList<int>>("Array List Tester", logDir);
        ListTester<LinkedList<int>> linkedListTester = new ListTester<LinkedList<int>>("Linked List Tester", logDir);
        CircularListTester circularListTester = new CircularListTester("Circular List Tester", logDir);
        UnsafeCircularListTester unsafeCircularListTester = new UnsafeCircularListTester("Unsafe Circular List Tester", logDir);
        OrderedListTester orderedListTester = new OrderedListTester("Ordered List Tester", logDir);
        OrderedListTester_Comparable orderedListTester_Comparable = new OrderedListTester_Comparable("Ordered List Tester Comparable", logDir);
        SkipListTester skipListTester = new SkipListTester("Skip List Tester", logDir);
        StackTester<Stack<int>> stackTester = new StackTester<Stack<int>>("Stack Tester", logDir);
        TreeTester<BinaryTree<int>> binaryTreeTester = new TreeTester<BinaryTree<int>>("Binary Tree Tester", logDir);
        TreeTester<AvlTree<int>> avlTreeTester = new TreeTester<AvlTree<int>>("AVL Tree Tester", logDir);
        TreeTester<BTree<int>> bTreeTester = new TreeTester<BTree<int>>("B Tree Tester", logDir);
        TreeTester<BPlusTree<int>> bPlusTreeTester = new TreeTester<BPlusTree<int>>("B+ Tree Tester", logDir);
        TreeTester<RedBlackTree<int>> redBlackTreeTester = new TreeTester<RedBlackTree<int>>("Red Black Tree Tester", logDir);
        TrieTester trieTester = new TrieTester("Trie Tester", logDir);
        KDTreeTester kdTreeTester = new KDTreeTester("KD Tree Tester", logDir);
        OctreeTester octreeTester = new OctreeTester("Octree Tester", logDir);
        MinHeapTester minHeapTester = new MinHeapTester("Min Heap Tester", logDir);
        MaxHeapTester maxHeapTester = new MaxHeapTester("Max Heap Tester", logDir);
        PermutationNoiseTester<LinearCongruentGenerator> linearCongruentTester = new PermutationNoiseTester<LinearCongruentGenerator>("Permutation Noise Tester", logDir);
        PermutationNoiseTester<MersenneTwister> mersenneTwisterTester = new PermutationNoiseTester<MersenneTwister>("Permutation Noise Tester", logDir);
        KDTree_Array_Tester kdTree_Array_Tester = new KDTree_Array_Tester("KD Tree Array Tester", logDir);

        // unsafeArrayListTester.StartTests(8);
        // unsafeCircularListTester.StartTests(8);
        // arrayListTester.StartTests(8);
        // linkedListTester.StartTests(8);
        // circularListTester.StartTests(8);
        // orderedListTester.StartTests(8);
        // orderedListTester_Comparable.StartTests(8);
        // skipListTester.StartTests(8);
        // stackTester.StartTests(8);
        // binaryTreeTester.StartTests(8);
        // avlTreeTester.StartTests(8);
        // bTreeTester.StartTests(8);
        // bPlusTreeTester.StartTests(8);
        // redBlackTreeTester.StartTests(8);
        // trieTester.StartTests(8);
        //kdTreeTester.StartTests(8);
        // octreeTester.StartTests(8);
        // minHeapTester.StartTests(8);
        // maxHeapTester.StartTests(8);
        // linearCongruentTester.StartTests(8);
        // mersenneTwisterTester.StartTests(8);
        //kdTree_Array_Tester.StartTests(8);

        kdTreeTester.StartTests(8);
    }
}

#endif
