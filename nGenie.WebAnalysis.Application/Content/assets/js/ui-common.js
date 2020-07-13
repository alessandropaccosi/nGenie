var MSG_ERRORE_GENERICO = "Si è verificato un errore durante l'operazione";
var MSG_ERRORE_DETTAGLIO_NON_DISPONIBILE = "Si è verificato un errore ma non sono disponibili dettagli sul tipo di errore. Provare a ricaricare la pagina";
var CODICE_ERRORE_GENERICO = 100;

$.ajaxSetup({
    // Disabilita la cache di ogni risposta AJAX
    cache: false
});

// Mette in evidenza la voce di menu richiesta
function selezionaMenuItem(menuItemId) {
    //Toglie la selezione a tutte le voci di menu
    $('.cubo-text').removeClass('selected');

    //Seleziona la voce di menu richiesta il cubo corrente
    $('#cuboItemTitle_' + menuItemId.toString()).addClass('selected');
}

//Effettua una chiamata HTTP POST asincrona alla url specificata passandogli il parametro in formato json.
//Attende una risposta json. A completamento dell'operazione chiama la funzione functionCallback riportata nel parametro.
function ExecuteHttpPost(stringUrl, parametro, functionCallback) {
    jsonStringDatiDaInviare = stringify(parametro);

    jQuery.ajax({
        url: stringUrl,
        dataType: 'json',
        cache: false,
        contentType: "application/json",
        data: "{'_par': '" + jsonStringDatiDaInviare + "'}",
        type: 'POST',
        error: function (jqXHR, textStatus, errorThrown) {
            result = createEsitoOperazioneError(errorThrown);
            functionCallback(result, parametro);
        },
        success: function (risultato) {
            functionCallback(risultato, parametro);
        }
    });
}

function ExecuteHttpGetNoOperation(stringUrl, parametro, functionCallback) {
    jsonStringDatiDaInviare = stringify(parametro);

    jQuery.ajax({
        url: stringUrl,
        dataType: 'json',
        cache: false,
        contentType: "application/json",
        data: "{'_par': '" + jsonStringDatiDaInviare + "'}",
        type: 'GET',
        error: function (jqXHR, textStatus, errorThrown) {
            result = createEsitoOperazioneError(errorThrown);
            functionCallback(result, parametro);
        },
        success: function (risultato) {
            functionCallback(risultato, parametro);
        }
    });
}

//Effettua una chiamata get al server per risolvere un problema con Internet Explorer
function ExecuteHttpGet() {
    var callback = function esitoOperazioneCallback(risultato, parametro) {
        try {
            checkEsito(risultato);
        }
        catch (err) {
            showTooltipError(err);
        }
    }

    try {
        url = '/Home/ExecuteHttpGet';
        var parametro = {
            Id: 0
        };
        ExecuteHttpGetNoOperation(url, parametro, callback);
    }
    catch (err) {
        showTooltipError(err);
    }
}

