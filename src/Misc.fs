namespace FSpotify

open System

module Misc =
    let buildCommaList = String.concat ","

    let buildIdList = List.map SpotifyId.asString >> buildCommaList

    let uriEscape = System.Uri.EscapeDataString

    let rec isGenericSubclass (genericBase: Type) (someType: Type) =
        if someType.IsGenericType && someType.GetGenericTypeDefinition() = genericBase then
            true
        elif someType.BaseType <> null then
            isGenericSubclass genericBase someType.BaseType
        else
            false