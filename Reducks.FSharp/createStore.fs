module reducks

type MiddlewareStore<'State, 'Payload> =
    { getState : unit -> 'State
      dispatch : 'Payload -> 'Payload }

type Store<'State, 'Payload> =
    { getState : unit -> 'State
      dispatch : 'Payload -> 'Payload
      subscribe : (unit -> unit) -> unit -> unit }

let compose chain =
    if (Seq.isEmpty chain) then
        fun arg -> arg
    else
        let last = Seq.last chain
        let rest = Seq.toList(chain).[0..(Seq.length chain) - 2]

        fun arg -> List.foldBack (fun func composed -> func composed) rest (last arg)

let rec createStore<'State, 'Payload> =
    fun reducer (initialState : 'State) (middlewares : seq<MiddlewareStore<'State, 'Payload> -> ('Payload -> 'Payload) -> 'Payload -> 'Payload>) ->
        match Seq.isEmpty middlewares with
        | false ->
            let store = createStore reducer initialState Seq.empty
            let mutable dispatch = store.dispatch

            let chain =
                middlewares |> Seq.map (fun m ->
                                   m ({ getState = store.getState
                                        dispatch = fun action -> dispatch (action) }))

            dispatch <- dispatch |> compose chain

            { store with dispatch = dispatch }
        | true ->
            let mutable state = initialState
            let mutable subs = Seq.empty

            let dispatcher (action : 'Payload) =
                state <- reducer state action
                subs |> Seq.iter (fun s -> s())
                action

            let subscriber subscriber =
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
