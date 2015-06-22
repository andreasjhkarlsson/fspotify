
open FSpotify
open FSpotify.Authorization


[<EntryPoint>]
let main argv = 

    let clientId = "<id>"
    let clientSecret = "<secret>"

    let token =
        [PlaylistReadPrivate; PlaylistReadCollaborative ;PlaylistModifyPublic
         PlaylistModifyPrivate; Streaming; UserFollowModify; UserFollowRead
         UserLibraryRead; UserLibraryModify; UserReadPrivate; UserReadBirthday
         UserReadEmail]
        |> UserAuthentication.authorize clientId clientSecret

    
    ArtistSummary.print (SpotifyId "2BpAc5eK7Rz5GAwSp9UYXa")

    0 
