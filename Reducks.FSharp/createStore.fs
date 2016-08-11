module reducks

type Action<'Payload> =
    { ``type`` : string
      payload : 'Payload }

type Store<'State, 'Payload> =
    { getState : unit -> 'State
      dispatch : 'Payload -> 'Payload
      subscribe : (unit -> unit) -> unit -> unit }

let createStore<'State, 'Payload> =
    fun (reducer, initialState : 'State) ->
        let mutable state = initialState
        let mutable subs = Seq.empty

        let dispatcher =
            fun (action : 'Payload) ->
                state <- reducer state action
                subs |> Seq.iter (fun s -> s())
                action

        let subscriber =
            fun subscriber ->
                subs <- Seq.append subs [ subscriber ]
                let index = Seq.length subs
                fun () ->
                    subs <- subs
                            |> Seq.mapi (fun i s -> i, s)
                            |> Seq.filter (fun (i, e) -> i <> index)
                            |> Seq.map (fun (i, s) -> s)

        let getState = fun () -> state
        { getState = getState
          dispatch = dispatcher
          subscribe = subscriber }
