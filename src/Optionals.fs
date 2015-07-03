namespace FSpotify

open System
open System.Reflection
open Microsoft.FSharp.Reflection
open Request

module Optionals =

    type OptionalStringQueryParameter(name) = inherit OptionalQueryParameter<string>(name,string)

    type OptionalIntQueryParameter(name) = inherit OptionalQueryParameter<int>(name,string)

    type MarketParameter() = inherit OptionalStringQueryParameter("market")

    type CountryParameter() = inherit OptionalStringQueryParameter("country")

    type LocaleParameter() = inherit OptionalStringQueryParameter("locale")

    type PositionParameter() = inherit OptionalIntQueryParameter("position")

    type LimitParameter() = inherit OptionalIntQueryParameter("limit")

    type OffsetParameter() = inherit OptionalIntQueryParameter("offset")

    type TimestampParameter() = inherit OptionalStringQueryParameter("timestamp")

    type AlbumTypesParameter() = inherit OptionalQueryParameter<AlbumType list>("album_type", Misc.buildCommaList << List.map AlbumType.asString)

    type MarketOption = {
        [<MarketParameter()>]
        market: Market option
    } with static member Default = {market = None}

    type CountryOption = {
        [<CountryParameter()>]
        country: Country option
    } with static member Default = {country = None}

    type CountryAndLocaleOption = {
        [<CountryParameter()>]
        country: Country option
        [<LocaleParameter()>]
        locale: Locale option
    } with static member Default = {country = None; locale = None}

    type PositionOption = {
        [<PositionParameter()>]
        position: int option
    } with static member Default = {position = None}

    type LimitAndOffsetOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
    } with static member Default = {limit = None; offset = None}

    type MarketLimitAndOffsetOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
        [<MarketParameter()>]
        market: Market option
    } with static member Default = {market = None; offset = None; limit = None}

    type CountryOffsetAndLimitOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
        [<CountryParameter()>]
        country: Country option
    } with static member Default = {limit = None; offset = None; country = None}

    type LocaleCountryOffsetAndLimitOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
        [<CountryParameter()>]
        country: Country option
        [<LocaleParameter()>]
        locale: Locale option        
    } with static member Default = {limit = None; offset = None; country = None; locale = None}

    type TimestampLocaleCountryOffsetAndLimitOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
        [<CountryParameter()>]
        country: Country option
        [<LocaleParameter()>]
        locale: Locale option     
        [<TimestampParameter()>]
        timestamp: DateTime option   
    } with static member Default = {limit = None; offset = None; country = None; locale = None; timestamp = None}

    type MarketOffsetLimitAndAlbumTypesOption = {
        [<LimitParameter()>]
        limit: int option
        [<OffsetParameter()>]
        offset: int option        
        [<MarketParameter()>]
        market: Market option
        [<AlbumTypesParameter()>]
        albumTypes: (AlbumType list) option
    } with static member Default = {limit = None; offset = None; market = None; albumTypes = None}
