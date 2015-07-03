namespace FSpotify

open System
open Microsoft.FSharp.Reflection

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

    let unsafeCopyAndUpdate<'a,'b> fieldName (value: 'b) (record: 'a) =
        let value = box value
        let recordType = record.GetType()
        
        let values =
            FSharpType.GetRecordFields(recordType)
            |> Array.map (fun field ->
                if field.Name = fieldName then
                    box value
                else
                    field.GetValue(record)
            )
        // Unsafe!
        FSharpValue.MakeRecord(recordType,values) :?> 'a