namespace FSpotify


open System.Net
open System.IO
open Serializing
open System.Web
open System

exception SpotifyError of int*string

module Request =

    type ResponseBuilder<'a> = string -> 'a

    type Request<'a,'b> = {
        path: string
        queryParameters: Map<string,string>
        responseMapper: ResponseBuilder<'a>
        optionals: 'b
    }


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

    let mapUrl fn request = {request with path = fn request.path}

    let withUrlPath path (request: Request<'a,'b>) = {request with path = sprintf "%s/%s" request.path path}

    let withQueryParameter (name,value) request = {request with queryParameters = request.queryParameters |> Map.add name value}

    let withBody body request = {request with body = body}

    let withHeader (name,value) request = {request with headers = request.headers |> Map.add name value}

    let withAuthorization {access_token = token; token_type = token_type} =
        withHeader ("Authorization",sprintf "%s %s" token_type token)

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

            headers
            |> Map.toList
            |> List.iter httpRequest.Headers.Add

            if verb <> Get then
                httpRequest.ContentType <- "application/x-www-form-urlencoded"

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

    let createFromEndpoint verb endpoint = create verb "https://api.spotify.com/v1" |> withUrlPath endpoint


