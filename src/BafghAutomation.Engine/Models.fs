namespace BafghAutomation.Engine.Models

open System.ComponentModel

// Everything string, Types are a lie lol.
// TODO: Change strings to proper types, this needs a giant refactoring.

[<CLIMutable>]
type Good =
    { Id       : int
      ItemCode : string
      Diameter : string
      Length   : string
      SignId   : string }

[<CLIMutable>]
type Pack =
    { Id : int
      ItemCode       : string
      Weight         : string
      Time           : string
      Date           : string
      PackNo         : string
      IsPrinted      : bool
      NumberOfPrints : int}

[<CLIMutable>]
type SentItem =
    { Id : int
      Second  : string
      Minute  : string
      Hour    : string
      Day     : string
      Month   : string
      Year    : string
      Content : string }
