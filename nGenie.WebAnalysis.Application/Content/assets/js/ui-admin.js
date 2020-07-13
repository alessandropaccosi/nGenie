var DIV_TABELLA_ZERO_CONFIGURATION_ID = 'divTableZeroConfiguration';
var TABELLA_ZERO_CONFIGURATION_ID = "tableZeroConfiguration";
var DROP_DOWN_LIST_DATABASE_ID = 'selectDatabase';

$(document).ready(function () {
    formattaTabellaZeroConfiguration();
})

function nuovoCubo() {
    var databaseId = getDatabaseIdSelected();

    if (databaseId == 0)
        showTooltipWarning('Selezionare un database');
    else {
        caricaNuovoCubo(databaseId);
    }
}

function caricaNuovoCubo(databaseId) {
    var callback = function caricaVistaParzialeCallback() {
    }

    var url = '/Admin/PartialViewNuovoCubo/' + databaseId;
    caricaVistaParziale(url, callback);
}

//Carica il dettaglio del cubo
//Se non viene specificato l'identificativo del cubo viene richiesto di creare un nuovo cubo
function caricaCuboDettaglio(cuboId) {
    var callback = function caricaVistaParzialeCallback() {
    }

    var url = '/Admin/PartialViewModificaCubo';
    var parametro = (cuboId == undefined ? '' : '/' + cuboId);

    caricaVistaParziale(url + parametro, callback);
}

//Restituisce l'identificativo del database attualmente selezionato.
//Restituisce zero se non è selezionato nessun database
function getDatabaseIdSelected() {
    return getById(DROP_DOWN_LIST_DATABASE_ID).val();
}

function caricaListaUtenti() {
    var callback = function caricaVistaParzialeCallback() {
        formattaTabellaZeroConfiguration();
    }

    caricaVistaParziale('/Admin/PartialViewGestioneUtenti', callback);
    selezionaMenuItem('menuItemUtenti');
}

//Carica la pagina per la gestione dei Server Olap.
function caricaListaServer() {
    var callback = function caricaVistaParzialeCallback() {
        formattaTabellaZeroConfiguration();
    }

    caricaVistaParziale('/Admin/PartialViewElencoServer', callback);
    selezionaMenuItem('menuItemServer');
}

//Carica la pagina per la gestione dei Database Olap.
function caricaListaDatabase() {
    var callback = function caricaVistaParzialeCallback() {
        formattaTabellaZeroConfiguration();
    }

    caricaVistaParziale('/Admin/PartialViewElencoDatabase', callback);
    selezionaMenuItem('menuItemDatabase');
}

//Carica la pagina per la gestione dei Cubi.
//Parametri:
//  databaseId      eventuale identificatore del database da selezionare
function caricaGestioneCubi(databaseId) {
    var callback = function caricaVistaParzialeCallback() {
        if (!isNullOrUndefined(databaseId)) {
            caricaCubi(databaseId);
            formattaTabellaZeroConfiguration();
        }
            
    }

    var url = '/Admin/PartialViewGestioneCubi';
    var parametro = (isNullOrUndefined(databaseId) ? '' : '/' + databaseId);

    caricaVistaParziale(url + parametro, callback);
    selezionaMenuItem('menuItemCubi');
}

function caricaCubi(databaseId) {
    var callback = function caricaVistaParzialeCallback() {
        formattaTabellaZeroConfiguration();
    }

    if (isNullOrUndefined(databaseId) || databaseId == 0) {
        caricaGestioneCubi();
    }
    else {
        var url = '/Admin/PartialViewCubiRicerca';
        var parametro = '/' + databaseId;
        caricaVistaParziale(url + parametro, callback, 'pageContent02');
    }
}

// Mostra le autorizzazioni dell'utente sui cubi appartenenti al database selezionato.
function mostraAutorizzazioniCubi(databaseId) {
    titoloParagrafo = '';

    $(".autorizzazione").hide();
    if (databaseId != 0) {
        titoloParagrafo = "Mettere una spunta sui cubi da abilitare";
        $('div[data-databaseId="' + databaseId.toString() + '"]').show();
    }

    //Mostra il titolo del paragrafo
    getById("cubiInformazioni").text(titoloParagrafo);
}

function selezionaMenuItem(menuId) {
    //Toglie la selezione a tutte le voci di menu eccetto il menu principale perché deve restare sempre selezionato
    $('.menu-item:not(#menuPrincipale)').removeClass('selected');

    //Seleziona il menu corrispondente all'identificatore specificato
    getById(menuId).addClass('selected');
}

