module actions

open reducks
open System

type Payload =
    | NewUser of string
    | Message of (string * DateTimeOffset)
    | Typing of (string * string)

let postMessageAction name =
    Message(name, DateTimeOffset.Now)

let typingAction user value =
    Typing(user, value)

let newUserAction user =
    NewUser(user)
