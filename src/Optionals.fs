namespace FSpotify

open System.Reflection
open Microsoft.FSharp.Reflection


module Optionals =
    
    let marketBuilder (Market market) = Request.QueryParameterBuilder("market",market)

    let limitBuilder (limit: int) = Request.QueryParameterBuilder("limit",string limit)

    let offsetBuilder (offset: int) = Request.QueryParameterBuilder("offset",string offset)

    let albumTypesBuilder (albumTypes: AlbumType list) =
        Request.QueryParameterBuilder("album_type",albumTypes |> List.map AlbumType.asString |> Misc.buildCommaList)

    type HasMarket = abstract member market: (Market -> Request.QueryParameterBuilder)
    type HasLimit = abstract member limit: (int -> Request.QueryParameterBuilder)
    type HasOffset = abstract member offset: (int -> Request.QueryParameterBuilder)
    type HasAlbumTypes = abstract member albumTypes: (AlbumType list -> Request.QueryParameterBuilder)

    type MarketOption () =
        interface HasMarket with member this.market = marketBuilder

    type MarketOffsetAndLimitOption () =
        inherit MarketOption ()
        interface HasLimit with member this.limit = limitBuilder
        interface HasOffset with member this.offset = offsetBuilder

    type MarketOffsetLimitAndAlbumTypesOption () =
        inherit MarketOffsetAndLimitOption ()
        interface HasAlbumTypes with member this.albumTypes = albumTypesBuilder


    let inline withOptional<'a,'b,'c> fn (arg: 'c) (request: Request.Request<'a,'b>) = request |> Request.withOptionals (fun (inner) -> Request.build ((fn inner) arg))

    let inline withMarket market = withOptional (fun (o: 'a when 'a :> HasMarket) -> o.market) market

    let inline withLimit limit = withOptional (fun (o: 'a when 'a :> HasLimit) -> o.limit) limit

    let inline withOffset offset = withOptional (fun (o: 'a when 'a :> HasOffset) -> o.offset) offset

    let inline withAlbumTypes albumTypes = withOptional (fun (o: 'a when 'a :> HasAlbumTypes) -> o.albumTypes) albumTypes