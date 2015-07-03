
open System.IO
open FSpotify
open FSpotify.Authorization


[<EntryPoint>]
let main argv = 

    let clientId = "<id>"
    let clientSecret = "<secret>"

    let token =
        try
            let token = File.ReadAllText("token") |> Serializing.deserialize<Token>
            User.me |> Request.withAuthorization token |> Request.send |> ignore
            token
        with
        | _ ->
            let token =
                [PlaylistReadPrivate; PlaylistReadCollaborative ;PlaylistModifyPublic
                 PlaylistModifyPrivate; Streaming; UserFollowModify; UserFollowRead
                 UserLibraryRead; UserLibraryModify; UserReadPrivate; UserReadBirthday
                 UserReadEmail]
                |> UserAuthentication.authorize clientId clientSecret
            token |> Serializing.serialize |> fun str -> File.WriteAllText("token",str)
            token

    
    ArtistSummary.print (SpotifyId "2BpAc5eK7Rz5GAwSp9UYXa")

    0 
