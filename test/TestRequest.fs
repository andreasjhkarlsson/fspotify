module TestRequest

open System
open FSpotify
open Xunit
open FsUnit

[<Fact>]
let ``with endpoint`` () =
    let request = Request.createFromEndpoint Request.Get "nuclear"
    request.path |> should equal "https://api.spotify.com/v1/nuclear"

[<Fact>]
let ``with url map`` () =
    let request =
        Request.create Request.Get (Uri "http://foo/")
        |> Request.mapPath (fun url -> url + "bar")
    request.path |> should equal "http://foo/bar"
