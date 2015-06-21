namespace FSpotify

open System
open System.Text


module Authorization =    

    type Scope =
    | PlaylistReadPrivate
    | PlaylistReadCollaborative
    | PlaylistModifyPublic
    | PlaylistModifyPrivate
    | Streaming
    | UserFollowModify
    | UserFollowRead
    | UserLibraryRead
    | UserLibraryModify
    | UserReadPrivate
    | UserReadBirthday
    | UserReadEmail
    with
        static member ApiString this =
            match this with
            | PlaylistReadPrivate -> "playlist-read-private"
            | PlaylistReadCollaborative -> "playlist-read-collaborative"
            | PlaylistModifyPublic -> "playlist-modify-public"
            | PlaylistModifyPrivate -> "playlist-modify-private"
            | Streaming -> "streaming"
            | UserFollowModify -> "user-follow-modify"
            | UserFollowRead -> "user-follow-read"
            | UserLibraryRead -> "user-library-read"
            | UserLibraryModify -> "user-library-modify"
            | UserReadPrivate -> "user-read-private"
            | UserReadBirthday -> "user-read-birthdate"
            | UserReadEmail -> "user-read-email"

    let clientCredentials id secret =
        
        let encoded =
            sprintf "%s:%s" id secret
            |> Encoding.UTF8.GetBytes
            |> Convert.ToBase64String

        Request.create Request.Post "https://accounts.spotify.com/api/token"
        |> Request.withHeader ("Authorization",sprintf "Basic %s" encoded)
        |> Request.withBody "grant_type=client_credentials"
        |> Request.parse<Token,_>

    let authorizeCode id secret code (redirectUri: Uri) =
        let baseRequest = clientCredentials id secret
        baseRequest
        |> Request.withBody (sprintf "grant_type=authorization_code&code=%s&redirect_uri=%s" code redirectUri.AbsoluteUri)

    let refresh id secret (token: Token) =
        token.refresh_token |> Option.map(fun refresh_token ->
            let baseRequest = clientCredentials id secret
            baseRequest
            |> Request.withBody (sprintf "grant_type=refresh_token&code=%s" refresh_token)   
        )


    let buildAuthorizationCodeConfirmUri id (redirectUri: Uri) state scopes showDialog =
        let uri =
            sprintf "https://accounts.spotify.com/authorize/?client_id=%s&response_type=code&redirect_uri=%s"
                (Uri.EscapeDataString(id))
                (Uri.EscapeDataString(redirectUri.AbsoluteUri))
        
        let uri =
            match state with
            | Some str ->
                sprintf "%s&state=%s" uri (Uri.EscapeDataString(str))
            | None ->
                uri

        let uri =
            match scopes with
            | Some scopes ->
                scopes
                |> List.map Scope.ApiString
                |> String.concat " "
                |> Uri.EscapeDataString
                |> sprintf "%s&scope=%s" uri
            | None ->
                uri
        
        let uri =
            match showDialog with
            | Some bool ->
                sprintf "%s&show_dialog=%b" uri bool
            | None ->
                uri

        Uri(uri)

    