// Per effettuare una rihiesta Ajax verso un controller MVC, la funzione JSON.stringify
// non converte correttamente le stringhe dirette al server. Ad esempio il metodo del controller 
// non viene chiamato con stringhe contenenti \ oppure '
// La seguente funzione deve essere chiamata al posto di JSON.stringify
// Ad esempio verranno effettuate le seguenti sostituzioni
//	' -> \'	
//	" -> \"
function stringify(oggetto) {
    var s = JSON.stringify(oggetto).replace(/(['"])/g, "\\$1");

    return s;
}

//Crea un oggetto EsitoOperazione con esito errore contenente il messaggio specificato
//nel parametro.
function createEsitoOperazioneError(messaggio) {
    return 
    {
        EsitoOperazione:
            {
                Esito: CODICE_ERRORE_GENERICO,
                Messaggio = messaggio
            }
    }
}

/*
 * Permette di visualizzare il messaggio specificato. Il messaggio sparirà automaticamente dopo qualche secondo.
*/
function showTooltipSuccess(messaggioDaMostrare) {
    showTooltipToastr('success', messaggioDaMostrare);
}

/*
 * Permette di visualizzare il messaggio specificato. Il messaggio sparirà automaticamente dopo qualche secondo.
*/
function showTooltipWarning(messaggioDaMostrare) {
    showTooltipToastr('warning', messaggioDaMostrare);
}

/*
 * Permette di visualizzare il messaggio specificato. Il messaggio sparirà automaticamente dopo qualche secondo.
*/
function showTooltipError(messaggioDaMostrare) {
    showTooltipToastr('error', messaggioDaMostrare);
}


/*
Permette di visualizzare il messaggio specificato utilizzando la libreria Toaser. Il messaggio sparirà automaticamente.
    tipologiaTooltip:
        'success'
        'info'
        'warning'
        'error'
Dettagli implementativi:
    Viene utilizzata la libriria toastr. Per maggiori informazioni consultare https://codeseven.github.io/toastr/demo.html
*/
function showTooltipToastr(tipologiaTooltip, messaggioDaMostrare) {
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-center",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    toastr[tipologiaTooltip](messaggioDaMostrare);
}

//Restituisce true se il valore specificato è null, stringa vuota o stringa contenente soltanto spazi
function isNullOrEmptyString(valore) {
    return (valore == null || valore == undefined || valore.trim() === '');
}

//Restituisce true se il valore specificato è null oppure undefined,
//false altrimenti.
function isNullOrUndefined(valore) {
    return (valore === null || valore === undefined);
}

//Converte la stringa specificata in int.  
//Se la stringa non possiede un valore sara' restituito 0.
function stringToIntValueOrDefault(valore) {
    var result = 0;
    if (!isNullOrUndefined(valore) && !isNullOrEmptyString(valore)) {
        result = parseInt(valore);
    }

    return result;
}


//Restituisce true in caso di esito OK, false altrimenti
function isEsitoOk(esitoOperazione) {
    return (!isNullOrUndefined(esitoOperazione)) && (esitoOperazione.Esito == 0);
}

//Controlla l'esito dell'operazione dal parametro specificato. 
//Il parametro puo' corrispondere ad un esito oppure contenere l'esito al suo interno.
//Solleva eccezione se l'esito non e' presente o non corrisponde ad OK.
function checkEsito(data) {
    var esitoOperazione = getEsito(data);

    if (!isEsitoOk(esitoOperazione)) {
        //Prova a recuperare il messaggio di errore contenuto nell'esito. Altrimenti restituisce un messaggio di errore generico.
        var errorMessage = isNullOrEmptyString(esitoOperazione.Messaggio) ? MSG_ERRORE_GENERICO : esitoOperazione.Messaggio;

        //Solleva eccezione
        throw errorMessage;
    }
}

//Restituisce l'esito dell'operazione contenuto nel parametro specificato. 
//Solleva eccezione se l'esito non e' presente
function getEsito(risultato) {
    var ERROR_MESSAGE = "Esito dell'operazione non disponibile";

    var result = null;
    if (!isNullOrUndefined(risultato)) {
        //Caso 1: l'oggetto contiene un esito
        if (!isNullOrUndefined(risultato.EsitoOperazione)) {
            result = risultato.EsitoOperazione;
        }
        else
        //Caso 2: l'oggetto corrisponde ad un esito
        if (!isNullOrUndefined(risultato.Esito)) {
            result = risultato;
        }
    }

    if (isNullOrUndefined(result))
        throw ERROR_MESSAGE;

    return result;
}

//Restituisce l'elemento jquery corrispondente all'identificatore specificato
function getById(id) {
    return $('#' + id);
}

function getValue(id) {
    return getById(id).val();
}

function getText(id) {
    return getById(id).text();
}

function getValueAsInt(id) {
    return parseInt(getValue(id));
}

function getValueAsBoolean(id) {
    return getValue(id) == "true";
}

function htmlEncode(stringValue) {
    var result = '';
    if (stringValue != null && stringValue != undefined) {
        result = stringValue
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    return result;
}

// @@Ricontrollare. Deve codificare la stringa utilizzando caratteri xml validi. Al momento uguale ad htmlEncode
function xmlEncode(stringValue) {
    var result = '';
    if (stringValue != null && stringValue != undefined) {
        result = stringValue
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    return result;
}

//Restituisce la stringa html contenente le intestazioni della  tabella drill through specificata
function getNumeroElementiArrayAssociativo(arrayAssociativo) {
    var result = 0;
    if (!isNullOrUndefined(arrayAssociativo)) {
        for (var key in arrayAssociativo) {
            result++;
        }
    }

    return result;
}

//Tipo di dato LogEntry
//Contiene informazioni per effettuare un log
function LogEntry() {
    this.Messaggio = "";
}

// Inserisce un log chiamando un metodo del controller. Non solleva eccezioni
function writeLogWithoutException(aLogEntry) {
    try
    {
        var callback = function esitoOperazioneCallback(risultato, parametro) {
        }

        var url = '/Home/InserisciLog';
        var parametro = aLogEntry;

        ExecuteHttpPost(url, parametro, callback);
    }
    catch (err) {
    }
}

function inserisciLogByString(messaggio) {
    var logEntry = new LogEntry();
    logEntry.Messaggio = messaggio;
    writeLogWithoutException(logEntry);
}

//Restituisce la stringa specificata se non nulla o indefinita. Altrimenti restituisce stringa vuota.
function toStringNotNull(value) {
    var result = "";
    if (!isNullOrUndefined(value))
        result = value;

    return result;
}

//Carica la vista parziale specificata.
//Ad esempio riportare come parametro:
//  '/Admin/UtenteDettaglio'
function caricaVistaParziale(url, callback, appendToDivId) {
    //$("#pageContent").remove();
    if (appendToDivId === undefined)
        appendToDivId = 'pageContent';
    getById(appendToDivId).load(url, callback);
}

//Controlla se effettuare un reload della pagina.
//Se e' stato premuto il pulsante back del browser ricarica la pagina dal server.
function reloadPageIfRequired() {
    var perfEntries = performance.getEntriesByType("navigation");

    if (perfEntries[0].type === "back_forward") {
        location.reload(true);
    }
}

function checkCaratteriSpeciali(stringValue) {
    if (stringContains(stringValue, "'"))
        throw "Carattere non valido: '";

    if (stringContains(stringValue, '"'))
        throw 'Carattere non valido: "';

    if (stringContains(stringValue, "/"))
        throw "Carattere non valido: /";

    if (stringContains(stringValue, "\\"))
        throw "Carattere non valido: \\";
}

//Restituisce true se la stringa contiene il carattere specificato
function stringContains(stringValue, carattere) {
    var result = false;

    if (!isNullOrEmptyString(stringValue)) {
        result = stringValue.indexOf(carattere) > -1;
    }

    return result;
}

//Restituisce true se la prima stringa e' uguale alla seconda. Il case viene ignorato
function stringEqualsWithoutCase(firstString, secondString) {
    var result = false;
    try {
        return (firstString === secondString || firstString.toLowerCase() === secondString.toLowerCase());
    }
    catch (err) {
    }

    return result;
}


