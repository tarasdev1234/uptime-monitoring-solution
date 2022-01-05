using System;
using System.Collections.Generic;
using System.Linq;
using Uptime.Extensions;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Helpers {
    public static class TimeLine {
        public static double GetUptime(IEnumerable<SummaryEvent> events, int days, bool downTime = false) {
            var lastNdays = DateTime.UtcNow.AddDays(-days);
            var timeline = events.TakeUntil(evt => evt.Created > lastNdays).ToList();
            
            if (timeline.Count == 0) {
                return 0;
            }
            
            var totalMins = days * 1440; // 24 * 60 = 1440
            var lastEvent = timeline.Last();
            var firstEvent = timeline.First();
            var totalDuration = (int)(firstEvent.Created.AddHours(firstEvent.Hours).AddMinutes(firstEvent.Mins) - lastEvent.Created).TotalMinutes;
            var upMins = 0;
            var downMins = 0;
            var stoppedMins = 0;
            var lastType = lastEvent.Type;

            foreach (var evt in timeline) {
                var mins = evt.Hours * 60 + evt.Mins;
                
                switch (evt.Type) {
                    case EventType.Up:
                        upMins += mins;
                        break;
                    case EventType.Down:
                        downMins += mins;
                        break;
                    case EventType.Paused:
                    case EventType.Started:
                    case EventType.Stopped:
                        stoppedMins += mins;
                        break;
                    default:
                        throw new Exception("Unknown event type");
                }
            }

            // если в выбранных евентах больше минут чем в проверяемом периоде
            // вычитаем это количество из соответсвующего типа
            if (totalDuration > totalMins) {
                var diff = totalDuration - totalMins;
                
                if (lastType == EventType.Up) {
                    upMins -= diff;
                } else if (lastType == EventType.Down) {
                    downMins -= diff;
                }
            }

            totalMins -= stoppedMins;

            var time = downTime ? downMins : upMins;

            return Math.Round(totalMins > 0 ? time * 100.0 / totalMins : 0, 1);
        }
    }
}