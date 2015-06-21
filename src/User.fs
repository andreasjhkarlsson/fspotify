namespace FSpotify




type SubscriptionLevel = |Premium |Free |Open

type PrivateUser = {
    birthdate: string option
    country: Country option
    display_name: string
    email: string option
    followers: Followers
    href: Url
    id: SpotifyId
    images: Image list
    product: SubscriptionLevel option
    ``type``: string
    uri: Url
}

module User =
    
    let me = 
        Request.createFromEndpoint Request.Get "me"
        |> Request.parse<PrivateUser,_>

    let user (SpotifyId id) =
        Request.createFromEndpoint Request.Get "users"
        |> Request.withUrlPath id
        |> Request.parse<PublicUser,_>