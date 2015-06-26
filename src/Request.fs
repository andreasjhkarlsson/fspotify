﻿namespace FSpotify


open System.Net
open System.IO
open Serializing
open System.Web
open System

exception SpotifyError of string*string

module Request =

    type HttpVerb = Get | Post | Put

    type HttpHeader =
    | ContentType
    | Custom of string

    type ResponseBuilder<'a> = string -> 'a

    type Request<'a,'b> = {
        verb: HttpVerb
        path: string
        queryParameters: Map<string,string>
        headers: Map<HttpHeader,string>
        body: string
        responseMapper: ResponseBuilder<'a>
        optionals: 'b
    }

    let withUrl (url: Uri) (request: Request<'a,'b>) =
        let path = url.GetLeftPart(UriPartial.Path)
        let queryArgs =
            if url.Query.StartsWith "?" then
                url.Query.Substring(1).Split('&')
                |> Array.map (fun str -> str.Split('='))
                |> Array.map (Array.map Uri.UnescapeDataString)
                |> Array.map (fun keyValue -> keyValue.[0],keyValue.[1])
                |> Array.toList
                |> List.foldBack ((<||) Map.add)
                <| request.queryParameters
            else
                request.queryParameters

        {request with path = path; queryParameters = queryArgs}

    let withVerb verb request ={request with verb = verb}
        
    // This is a magical little function. It applies a "builder" object to a request, transforming it.
    // Use statically resolved type parameters since the generic type(s) of the builder cannot be known.
    let inline build (builder: ^a when ^a: (member Apply: Request<'b,'c> -> Request<'b,'c>)) (request: Request<'b,'c>) =
        (^a: (member Apply: Request<'b,'c> -> Request<'b,'c>) (builder, request))


    type ResultOption<'a> = Success of 'a | Error of string*string

    let mapResponse<'a,'b,'c> (fn: 'a -> 'c) (request: Request<'a,'b>) =       
        {
            verb = request.verb
            headers = request.headers
            body = request.body
            path = request.path
            queryParameters = request.queryParameters
            responseMapper = request.responseMapper >> fn
            optionals = request.optionals
        }


    let addOptionals<'a,'b,'c> (optionals: 'c) (request: Request<'a,'b>) =
        {
               verb = request.verb
               headers = request.headers
               path = request.path
               body = request.body
               queryParameters = request.queryParameters
               responseMapper = request.responseMapper
               optionals = optionals
        }


    let withOptionals (fn: 'a -> Request<'b,'a> -> Request<'b,'a>) (request: Request<'b,'a>)=
         request |> fn request.optionals

    let unwrap<'a> = mapResponse<string,'a,string> << Serializing.unwrap

    let parse<'a,'b> = mapResponse<string,'b,'a> Serializing.deserialize<'a>

    let mapPath fn request = {request with path = fn request.path}

    let withUrlPath path (request: Request<'a,'b>) = {request with path = sprintf "%s/%s" request.path path}

    let withQueryParameter (name,value) request = {request with queryParameters = request.queryParameters |> Map.add name value}

    let withBody body request = {request with body = body}

    let withHeader (name,value) request = {request with headers = request.headers |> Map.add name value}

    let withFormBody args = 
        let args =
            args  
            |> List.map (fun (key,value) ->
                sprintf "%s=%s" (Uri.EscapeDataString(key)) (Uri.EscapeDataString(value))
            )
            |> String.concat "&"
        
        withHeader (ContentType, "application/x-www-form-urlencoded") >> withBody args

    let withJsonBody data =
        withHeader (ContentType, "application/json") >> withBody (Serializing.serialize data)

    let withAuthorization {access_token = token; token_type = token_type} =
        withHeader (Custom "Authorization",sprintf "%s %s" token_type token)

    let send ({ path = path
                queryParameters = queryParameters
                responseMapper = responseMapper
                verb = verb
                body = body
                headers = headers}) =
        
        let queryString =
            queryParameters
            |> Map.toList
            |> List.map (fun (key,value) ->
                sprintf "%s=%s" (Uri.EscapeDataString key) (Uri.EscapeDataString value)
            )
            |> String.concat "&"
            |> fun queryString -> if queryString <> "" then "?" + queryString else  "" 
        try

            let httpRequest = WebRequest.Create(path + queryString)

            httpRequest.Method <-
                match verb with
                | Get -> "GET"
                | Post -> "POST"
                | Put -> "PUT"

            headers
            |> Map.toList
            |> List.iter (fun (name,value) ->
                match name with
                | ContentType ->
                    httpRequest.ContentType <- value
                | Custom name ->
                    httpRequest.Headers.Add(name,value)
            )

            if body <> "" then
                use requestStream = httpRequest.GetRequestStream()
                use requestWriter = new StreamWriter(requestStream)
                requestWriter.Write(body)
                requestWriter.Flush()
                requestWriter.Close()

            use httpResponse = httpRequest.GetResponse()
            use stream = httpResponse.GetResponseStream()
            use streamReader = new StreamReader(stream)

            streamReader.ReadToEnd() |> responseMapper
        with
        | :? WebException as error ->
            let responseStream = error.Response.GetResponseStream()
            let errorBody = (new StreamReader(responseStream)).ReadToEnd()
            let error = (Serializing.unwrap "error" errorBody) |> deserialize<ErrorObject>
            printfn "Debug: Spotify returned an error (%s: %s)" error.status error.message
            raise <| SpotifyError(error.status,error.message)

    let trySend operation =
        try
            send operation |> Success
        with
        | :? SpotifyError as error ->
            // Todo: Somehow pattern match the tuple above?!?!
            Error (error.Data0,error.Data1)

    let asyncSend operation = async {
        return send operation
    }

    let asyncTrySend operation = async {
        return trySend operation
    }

    type QueryParameterBuilder(key,value) =
        member this.Apply<'a,'b> (request: Request<'a,'b>) =
            request |> withQueryParameter (key,value)

    let create verb url = 
        {
            verb = verb
            path = ""
            headers = Map.empty
            body = ""
            queryParameters = Map.empty
            responseMapper = string
            optionals = ()
        }
        |> withUrl url

    let createFromEndpoint verb endpoint = create verb (Uri "https://api.spotify.com/v1") |> withUrlPath endpoint


