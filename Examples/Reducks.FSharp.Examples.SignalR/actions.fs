module actions

open reducks

type Payload = 
    | NewUser of string
    | Message of (string * System.DateTimeOffset)
    | Typing of (string * string)

let postMessageAction = fun name ->
    Message(name, System.DateTimeOffset.Now)

let typingAction = fun user value ->
    Typing(user, value)

let newUserAction = fun user ->
    NewUser(user)
