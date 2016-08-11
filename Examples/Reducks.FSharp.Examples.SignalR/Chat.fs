module Chat

open Microsoft.AspNet.SignalR
open FSharp.Interop.Dynamic
open actions
open chatRedux
open reducks

type Typing = {
    user: string
    value: string
}

type Payload =
    | User of string
    | PostMessage of string
    | Typing of Typing

type public ChatHub() =
    inherit Hub()

    let store = getStore()

    override this.OnConnected() =
        store.subscribe(fun () ->
            this.Clients.Client(this.Context.ConnectionId)?subscribe(store.getState())
            ()
        ) |> ignore
        System.Threading.Tasks.Task.Run(fun () -> ())

    member this.Dispatch (action: Payload) =
        match action with
        | Typing payload -> 
            store.dispatch(typingAction payload.user payload.value) |> ignore
        | User payload -> store.dispatch(newUserAction payload) |> ignore
        | PostMessage payload -> store.dispatch(postMessageAction payload) |> ignore
