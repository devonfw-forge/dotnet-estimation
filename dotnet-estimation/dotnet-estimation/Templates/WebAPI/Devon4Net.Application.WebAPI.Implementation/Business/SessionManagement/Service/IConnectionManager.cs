using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public interface IConnectionManager
    {
        /// <summary>
        /// Get the corresponding websocket to the given client id
        /// </summary>
        /// <param name="id">Client UUID</param>
        /// <returns>Websocket of the corresponding id</returns>
        WebSocket GetSocketById(string id);

        /// <summary>
        /// Returns all Websockets for the current ConnectionManager.
        /// </summary>
        /// <returns>ConcurrentDictionary of the current connections. Where the key string, is the id of the user and the Websocket value is the corresponding websocket.</returns>
        ConcurrentDictionary<string, WebSocket> GetAll();

        /// <summary>
        /// Returns all Websockets residing in Open state for the current ConnectionManager.
        /// </summary>
        /// <returns>ConcurrentDictionary of all websocket connections in open state</returns>
        ConcurrentDictionary<string, WebSocket> GetAllOpen();

        /// <summary>
        /// Returns all Websockets residing in Closed or CloseReceived state for the current ConnectionManager.
        /// </summary>
        /// <returns>ConcurrentDictionary of all websocket connections in Closed or CloseReceived state</returns>
        ConcurrentDictionary<string, WebSocket> GetAllClosed();

        /// <summary>
        /// Get a list of all current open Websocket id's.
        /// </summary>
        /// <returns>List with strings of available sockets.</returns>
        List<string> GetAvailableSocketIds();

        /// <summary>
        /// Get the corresponding client id to the given websocket
        /// </summary>
        /// <param name="socket">Websocket to search for the id</param>
        /// <returns>Client UUID of the corresponding websocket</returns>
        string GetId(WebSocket socket);

        /// <summary>
        /// Attempts to add a socket to the ConnectionManager
        /// </summary>
        /// <param name="id">Client UUID</param>
        /// <param name="socket">Websocket to add</param>
        /// <returns>True, if the Websocket was successfully saved under given id. False if the Client UUID already exists.</returns>
        bool AddSocket(string id, WebSocket socket);

        /// <summary>
        /// Update a closed socket connection for a known user.
        /// </summary>
        /// <param name="id">Client UUID</param>
        /// <param name="socket">New Websocket</param>
        /// <returns>True if the websocket for given id was updated. Otherwise False</returns>
        bool ReconnectClient(string id, WebSocket socket);

        /// <summary>
        /// Abort and dispose the websocket for the given Client UUID to clear unused resources and update the current ConnectionManager.
        /// </summary>
        /// <param name="id">Client UUID</param>
        /// <returns>True if the websocket for given id was deleted succesfully from the current ConnectionManager. Else false.</returns>
        Task<bool> RemoveSocket(string id);
    }
}