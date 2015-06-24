module ArtistSummary

open FSpotify

let print artistId =

    let artist = Artist.artist artistId |> Request.send

    printfn "=== %s ===" artist.name
    printfn "Genre: %s" (artist.genres |> List.map (fun (Genre genre) -> genre) |> String.concat ", ") 
    printfn "Followers: %i" artist.followers.total

    let albums =
        Artist.albums artistId
        |> Optionals.withAlbumTypes [AlbumType.Album]
        |> Paging.page
        |> Paging.asSeq
        |> Seq.map (Album.ofSimple >> Request.send)

    let latestRelease = albums |> Seq.maxBy (fun album -> album.release_date)

    printfn "Latest release: %s (%i)" latestRelease.name (ImpreciseDate.Year latestRelease.release_date)

    let related =
        Artist.related artistId
        |> Request.send
        |> List.mapi (fun i e -> (new System.Random(i)).Next(),e)
        |> List.sortBy fst
        |> List.map snd
        |> Seq.ofList
        |> Seq.truncate 5

    printfn "Related artists: %s" (related |> Seq.map (fun artist -> artist.name) |> String.concat ", ")

    ()



    

   