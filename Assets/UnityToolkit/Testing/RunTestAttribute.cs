namespace ProjectWorlds.Testing
{
    // This attribute is used to label a function as a test and to enable/disable the test
    public class RunTestAttribute : System.Attribute
    {
        public bool RunTest { get { return runTest; } }
        public object[] TestArgs { get { return testArgs; } }
        public int TimeoutTime_ms { get { return timeoutTime_ms; } }

        // Flag that tracks if the test should be ran
        private bool runTest = false;
        // Array of arguments to pass to the test
        private object[] testArgs = null;
        // Timeout time for the test in ms
        private int timeoutTime_ms = 250;

        public RunTestAttribute(bool runTest, object[] testArgs=null, int timeoutTime_ms=250)
        {
            this.runTest = runTest;
            this.testArgs = (testArgs == null) ? new object[0] : testArgs;
            this.timeoutTime_ms = timeoutTime_ms;
        }
    }
}