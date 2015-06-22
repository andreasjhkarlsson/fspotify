
open System
open System.Diagnostics
open System.Net
open System.IO
open Newtonsoft.Json
open Newtonsoft.Json.FSharp
open Microsoft.FSharp
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Reflection
open FSpotify
open FSpotify.Optionals
open FSpotify.Search
open FSpotify.Authorization



[<EntryPoint>]
let main argv = 
    
    ArtistSummary.Example (SpotifyId "2BpAc5eK7Rz5GAwSp9UYXa")

    0 
