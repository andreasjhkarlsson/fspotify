

open Newtonsoft.Json
open Newtonsoft.Json.FSharp
open Microsoft.FSharp
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Reflection
open FSpotify
open FSpotify.Optionals


let printAlbum (album: Album) =
    printfn "%s - %s (%s)" (List.head album.artists).name album.name album.release_date
    album.tracks.items |> List.iter (fun track ->
        printfn "%d. %s" track.track_number track.name
    )    
    printfn ""

let printArtist (artist: Artist) =
    printf "%s (" artist.name
    artist.genres
    |> List.map (fun (Genre genre) -> genre)
    |> String.concat ", "
    |> printfn "%s)"
    printfn ""

[<EntryPoint>]
let main argv = 


    let token =
        Authorization.ClientCredentials "<client-id>" "<client-secret>"
        |> Request.send     
    
    let hospice = SpotifyId "13sfns6Tw1Nkv7Wb3xDNvH"

    let letsgetoutofthiscountry = SpotifyId "5SIbhjUUfpLeH9yZxfxJZm"

    let okcomputer = SpotifyId "2fGCAYUMssLKiUAoNdxGLx"
    
    let bobdylan = SpotifyId "74ASZWbe4lXaubB36ztrGX"

    let velvetunderground = SpotifyId "1nJvji2KIlWSseXRSlNYsC"

    let cameraobscura = SpotifyId "5gInJ5P5gQnOKPM3SUEVFt"

    let radiohead = SpotifyId "4Z8W4fKeB5YxbusRsdQVPb"

    printfn "-- Fetch several albums --"
    Album.albums [SpotifyId "13sfns6Tw1Nkv7Wb3xDNvH"; SpotifyId "5SIbhjUUfpLeH9yZxfxJZm"]
    |> withMarket (Market "SE")
    |> Request.send
    |> List.iter printAlbum
    
    printfn "-- Album tracks (paged) --"
    Album.tracks letsgetoutofthiscountry
    |> withLimit 2
    |> withMarket (Market "SE")
    |> Paging.page
    |> Paging.asSeq
    |> Seq.take 5
    |> Seq.iter (fun track -> printfn "Track: %s" track.name)

    printfn "-- Artist albums --"
    Artist.albums cameraobscura
    |> withAlbumTypes [Appears_On; Album]
    |> Paging.page
    |> Paging.asSeq
    |> Seq.iter (fun album ->
        printfn "%s: %s" (AlbumType.asString album.album_type) album.name
    )

    printfn "-- Fetch several artists --"
    Artist.artists [bobdylan; velvetunderground]
    |> Request.send
    |> List.iter printArtist
    
    printfn "-- Top tracks --"
    Artist.topTracks bobdylan (Country "SE")
    |> Request.send
    |> List.iteri (fun index track ->
        printfn "%i. %s" (1+index) track.name
    )

    printfn "-- Related artists --"
    Artist.related radiohead
    |> Request.withAuthorization token
    |> Request.send
    |> List.iter printArtist

    
    Browse.newReleases
    |> Request.withAuthorization token
    |> Optionals.withLimit 30
    |> Paging.page
    |> Paging.asSeq
    |> Seq.iter (fun album ->
        printfn "%s" album.name
    )
    

    0 
