namespace FSpotify

open System
open System.Text

module Authorization =    

    let ClientCredentials id secret =
        
        let encoded =
            sprintf "%s:%s" id secret
            |> Encoding.UTF8.GetBytes
            |> Convert.ToBase64String

        Request.create Request.Post "https://accounts.spotify.com/api/token"
        |> Request.withHeader ("Authorization",sprintf "Basic %s" encoded)
        |> Request.withBody "grant_type=client_credentials"
        |> Request.parse<Token,_>
