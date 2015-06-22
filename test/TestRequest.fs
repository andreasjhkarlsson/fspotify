module TestRequest

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
        Request.create Request.Get "http://foo/"
        |> Request.mapUrl (fun url -> url + "bar")
    request.path |> should equal "http://foo/bar"
