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

let reducer = fun (state: State) (action: Payload) ->
    match action with
    | Message (user, timestamp) -> 
        let message = state.typing.[user]
        { state with 
            messages = Seq.append state.messages [{ user = user; timestamp = timestamp; message = message }]
            typing = Map.remove user state.typing }
    | Typing (user, value) -> { state with typing = state.typing.Add(user, value) }
    | NewUser user -> { state with users = Set.add user state.users }


let middleware = fun store ->
    fun next ->
        fun action ->
            match action with
            | NewUser user -> printfn "The user '%s' has joined" user
            | _ -> ()

            next(action)

let store = createStore
                reducer
                { typing = Map.empty<string, string>; messages = Seq.empty; users = Set.empty }
                [middleware]

let getStore = fun () -> store
