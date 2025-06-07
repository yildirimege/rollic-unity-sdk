using System.Runtime.CompilerServices;

// This attribute tells the C# compiler that the specified assembly
// is a friend class thus the test functions can access the protected/private methods in the SDK.
[assembly: InternalsVisibleTo("Editor")]