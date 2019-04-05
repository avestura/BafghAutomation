namespace BafghAutomation.Engine.Models

open System.ComponentModel

[<CLIMutable>]
type Good =
    { Id : int
      ItemCode : string
      Diameter : string
      Length   : string
      SignId   : string }

[<CLIMutable>]
type Pack =
    { Id : int
      ItemCode : string
      Weight   : string
      Time     : string
      Date     : string
      PackNo   : string }

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
