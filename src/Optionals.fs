namespace FSpotify

open System.Reflection
open Microsoft.FSharp.Reflection

module Optionals =
    
    type Optional<'a> = 'a -> Request.QueryParameterBuilder

    let builder name = fun data -> Request.QueryParameterBuilder(name,data)

    let marketBuilder (Market market) = builder "market" market

    let countryBuilder (Country country) = builder "country" country

    let limitBuilder (limit: int) = builder "limit" (string limit)

    let offsetBuilder (offset: int) = builder "offset" (string offset)

    let localeBuilder (Locale locale) = builder "locale" locale

    let timestampBuilder (datetime: System.DateTime) = builder "timestamp" (datetime.ToString("yyyy-MM-ddTHH:mm:ss"))

    let albumTypesBuilder (albumTypes: AlbumType list) = builder "album_type" (albumTypes |> List.map AlbumType.asString |> Misc.buildCommaList)

    type HasMarket = abstract member market: Market Optional
    type HasCountry = abstract member country: Country Optional
    type HasLimit = abstract member limit: int Optional
    type HasOffset = abstract member offset: int Optional
    type HasAlbumTypes = abstract member albumTypes: AlbumType list Optional
    type HasLocale = abstract member locale: Locale Optional
    type HasTimestamp = abstract member timestamp: System.DateTime Optional

    type MarketOption () =
        interface HasMarket with member this.market = marketBuilder

    type CountryOption () =
        interface HasCountry with member this.country = countryBuilder

    type CountryAndLocaleOption () =
        inherit CountryOption ()
        interface HasLocale with member this.locale = localeBuilder

    type LimitAndOffsetOption () =
        interface HasLimit with member this.limit = limitBuilder
        interface HasOffset with member this.offset = offsetBuilder

    type MarketOffsetAndLimitOption () =
        inherit LimitAndOffsetOption ()
        interface HasMarket with member this.market = marketBuilder

    type CountryOffsetAndLimitOption () =
        inherit MarketOption ()
        interface HasLimit with member this.limit = limitBuilder
        interface HasOffset with member this.offset = offsetBuilder
        interface HasCountry with member this.country = countryBuilder

    type LocaleCountryOffsetAndLimitOption () =
        inherit CountryOffsetAndLimitOption ()
        interface HasLocale with member this.locale = localeBuilder

    type TimestampLocaleCountryOffsetAndLimitOption () =
        inherit LocaleCountryOffsetAndLimitOption ()
        interface HasTimestamp with member this.timestamp = timestampBuilder

    type MarketOffsetLimitAndAlbumTypesOption () =
        inherit MarketOffsetAndLimitOption ()
        interface HasAlbumTypes with member this.albumTypes = albumTypesBuilder


    let inline withOptional<'a,'b,'c> fn (arg: 'c) (request: Request.Request<'a,'b>) = request |> Request.withOptionals (fun (inner) -> Request.build ((fn inner) arg))

    let withMarket market = withOptional (fun (o: #HasMarket) -> o.market) market

    let withCountry country = withOptional (fun (o: #HasCountry ) -> o.country) country

    let withLimit limit = withOptional(fun (o: #HasLimit) -> o.limit) limit

    let withOffset offset = withOptional (fun (o: #HasOffset) -> o.offset) offset

    let withAlbumTypes albumTypes = withOptional (fun (o: #HasAlbumTypes) -> o.albumTypes) albumTypes

    let withLocale locale = withOptional (fun (o: #HasLocale) -> o.locale) locale

    let withTimestamp timestamp = withOptional (fun (o: #HasTimestamp) -> o.timestamp) timestamp