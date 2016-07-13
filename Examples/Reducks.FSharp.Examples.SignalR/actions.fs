module actions

open reducks

let postMessage = "POST_MESSAGE"
let typing = "TYPING"
let newUser = "NEW_USER"

type Payload = 
    | NewUser of string
    | Message of (string * System.DateTimeOffset)
    | Typing of (string * string)

let postMessageAction = fun name ->
    { ``type`` = postMessage; payload = Message(name, System.DateTimeOffset.Now) }

let typingAction = fun user value ->
    { ``type`` = typing; payload = Typing(user, value) }

let newUserAction = fun user ->
    { ``type`` = newUser; payload = NewUser(user) }
