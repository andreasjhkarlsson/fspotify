namespace FSpotify


module Follow =
    
    type FollowType = Artist | User with static member asString = function Artist -> "artist" | User -> "user"

    let request = Request.createFromEndpoint Request.Get "me/following"

    let follow ``type`` ids =
        request
        |> Request.withVerb Request.Put
        |> Request.withQueryParameter("type",FollowType.asString ``type``)
        |> Request.withQueryParameter("ids",Misc.buildIdList ids)
        |> Request.withEmptyResult

    let followArtists = follow Artist
    let followArtist id = followArtists [id]

    let followUsers = follow User
    let followUser id = followUsers [id]

    let follows ``type`` ids =
        request
        |> Request.withUrlPath "contains"
        |> Request.withQueryParameter ("type",FollowType.asString ``type``)
        |> Request.withQueryParameter ("ids",Misc.buildIdList ids)
        |> Request.parse<bool list,_>
        |> Request.mapResponse (List.zip ids)

    let followsArtists = follows Artist
    let followsArtist id = followsArtists [id] |> Request.mapResponse (List.head >> snd)

    let followsUsers = follows User
    let followsUser id = followsUsers [id] |> Request.mapResponse (List.head >> snd)

    let unfollow ``type`` ids =
        request
        |> Request.withVerb Request.Delete
        |> Request.withQueryParameter("type",FollowType.asString ``type``)
        |> Request.withQueryParameter("ids",Misc.buildIdList ids)
        |> Request.withEmptyResult

    let unfollowArtists = unfollow Artist
    let unfollowArtist id = unfollowArtists [id]

    let unfollowUsers = unfollow User
    let unfollowUser id = unfollowUsers [id]

    let followPlaylistRequest ownerId playlistId =
        Playlist.playlistRequest ownerId playlistId
        |> Request.withUrlPath "followers"

    let followPlaylist ownerId playlistId (``public``: bool) =
        followPlaylistRequest ownerId playlistId
        |> Request.withVerb Request.Put
        |> Request.withJsonField ("public",``public``)
        |> Request.withEmptyResult

    let unfollowPlaylist ownerId playlistId  =
        followPlaylistRequest ownerId playlistId
        |> Request.withVerb Request.Delete
        |> Request.withEmptyResult

    let usersFollowsPlaylist ownerId playlistId ids =
        followPlaylistRequest ownerId playlistId
        |> Request.withVerb Request.Get
        |> Request.withUrlPath "contains"
        |> Request.withQueryParameter ("ids",Misc.buildIdList ids)
        |> Request.parse<bool list,_>
        |> Request.mapResponse (List.zip ids)

    let followsPlaylist ownerId playlistId id =
        usersFollowsPlaylist ownerId playlistId [id]
        |> Request.mapResponse (List.head >> snd)


