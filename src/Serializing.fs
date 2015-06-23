namespace FSpotify

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.FSharp
open Microsoft.FSharp.Reflection

module Serializing =

    // Convert unions of form "type Union = A | B | C" to/from json strings
    type UnionEnumConverter () =
        inherit JsonConverter ()

        override this.CanConvert(t) =
            FSharpType.IsUnion(t) &&
            not (FSharpType.GetUnionCases(t) |> Array.exists (fun case -> case.GetFields().Length > 0))

        override this.WriteJson(writer, value, serializer) =
            let name =
                if value = null then null
                else
                    match FSharpValue.GetUnionFields(value, value.GetType()) with
                    | case, _ -> case.Name  
            serializer.Serialize(writer,name)

        override this.ReadJson(reader, t, existingValue, serializer) =
            let value = serializer.Deserialize(reader,typeof<string>) :?> string

            let case = FSharpType.GetUnionCases(t) |> Array.tryPick (fun case ->
                // Note: Case insensitive match!
                if value <> null && case.Name.ToUpper() = value.ToUpper() then Some case else None
            )
            match case with
            | Some case -> FSharpValue.MakeUnion(case,[||])
            | None -> null

    // Convert single case unions like "type Email = Email of string" to/from json. 
    type SingleCaseUnionConverter () =
        inherit JsonConverter ()

        override this.CanConvert(t) =
            FSharpType.IsUnion(t) && FSharpType.GetUnionCases(t).Length = 1

        override this.WriteJson(writer, value, serializer) =
            let value = 
                if value = null then null
                else 
                    let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                    fields.[0]  
            serializer.Serialize(writer, value)

        override this.ReadJson(reader, t, existingValue, serializer) =
            // Todo: Allow more than one field.
            let value = serializer.Deserialize(reader,FSharpType.GetUnionCases(t).[0].GetFields().[0].PropertyType)
            if value <> null then FSharpValue.MakeUnion(FSharpType.GetUnionCases(t).[0],[|value|]) else null

    let settings =
        let settings = JsonSerializerSettings()
        settings.Converters.Add(UnionEnumConverter())
        settings.Converters.Add(SingleCaseUnionConverter())
        Serialisation.extend (settings) 

    let serialize obj = JsonConvert.SerializeObject(obj, settings)
       
    let deserialize<'a> json = JsonConvert.DeserializeObject<'a>(json,settings)

    let unwrap element json = JObject.Parse(json).[element].ToString()

    let hasElement element json = JObject.Parse(json).[element] <> null