namespace Tests_F


open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Concurrency.F.F_Library


[<TestClass>]
type FTests () =

    [<TestMethod>]
    member this.LazyFNameTest () =
        Assert.IsTrue(LazyF.Equals("Thomas, McKirbster"));
