using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using nGenie.WebAnalysis.Application.Common;
using System.Threading.Tasks;
using nGenie.WebAnalysis.Application.Models;
using System.Timers;

namespace nGenie.WebAnalysis.Application.Hubs
{
    public class UtentiConnessiHub : Hub
    {
        //Connessioni attuali
        private readonly static ConnessioneHubCollection connessioniHub =
            new ConnessioneHubCollection();

        //Informazioni sulle connessioni eliminate
        private readonly static List<ConnessioneHubDeleted> connessioniEliminate =
            new List<ConnessioneHubDeleted>();

        //Timer per l'eliminazione delle connessioni
        private static System.Timers.Timer timer = new System.Timers.Timer(1);

        public UtentiConnessiHub()
        {
            lock (timer)
            {
                if (timer.Interval == 1)
                {
                    setTimer();
                }
            }
        }

        /// <summary>
        /// Metodo chiamato quando un utente si connette
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>>
        public override System.Threading.Tasks.Task OnConnected()
        {
            try
            {
                string username = Context.User.Identity.Name;
                connessioniHub.AddIfNotExists(username, Context.ConnectionId);
                sendUtentiConnessi();
                
                return base.OnConnected();
            }
            catch(Exception ex)
            {
                LogUtility.InsertLogWithoutException(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo chiamato quando un utente si disconnette, cambia pagina oppure a causa di timeout.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            //LogUtility.InsertTraceWithoutException(String.Format("OnDisconnected: {0}", Context.ConnectionId));
            try
            { 
                lock (connessioniEliminate)
                {
                    ConnessioneHubDeleted item = new ConnessioneHubDeleted();
                    item.ConnectionId = Context.ConnectionId;
                    item.DataEliminazione = DateTime.Now;

                    //Memorizza che l'identificativo di connessione deve essere eliminato (eliminazione effettuata dal timer)
                    connessioniEliminate.Add(item);
                }

                return base.OnDisconnected(stopCalled);
            }
            catch (Exception ex)
            {
                LogUtility.InsertLogWithoutException(ex);
                return null;
            }
        }

        public override System.Threading.Tasks.Task OnReconnected()
        {
            //LogUtility.InsertTraceWithoutException(String.Format("OnReconnected: {0}", Context.ConnectionId));
            try
            {
                string username = Context.User.Identity.Name;
                connessioniHub.AddIfNotExists(username, Context.ConnectionId);

                return base.OnReconnected();
            }
            catch (Exception ex)
            {
                LogUtility.InsertLogWithoutException(ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo per eliminare le connessioni e per inviare un aggiornamento
        /// del numero di connessioni attuali.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            try
            {
                //LogUtility.InsertTraceWithoutException("Ingresso timer hub");

                bool utenteEliminato = false;
                lock (connessioniEliminate)
                {
                    foreach (ConnessioneHubDeleted item in connessioniEliminate)
                    {
                        //LogUtility.InsertTraceWithoutException(string.Format("iterazione connessioneId:{0} data eliminazione {1}", item.ConnectionId, item.DataEliminazione));

                        //Elimina le connessioni che sono state eliminate da almeno 10 secondi
                        var secondi = (DateTime.Now - item.DataEliminazione).TotalSeconds;
                        if (secondi >= 10)
                        {
                            //LogUtility.InsertTraceWithoutException("Rilevata connessione eliminata da piu' di 10 secondi");
                            if (connessioniHub.Remove(item.ConnectionId))
                            {
                                utenteEliminato = true;
                                //LogUtility.InsertTraceWithoutException("Rilevato utente eliminato");
                            }

                            //Marca l'elemento in modo da richiedere la sua eliminazione
                            item.MarcatoDaEliminare = true;
                        }
                    }

                    //Elimina gli elementi marcati da eliminare
                    connessioniEliminate.RemoveAll(i => i.MarcatoDaEliminare);
                }

                //Se sono stati eliminati utenti invia il numero di utenti connessi
                if (utenteEliminato)
                {
                    sendUtentiConnessi();
                }
            }
            catch (Exception ex)
            {
                LogUtility.InsertLogWithoutException(ex);
            }
            finally
            {
                timer.Enabled = true;
            }
        }

        /// <summary>
        /// Invia a tutti i client il numero di utenti connessi
        /// </summary>
        private void sendUtentiConnessi()
        {
            Clients.All.updateUtentiConnessi(connessioniHub.Count);
        }

        private void setTimer()
        {
            timer.Interval = 10000;
            timer.Elapsed += OnTimer;
            timer.AutoReset = false;
            timer.Enabled = true;
        }
    }
}