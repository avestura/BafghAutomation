namespace BafghAutomation.Engine
module Utils =

    let GetDateFromString (str:string) =
        struct (str.[0..3], str.[4..5], str.[6..7])

    let GetTimeFromString (str:string) =
        struct (str.[0..1], str.[2..3])