// Richiede al server il salvataggio delle autorizzazioni sui cubi per l'utente selezionato
function salvaAutorizzazioni() {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            showTooltipSuccess('Autorizzazioni salvate correttamente');
            caricaListaUtenti();
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    if (confirm("Confermare l'operazione?")) {
        var idUtente = getIdUtenteSelected();
        var arrayCubiAutorizzati = getArrayIntCubiAutorizzati();

        url = '/Admin/SalvaAutorizzazioniUtenteCubi';
        parametro = {
            PkUtenteId: idUtente,
            arrayIntCubiAutorizzati: arrayCubiAutorizzati
        };

        ExecuteHttpPost(url, parametro, callback);
    }
}

// Restituisce l'identificativo dell'utente selezionato
function getIdUtenteSelected() {
    return getById("PkUtenteId").val();
}

//Restituisce un array contenente gli identificativi dei cubi a cui l'utente è stato autorizzato
function getArrayIntCubiAutorizzati() {
    var result = new Array();
    $("div.autorizzazione > input:checked").each(function (index) {
        result[index] = parseInt($(this).attr('id'));
    });

    return result;
}

// Permette di inserire o modificare un utente
// int utenteId
// bool abilita
function inserisciModificaUtente() {
    var callback = function esitoOperazioneCallback(result, parametro) {
        try {
            checkEsito(result);
            if (parametro.PkUtenteId == 0) {

                if (!isRuoloAmministratore(parametro.RuoloId)) {
                    //Caso inserimento normale utente
                    caricaAutorizzazioni(result.Dati.PkUtenteId);
                    showTooltipSuccess("Utente inserito correttamente. Selezionare i cubi a cui l'utente è autorizzato");
                }
                else {
                    //Caso inserimento utente amministratore
                    showTooltipSuccess("Utente inserito correttamente");
                    caricaListaUtenti(result.Dati.PkUtenteId);
                }
            } 
            else {
                showTooltipSuccess("Utente modificato correttamente");
                caricaListaUtenti(result.Dati.PkUtenteId);
            }
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    var utente = getUtente();
    try {
        checkUtente(utente);
        url = '/Admin/InserisciModificaUtente';
        parametro = utente;
        ExecuteHttpPost(url, parametro, callback);
    }
    catch (err) {
        showTooltipError(err);
    }
}

function caricaAutorizzazioni(utenteId) {
    var callback = function caricaVistaParzialeCallback() {
        //Istruzione necessaria per far apparire il controllo per la selezione del database
        $('.js-example-basic-single').select2();
    }

    var url = '/Admin/PartialViewAutorizzazioniUtenteCubi/' + utenteId.toString();
    caricaVistaParziale(url, callback);
}

function inserisciModificaServer() {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Id == 0 ? "Server inserito correttamente" : "Server aggiornato correttamente");
            showTooltipSuccess(messaggio);
            caricaListaServer();
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    var server = getServer();
    try {
        checkServer(server);
        url = '/Admin/InserisciModificaServer';
        parametro = server;
        ExecuteHttpPost(url, parametro, callback);
    }
    catch (err) {
        showTooltipError(err);
    }
}

function inserisciModificaDatabase() {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Id == 0 ? "Database inserito correttamente" : "Database aggiornato correttamente");
            showTooltipSuccess(messaggio);
            caricaListaDatabase();
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    var database = getDatabase();
    try {
        checkDatabase(database);
        url = '/Admin/InserisciModificaDatabase';
        parametro = database;
        ExecuteHttpPost(url, parametro, callback);
    }
    catch (err) {
        showTooltipError(err);
    }
}

function inserisciModificaCubo() {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Id == 0 ? "Cubo inserito correttamente" : "Cubo aggiornato correttamente");
            showTooltipSuccess(messaggio);

            var databaseId = getDatabaseId();
            caricaGestioneCubi(databaseId);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    var cubo = getCubo();
    try {
        checkCubo(cubo);
        url = '/Admin/InserisciModificaCubo';
        ExecuteHttpPost(url, cubo, callback);
    }
    catch (err) {
        showTooltipError(err);
    }
}

// Restituisce l'identificatore del database a cui il cubo appartiene.
// Utilizzabile durante l'inserimento/modifica di un cubo
function getDatabaseId() {
    return getById("DatabaseId").val();
}

//Controllo dei dati prima dell'inserimento o la modifica di un Server.
function checkServer(server) {
    checkNotEmpty(server.Nome, "Riportare il nome del server");
    checkNotEmpty(server.Url, "Riportare la url per il recupero di dati OLAP");
}

//Controllo dei dati prima dell'inserimento o la modifica di un Database.
function checkDatabase(database) {
    checkNotEmpty(database.Nome, "Riportare il nome del database");
    if (getServerOlapIdSelectedAsInt() == -1) {
        throw "Selezionare il server del database";
    }
}

