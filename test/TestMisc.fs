module TestMisc

open Xunit
open FsUnit
open FSpotify

[<Fact>]
let ``empty comma list`` () = Misc.buildCommaList [] |> should equal ""

[<Fact>]
let ``single item comma list`` () = Misc.buildCommaList ["foo"] |> should equal "foo"

[<Fact>]
let ``multiple item comma list`` () = Misc.buildCommaList ["foo"; "bar"; "baz"] |> should equal "foo,bar,baz"

