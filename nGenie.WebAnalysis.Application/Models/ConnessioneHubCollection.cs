using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models
{
    public class ConnessioneHubCollection
    {
        //Dizionario: username - hash connectionId
        private readonly Dictionary<string, HashSet<string>> connessioniUtente =
            new Dictionary<string, HashSet<string>>();

        //Dizionario: ConnectionId - Username
        //Introdotto perche' durante OnDisconnected sull'Hub e' disponibile solo connectionId e non la username dell'utente che si e' disconnesso
        private readonly Dictionary<string, string> connessioniMapping =
            new Dictionary<string, string>();

        public int Count
        {
            get
            {
                return connessioniUtente.Count;
            }
        }

        /// <summary>
        /// Aggiunge la connessione se non esiste.
        /// Restituisce true se e' stato inserito un nuovo utente, false altrimenti
        /// </summary>
        /// <param name="username"></param>
        /// <param name="connectionId"></param>
        /// <returns>true se e' stato inserito un nuovo utente, false altrimenti</returns>
        public bool AddIfNotExists(string username, string connectionId)
        {
            bool result = false;
            lock (connessioniUtente)
            {
                addMappingIfNotExists(connectionId, username);

                HashSet<string> hashSet;
                if (!connessioniUtente.TryGetValue(username, out hashSet))
                {
                    //Aggiunge un nuovo utente con con un hashset vuoto di identificativi di connessione
                    hashSet = new HashSet<string>();
                    connessioniUtente.Add(username, hashSet);
                    result = true;
                }
                
                //Aggiunge l'identificativo di connessione all'hashset se non esiste
                //La mutua esclusione viene garantita dalla lock sulle connessioni utente
                hashSet.Add(connectionId);

                return result;
            }
        }

        /// <summary>
        /// Elimina l'identificativo di connessione specificata.
        /// Restituisce true se e' stato eliminato anche l'utente.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public bool Remove(string connectionId)
        {
            bool result = false;
            lock (connessioniUtente)
            {
                string username;
                if (connessioniMapping.TryGetValue(connectionId, out username))
                {
                    result = Remove(username, connectionId);

                    //Rimuove la connessione dal dizionario: connessioneId - Utente
                    connessioniMapping.Remove(connectionId);
                }
            }

            return result;
        }

        /// <summary>
        /// Elimina l'identificativo di connessione corrispondente ai parametri specificati.
        /// Restituisce true se e' stato eliminato anche l'utente.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="connectionId"></param>
        public bool Remove(string username, string connectionId)
        {
            bool result = false;
            lock (connessioniUtente)
            {
                HashSet<string> hashSet;
                if (connessioniUtente.TryGetValue(username, out hashSet))
                {
                    hashSet.Remove(connectionId);

                    if (hashSet.Count == 0)
                    {
                        //Elimina l'utente
                        connessioniUtente.Remove(username);
                        result = true;
                    }
                }
            }

            return result;
        }

        private void addMappingIfNotExists(string connectionId, string username)
        {
            if (!connessioniMapping.ContainsKey(connectionId))
            {
                connessioniMapping.Add(connectionId, username);
            }
        }
    }
}