//Restituisce l'identificativo del server olap selezionato durante la modifica o l'inserimento di un database
function getServerOlapIdSelectedAsInt() {
    return getValueAsInt('ServerOlapId');
}

//Controllo dei dati prima dell'inserimento o la modifica di un Cubo.
function checkCubo(cubo) {
    checkNotEmpty(cubo.Nome, "Riportare il nome del cubo");
    checkNotEmpty(cubo.NomeFriendly, "Riportare il nome da visualizzare");
}

//Controllo dei dati prima dell'inserimento o la modifica di un Utente.
function checkUtente(utente) {
    checkNotEmpty(utente.Username, "Riportare la username");
}

//Controlla se la stringa specificata non è vuota (stringa null o vuota)
//sollevando un'eccezione.
//Parametri:
//  stringValue     stringa da controllare
//  errorMessage    messaggio di errore da riportare nell'eccezione
function checkNotEmpty(stringValue, errorMessage) {
    if (isNullOrEmptyString(stringValue)) {
        throw errorMessage;
    }
}

// Permette di abilitare o disabilitare il server specificato
// Server abilitato -> verrà disabilitato
// Server disabilitato -> verrà abilitato
// Parametri:
//      int ServerId
function abilitaDisabilitaServer(serverId) {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Attivo ? "Server abilitato correttamente" : "Server disabilitato correttamente");
            showTooltipSuccess(messaggio);
            setAbilitaDisabilita(serverId);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    if (confirm("Confermare l'operazione?")) {
        var server = getServerSelected(serverId);
        var abilita = !(server.abilitato);

        url = '/Admin/AbilitaDisabilitaServer';
        parametro = {
            Id: serverId,
            Attivo: abilita
        };

        ExecuteHttpPost(url, parametro, callback);
    }
}

// Permette di abilitare o disabilitare il database specificato
// Database abilitato -> verrà disabilitato
// Database disabilitato -> verrà abilitato
// Parametri:
//      int databaseId
function abilitaDisabilitaDatabase(databaseId) {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Attivo ? "Database abilitato correttamente" : "Database disabilitato correttamente");
            showTooltipSuccess(messaggio);
            setAbilitaDisabilita(databaseId);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    if (confirm("Confermare l'operazione?")) {
        var database = getDatabaseSelected(databaseId);
        var abilita = !(database.abilitato);

        url = '/Admin/AbilitaDisabilitaDatabase';
        parametro = {
            Id: databaseId,
            Attivo: abilita
        };

        ExecuteHttpPost(url, parametro, callback);
    }
}

// Permette di abilitare o disabilitare l'utente specificato. 
// Utente abilitato -> verrà disabilitato
// Utente disabilitato -> verrà abilitato
// int utenteId
// bool abilita
function abilitaDisabilitaCubo(cuboId) {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Attivo ? "Cubo abilitato correttamente" : "Cubo disabilitato correttamente");
            showTooltipSuccess(messaggio);
            setAbilitaDisabilita(cuboId);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    if (confirm("Confermare l'operazione?")) {
        var cubo = getCuboSelected(cuboId);
        var abilita = !(cubo.abilitato);

        url = '/Admin/AbilitaDisabilitaCubo';
        parametro = {
            Id: cuboId,
            Attivo: abilita
        };

        ExecuteHttpPost(url, parametro, callback);
    }
}

// Permette di abilitare o disabilitare l'utente specificato. 
// Utente abilitato -> verrà disabilitato
// Utente disabilitato -> verrà abilitato
// int utenteId
// bool abilita
function abilitaDisabilitaUtente(utenteId) {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
            var messaggio = (parametro.Abilitato ? "Utente abilitato correttamente" : "Utente disabilitato correttamente");
            showTooltipSuccess(messaggio);
            setAbilitaDisabilita(utenteId);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    if (confirm("Confermare l'operazione?")) {
        var utente = getUtenteSelected(utenteId);
        var abilita = !(utente.abilitato);

        url = '/Admin/AbilitaDisabilitaUtente';
        parametro = {
            PkUtenteId: utenteId,
            Abilitato: abilita
        };

        ExecuteHttpPost(url, parametro, callback);
    }

    //$("#confermaTitolo").html("Conferma");
    //$("#confermaRiepilogo").html('Riepilogo della finestra');
    //$('#modalConfirm').modal('show');
}

function getUtente() {
    var result = new Object();
    result.PkUtenteId = getValueAsInt('PkUtenteId');
    result.Username = getValue('Username');
    result.Abilitato = getValueAsBoolean('Abilitato');
    result.RuoloId = getValueAsInt('Ruolo');

    return result;
}

function getServer() {
    var result = new Object();
    result.Id = getValueAsInt('ServerId');
    result.Nome = getValue('NomeServer');
    result.Url = getValue('ServerUrl');
    result.Attivo = getValueAsBoolean('Attivo');

    return result;
}

