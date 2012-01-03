namespace Rtd

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Windows.Forms
open Microsoft.Office.Interop.Excel

[<ProgId("Rtd.Time");Guid("D0936AC8-643B-4C89-977E-DC2D7FAB7E4A");
  ClassInterface(ClassInterfaceType.None);
  ComVisible(true)>]
type Time () =
    let mutable updateEvent : IRTDUpdateEvent = null
    let timer = new Timer(Interval=2000)
    let mutable subscription : IDisposable = null
    let topics = Dictionary<int, DateTime option>()
    let toString = function Some time -> Elapsed.humandate time | None -> ""
    interface IRtdServer with
        member server.ServerStart(callback:IRTDUpdateEvent) = 
            updateEvent <- callback
            subscription <- timer.Tick.Subscribe(fun _ ->
                timer.Stop()
                if callback <> null then updateEvent.UpdateNotify()
            )
            timer.Start()
            1
        member server.ServerTerminate() =
            subscription.Dispose()
            timer.Stop()
            timer.Dispose()
        member server.ConnectData(topicId:int,strings:Array byref,newValues:bool byref) =
            let values = Array.create strings.Length String.Empty
            strings.CopyTo(values, 0)           
            let parse value = 
                match DateTime.TryParse value with
                | true, time -> Some time
                | false, _ -> None
            let time =    
                match (if strings.Length >= 1 then strings.GetValue(0) else null) with
                | :? string as s -> parse s
                | _ -> None                   
            topics.[topicId] <- time            
            box (toString time)
        member server.DisconnectData(topicId:int) =
            topics.Remove(topicId) |> ignore
        member server.RefreshData(topicCount:int byref) =
            let data = Array2D.zeroCreate 2 topics.Count
            topics |> Seq.iteri (fun i pair ->
                data.[0,i] <- box pair.Key               
                data.[1,i] <- box (toString pair.Value)
            )
            topicCount <- topics.Count
            timer.Start()
            data :> System.Array
        member server.Heartbeat() = 1