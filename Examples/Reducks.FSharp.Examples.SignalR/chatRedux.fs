module chatRedux

open reducks
open actions

type Message = {
    user: string
    timestamp: System.DateTimeOffset
    message: string
}

type State = {
    typing: Map<string, string>
    messages: seq<Message>
    users: Set<string>
}

let failer = fun () -> failwith "Type provided not vaid for action"

let reducer = fun (state: State) (action: Action<Payload>) ->
    match action.``type`` with
    | "POST_MESSAGE" ->
        match action.payload with
        | Message (user, timestamp) -> 
            let message = state.typing.[user]
            { state with 
                messages = Seq.append state.messages [{ user = user; timestamp = timestamp; message = message }]
                typing = Map.remove user state.typing }
        | _ -> state

    | "TYPING" ->
        match action.payload with
        | Typing (user, value) -> { state with typing = state.typing.Add(user, value) }
        | _ -> state

    | "NEW_USER" ->
        match action.payload with
        | NewUser user -> { state with users = Set.add user state.users }
        | _ -> state

    | _ -> state

let store = createStore (reducer, { typing = Map.empty<string, string>; messages = Seq.empty; users = Set.empty })

let getStore = fun () -> store
