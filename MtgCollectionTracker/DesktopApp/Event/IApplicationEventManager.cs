using System;

using DesktopApp.Event.EventModels;

namespace DesktopApp.Event
{
    /// <summary>
    /// Handles events across view models.
    /// </summary>
    internal interface IApplicationEventManager
    {
        /// <summary>
        /// Publishes an event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        void Publish<T>(T message) where T : IApplicationEvent;

        /// <summary>
        /// Subscribes to an event.
        /// </summary>
        /// <typeparam name="T">The action that will trigger once an event is published.</typeparam>
        /// <param name="action"></param>
        void Subscribe<T>(Action<T> action) where T : IApplicationEvent;

        /// <summary>
        /// Unsubscribe to an event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        void Unsubscribe<T>(Action<T> action) where T : IApplicationEvent;
    }
}
