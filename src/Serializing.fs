namespace FSpotify

open Newtonsoft.Json
open Newtonsoft.Json.Linq
open Newtonsoft.Json.FSharp

module Serializing =

    let converters: JsonConverter[] = [| new ListConverter();
                                          new TupleArrayConverter();
                                          new OptionConverter();
                                          new MapConverter();
                                          new SingleCaseUnionConverter();
                                          new UnionEnumConverter()|]
    
    let serialize obj = JsonConvert.SerializeObject(value = obj, converters = converters)
       
    let deserialize<'a> json = JsonConvert.DeserializeObject<'a>(value = json, converters = converters)

    let unwrap element json = JObject.Parse(json).[element].ToString()

    let hasElement element json = JObject.Parse(json).[element] <> null