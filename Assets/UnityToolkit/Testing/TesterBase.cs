using GalacticBoundStudios.DataScribes.Managed.Lists;
using GalacticBoundStudios.DataScribes.Managed.Queues;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ProjectWorlds.Testing
{
    public abstract class TesterBase
    {
        enum TestResults
        {
            Pending,
            SUCCESS,
            FAILURE,
            CRASH,
            TIMEOUT
        }

        System.Collections.Generic.Dictionary<UnitTest, TestResults> resultsMap = new System.Collections.Generic.Dictionary<UnitTest, TestResults>();
        System.Collections.Generic.Dictionary<UnitTest, long> timeMap = new System.Collections.Generic.Dictionary<UnitTest, long>();

        // Track the method and arguments for each test
        public struct UnitTest
        {
            public MethodInfo testMethod;
            public object[] testArgs;
            public int timeoutTime_ms;

            public UnitTest(MethodInfo testMethod, object[] testArgs, int timeoutTime_ms = 250)
            {
                this.testMethod = testMethod;
                this.testArgs = testArgs;
                this.timeoutTime_ms = timeoutTime_ms;
            }
        }

        public string TesterName { get { return testerName; } }
        public string LogFilePath { get { return logFilePath; } }
        public string ResultsFilePath { get { return resultsFilePath; } }

        // Name of the tester
        protected string testerName = "NotNamed";
        // Path to the file that will contain the results of the test
        protected string logFilePath = string.Empty;
        // Path to the file that will contain the results of the test
        protected string resultsFilePath = string.Empty;
        // If verbose, log all information, else only log errors
        bool verbose = false;

        // Queue of tests to run, todo make queue data structure
        GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest>? testQueue = null;

        // Collection of all active test threads
        ArrayList<Thread>? testThreads = null;

        int numTests = 0;

        public TesterBase(string testerName, string? resultsDir=null, bool verbose = false)
        {
            this.testerName = testerName;
            this.verbose = verbose;

            // Format the test name for the log file path
            // Convert to TitleCase
            this.testerName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(testerName);
            // Remove white space
            this.testerName = this.testerName.Replace(" ", "");

            // Make sure the log directory exists
            if (!Directory.Exists("TestLogs"))
            {
                Directory.CreateDirectory("TestLogs");
            }

            // Create log file path string
            logFilePath = "TestLogs/" + this.testerName + "Log.txt";
            
            if (resultsDir != null) {
                resultsFilePath = resultsDir + "/" + this.testerName + "Results.txt";
            } else {
                resultsFilePath = "TestResults/" + this.testerName + "Results.txt";
            }

            Console.WriteLine("Log file path: " + logFilePath);

            // Create the log file
            using (StreamWriter writer = new StreamWriter(logFilePath))
            {
                writer.WriteLine("Starting Tests at " + DateTime.Now.ToString());
            }
        }

        public abstract Type TestType { get; }

        // Handle collecting and starting all tests
        public void StartTests(int numThreads, int timeoutTime_ms=250)
        {
            if (verbose) {
                Log("Starting tester: " + testerName);
            }
    
            // Create the test queue
            testQueue = new GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest>();
            GatherTests(testQueue);

            StartWorkerThreads(numThreads);

            // Init the results file

            // If the results file already exists, delete it
            if (File.Exists(resultsFilePath))
            {
                File.Delete(resultsFilePath);
            }
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                writer.WriteLine("Starting Tests at " + DateTime.Now.ToString());
            
                // Finalize the test results
                int numSuccesses = 0;
                int numFailures = 0;
                int numException = 0;
                int numTimeout = 0;
                int pending = 0;

                lock (resultsMap)
                {
                    foreach (UnitTest test in resultsMap.Keys)
                    {
                        switch (resultsMap[test])
                        {
                            case TestResults.SUCCESS:
                                writer.WriteLine("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " passed. Time: " + timeMap[test] + "ms");
                                numSuccesses++;
                                break;
                            case TestResults.FAILURE:
                                writer.WriteLine("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " failed. Time: " + timeMap[test] + "ms");
                                numFailures++;
                                break;
                            case TestResults.CRASH:
                                writer.WriteLine("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " crashed");
                                numException++;
                                break;
                            case TestResults.Pending:
                                writer.WriteLine("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " is pending");
                                Log("Failed to run test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs);
                                pending++;
                                break;
                            case TestResults.TIMEOUT:
                                writer.WriteLine("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " timed out");
                                Log("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " timed out");
                                numTimeout++;
                                break;
                        }
                    }
                }

                writer.WriteLine("Finished tests at " + DateTime.Now.ToString() + " with " + numSuccesses + " / " + numTests + " successes, " + numFailures + " / " + numTests + " failures, " + numTimeout + " / " + numTests + " timeouts, and " + numException + " / " + numTests + " exceptions");
                Log(testerName + " Finished tests at " + DateTime.Now.ToString() + " with " + numSuccesses + " / " + numTests + " successes, " + numFailures + " / " + numTests + " failures, " + numTimeout + " / " + numTests + " timeouts, and " + numException + " / " + numTests + " exceptions");
            }
        }

        public GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest> GetTests()
        {
            GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest> queue = new GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest>();
            GatherTests(queue);
            return queue;
        }

        protected void GatherTests(GalacticBoundStudios.DataScribes.Managed.Queues.Queue<UnitTest> testQueue)
        {
            // Get the type of the class that is inheriting from this class
            Type testType = TestType;

            // Search through all functions defined in this class and all child classes looking
            // for any that have the RunTestAttribute
            foreach (MethodInfo method in testType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)) {
                // Check the attributes associated with the Method (function)
                foreach (Attribute attribute in method.GetCustomAttributes()) {
                    if (attribute == null) {
                        continue;
                    }
                    if (attribute is RunTestAttribute) {
                        if (verbose) {
                            Log("Queuing test: " + method.Name);
                        }

                        // If the method is virtual and has an override, skip it
                        /*if (method.IsVirtual && method.GetBaseDefinition().DeclaringType != method.DeclaringType)
                        {
                            Log("Skipping: " + method.Name + " as it is an override");
                            // continue;
                        }*/

                        // Convert the attribute to a RunTestAttribute
                        RunTestAttribute? runTestAttribute = attribute as RunTestAttribute;

                        // If the conversion failed, skip the test
                        if (runTestAttribute == null) {
                            Log("Failed to convert attribute to RunTestAttribute");
                            continue;
                        }
                        else if (!runTestAttribute.RunTest) {
                            Log("Skipping test: " + method.Name);
                            continue;
                        }

                        // Create and add the test to the queue
                        testQueue.Add(new UnitTest(method, runTestAttribute.TestArgs, runTestAttribute.TimeoutTime_ms));
                        // Add an entry to the results map
                        resultsMap.Add(testQueue.Last(), TestResults.Pending);
                        timeMap.Add(testQueue.Last(), -1);
                    }
                }
            }

            numTests = testQueue.Count;
        }

        protected void StartWorkerThreads(int numThreads)
        {
            testThreads = new ArrayList<Thread>(numThreads);

            // Create each of the worker threads
            for (int i = 0; i < numThreads; i++) {
                // Start the worker thread with the RunTests function
                Thread workerThread = new Thread(RunTests);
                workerThread.Start();

                testThreads.Add(workerThread);
            }

            // Wait for all threads to finish
            foreach (Thread thread in testThreads) {
                thread.Join();
            }

            // Clear the test threads
            testThreads.Clear();
        }

        protected void RunTests()
        {
            while (testQueue != null) {
                UnitTest test;

                lock (testQueue) {
                    // If the queue is empty, break out of the loop
                    if (testQueue.Count == 0) {
                        break;
                    }

                    test = testQueue.Dequeue();
                }

                // Create tread to run the test
                Thread testThread = new Thread(() => RunTest(test));
                testThread.Start();

                // Wait for the test to finish or timeout
                Stopwatch stopwatch = Stopwatch.StartNew();
                if (!testThread.Join(test.timeoutTime_ms)) {
                    stopwatch.Stop();
                    testThread.Interrupt();
                    Log("Test: " + testerName + "::" + test.testMethod.Name + " with args: " + test.testArgs + " timed out. Elapsed time: " + stopwatch.ElapsedMilliseconds);
                    resultsMap[test] = TestResults.TIMEOUT;
                    // Failed to exit in time, mark as timeout
                    timeMap[test] = -1;
                }
            }
        }

        protected void RunTest(UnitTest unitTest)
        {
            try
            {
                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                object? result = unitTest.testMethod.Invoke(this, unitTest.testArgs);
                watch.Stop();

                if (result != null && ((bool)result) == true)
                {
                    if (verbose) {
                        Log("Test: " + testerName + "::" + unitTest.testMethod.Name + " passed. Time: " + watch.ElapsedMilliseconds + "ms");
                    }
                    lock (resultsMap) {
                        resultsMap[unitTest] = TestResults.SUCCESS;
                        timeMap[unitTest] = watch.ElapsedMilliseconds;
                    }
                }
                else
                {
                    Log("Test: " + testerName + "::" + unitTest.testMethod.Name + " failed. Time: " + (watch.ElapsedMilliseconds) + "ms");
                    lock (resultsMap) {
                        resultsMap[unitTest] = TestResults.FAILURE;
                        timeMap[unitTest] = watch.ElapsedMilliseconds;
                    }
                }
            }
            catch (Exception e)
            {
                Log("Error running test: " + testerName + "::" + unitTest.testMethod.Name + " with args: " + e);
                Log("Error: " + e.Message);
                Log("Stack Trace: " + e.StackTrace);
                lock (resultsMap) {
                    resultsMap[unitTest] = TestResults.CRASH;
                    timeMap[unitTest] = -1;
                }
            }
        }

        protected void Log(string message)
        {
#if UNITY_STANDALONE
            UnityEngine.Debug.Log(message);
#else
            Console.WriteLine(message);
#endif

            lock (logFilePath)
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}