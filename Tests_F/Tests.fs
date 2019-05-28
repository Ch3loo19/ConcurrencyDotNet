namespace Tests_F

open System
open Microsoft.VisualStudio.TestTools.UnitTesting


[<TestClass>]
type FTests () =

    [<TestMethod>]
    member this.TestMethodPassing () =
        Assert.IsTrue(true);
