module UserAuthentication

open System
open System.Net
open System.Diagnostics
open System.IO
open FSpotify

let authorize clientId clientSecret permissions =
    let localRedirect = Uri("http://localhost:5000/")

    // Create some unique state to correlate request with callback
    let state = Guid.NewGuid().ToString()

    let spotifyAuthUri = Authorization.buildAuthorizationCodeConfirmUri clientId localRedirect (Some state) (Some permissions) (Some true)
    
    // Create callback listener
    use listener = new HttpListener()
    listener.Prefixes.Add localRedirect.AbsoluteUri
    listener.Start()

    // Open browser
    Process.Start(spotifyAuthUri.AbsoluteUri) |> ignore

    printfn "Waiting for auth callback..."

    // Wait for callback
    let context = listener.GetContext()
    let incoming = context.Request
    let response = context.Response

    // Respond to callback
    use writer = new StreamWriter(response.OutputStream)
    writer.Write("OK")
    writer.Close();

    // User denied (or some other error).
    if incoming.QueryString.Get("error") <> null then failwith "Did not get user authorization"
    
    if state <> incoming.QueryString.Get("state") then failwith "Authorization state did not match! Possible CSRF detected!"

    // Extract auth code!
    let code = (incoming.QueryString.Get("code"))

    // Use auth code to get token
    let token =
        Authorization.authorizeCode clientId clientSecret code localRedirect
        |> Request.send  

    printfn "%s logged in!" (User.me |> Request.withAuthorization token |> Request.send).display_name

    token