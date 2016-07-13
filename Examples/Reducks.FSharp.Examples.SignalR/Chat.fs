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

    member this.Dispatch (action: Action<Payload>) =
        match action.``type`` with
        | "USER_TYPING" ->
            match action.payload with
            | Typing payload -> 
                store.dispatch(typingAction payload.user payload.value) |> ignore
            | _ -> ()

        | "NEW_USER" ->
            match action.payload with
            | User payload -> store.dispatch(newUserAction payload) |> ignore
            | _ -> ()

        | "POST_MESSAGE" ->
            match action.payload with
            | User payload -> store.dispatch(postMessageAction payload) |> ignore
            | _ -> ()

        | _ -> ()