function getDatabase() {
    var result = new Object();
    result.Id = getValueAsInt('DatabaseId');
    result.Nome = getValue('NomeDatabase');
    result.ServerOlapId = getServerOlapIdSelectedAsInt();
    result.Attivo = getValueAsBoolean('Attivo');

    return result;
}

function getCubo() {
    var result = new Object();
    result.Id = getValueAsInt('CuboId');
    result.DatabaseId = getValueAsInt('DatabaseId');
    result.Nome = getValue('NomeCubo');
    result.NomeFriendly = getValue('NomeCuboFriendly');
    result.Attivo = getValueAsBoolean('Attivo');

    return result;
}

//Imposta l'etichetta Abilita/Disabilita invertendo il valore.
function setAbilitaDisabilita(rowId) {
    var row = getRowSelected(rowId);

    //Inverte l'abilitazione del database
    if (row.abilitato) {
        //Occorre disabilitare
        row.attr('data-abilitato', false);
        row.text('Abilita');
        row.removeClass('abilitato');
        row.addClass('disabilitato');
    }
    else {
        //Occorre abilitare
        row.attr('data-abilitato', true);
        row.text('Disabilita');
        row.removeClass('disabilitato');
        row.addClass('abilitato');
    }
}

function getRowSelected(rowId) {
    var result = getById(rowId);
    if (result.attr('data-abilitato') == "true") {
        result.abilitato = true;
    }
    else {
        result.abilitato = false;
    }

    return result;
}

function getServerSelected(serverId) {
    var result = getById(serverId);
    if (result.attr('data-abilitato') == "true") {
        result.abilitato = true;
    }
    else {
        result.abilitato = false;
    }

    return result;
}

function getDatabaseSelected(databaseId) {
    var result = getById(databaseId);
    if (result.attr('data-abilitato') == "true") {
        result.abilitato = true;
    }
    else {
        result.abilitato = false;
    }

    return result;
}

function getCuboSelected(cuboId) {
    var result = getById(cuboId);
    if (result.attr('data-abilitato') == "true") {
        result.abilitato = true;
    }
    else {
        result.abilitato = false;
    }

    return result;
}

function getUtenteSelected(utenteId) {
    var result = getById(utenteId);
    if (result.attr('data-abilitato') == "true") {
        result.abilitato = true;
    }
    else {
        result.abilitato = false;
    }

    return result;
}

//Funzione da chiamare per mostrare una specifica tabella drill through
function formattaTabellaZeroConfiguration() {
    getById(DIV_TABELLA_ZERO_CONFIGURATION_ID).append(getById(DIV_TABELLA_ZERO_CONFIGURATION_ID));

    //Formatta la tabella utilizzando un plugin.
    getById(TABELLA_ZERO_CONFIGURATION_ID).DataTable({
        //Le seguenti istruzioni implementano lo scroll dellta tabella e sono state commentate.
        //Lo scroll funziona male.
        //"scrollY": true,
        //"scrollX": true,
        //dom: 'Bfrtip',
        //buttons: [
        //    {
        //        extend: 'copy',
        //        text: 'Copia'
        //    },
        //    {
        //        extend: 'csv',
        //        text: 'Salva CSV'
        //    },
        //    {
        //        extend: 'excel',
        //        text: 'Salva Excel'
        //    },
        //    {
        //        extend: 'pdf',
        //        text: 'Salva PDF'
        //    }
        //],
        "language": {
            "lengthMenu": "Mostra _MENU_ record per pagina"
            , "zeroRecords": "Nessun record trovato"
            , "info": "Pagina _PAGE_ di _PAGES_"
            , "infoEmpty": "Nessun record disponibile"
            , "infoFiltered": "(filtrati da _MAX_ record totali)"
            , "search": 'Ricerca:'
            , "paginate": {
                "first": "Inizio",
                "last": "Ultimo",
                "next": "Successivo",
                "previous": "Precedente"
            },
        }
    });

    //Istruzioni aggiuntive per aggiustare la formattazione della tabella
    aggiustaTabellaDrillThrough();
}

function aggiustaTabellaDrillThrough() {
    //Inserisce un div in modo da racchiudere il tag <table> contenente la tabella drill through.
    //L'istruzione serve per implementare lo scroll della tabella.
    getById(TABELLA_ZERO_CONFIGURATION_ID).wrapAll('<div style="overflow: auto;" />');
}

//Restituisce true se il ruolo specificato corrisponde a quello di Amministratore.
//Non e' stato messo in ui-common.js per motivi di protezione.
function isRuoloAmministratore(ruoloId) {
    return ruoloId == 1;
}


