module Tests

open System
open Xunit
open BafghAutomation.Engine

type FunFact = FactAttribute

[<FunFact>]
let ``Date from string returns correct result`` () =
    Assert.True (Utils.GetDateFromString "13980110" = struct("1398", "01", "10"))
    Assert.True (Utils.GetDateFromString "13761001" = struct("1376", "10", "01"))

[<FunFact>]
let ``Time from string returns correct result`` () =
    Assert.True (Utils.GetTimeFromString "1020" = struct("10", "20"))
    Assert.True (Utils.GetTimeFromString "2230" = struct("22", "30"))