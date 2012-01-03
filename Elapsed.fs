﻿module Elapsed

open System

let describeTimeElapsed span =
    match span with
    | (min, _, _, _, _) when min = 0.0 -> "just now"
    | (min, _, _, _, _) when min < 2.0 -> "a minute ago"
    | (min, _, _, _, _) when min < 60.0 -> sprintf "%g minutes ago" min
    | (_, hour, _, _, _) when hour = 1.0 -> "an hour ago"
    | (_, hour, _, _, _) when hour < 24.0 -> sprintf "%g hours ago" hour
    | (_, _, day, _, _) when day = 1.0 -> "yesterday"
    | (_, _, day, _, _) when day < 7.0 -> sprintf "%g days ago" (Math.Round(day))
    | (_, _, day, _, _) when Math.Round(float(day)/7.0) = 1.0 -> "1 week ago"
    | (_, _, day, _, _) when day < 31.0 -> sprintf "%g weeks ago" (Math.Round(day/7.0)) 
    | (_, _, _, months, _) when months = 1.0 -> "a month ago"
    | (_, _, _, months, _) when months < 12.0 -> sprintf "%g months ago" months
    | (_, _, _, _, years) when years = 1.0 -> "a year ago"
    | (_, _, _, _, years) -> sprintf "%g years ago" years

let humandate (dt:DateTime) =
    let span = DateTime.Now.Subtract(dt)
    describeTimeElapsed 
        (Math.Round(span.TotalMinutes), 
            Math.Round(span.TotalHours), 
            Math.Round(span.TotalDays), 
            Math.Round(span.TotalMinutes/43829.0639), 
            Math.Round(span.TotalMinutes/525948.766))