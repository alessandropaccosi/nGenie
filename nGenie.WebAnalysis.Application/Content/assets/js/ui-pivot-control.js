
var collapsed = {
    columns: [],
    rows: []
};
var pivotgrid;

var switchery;
var dS;

$.ajaxSetup({
    // Disabilita la cache di ogni risposta AJAX
    cache: false
});


var ERR_SERVER_O_TIMEOUT = "Timeout o server non raggiungibile. Provare ad effettuare una nuova richiesta o riconnettersi al sito";

//Tempo massimo in millisecondi da attendere prima di sollevare un timeout in una richiesta http 
//var TIMEOUT_HTTP_REQUEST = 10000;

//Tempo massimo in millisecondi da attendere prima di sollevare un timeout in una interrogazione drill through 
var DRILL_THROUGH_TIMEOUT = 30000;

// Identificatore del div contenente la tabella drill through attualmente visualizzata
var DRILL_THROUGH_TABLE_ID = 'drillThroughTable';

//Identificativo del tag da utilizzare per aggiungere i controlli necessari per la visualizzazione della tabella drill through
var DRILL_THROUGH_DIV_PRINCIPALE_ID = 'drillThroughDivPrincipale';

//Costante elemento del treeview configurator
var KPIS_TEXT = 'KPIs';

//Costante elemento del treeview configurator
var MEASURES_TEXT = 'Measures';

//Aggiunte di Ionut
window.kendoTheme = "default-v2";
window.kendoCommonFile = "common-empty";

//Variabile globale necessaria per identificare il treeview all'interno della finestra Includi campi
//aperta dall'utente
var treeviewCampiDaIncludere;

//Variabile globale per disabilitare o abilitare i messaggi di eccezione
var eccezioniAbilitate = true;

$(window).on("load", function () {
    rimuoviTagDuplicati();
});

//Codice di Ionut
function rimuoviTagDuplicati() {
    var $divSelectorToggle = $('.selector-toggle');
    var $divFontHeader = $('h5.font-header');

    if ($divSelectorToggle.length > 1) {
        $divSelectorToggle.not(':first').remove()
    }

    if ($divFontHeader.length > 1) {
        $divFontHeader.not(':first').remove()
    }
}

$(window).on("resize", function () {
    ridimensionaPivotGrid();

    //$("#chart").data("kendoChart").refresh();
});
//fine aggiunte Ionut

//Serve per intercettare l'evento pieno schermo/togli pieno schermo
document.addEventListener("fullscreenchange", function () {
    //Quando si porta la visualizzazione in pieno schermo o quando si toglie
    //la pivot grid ha un problema di visualizzazione e occorre richiedere di ridisegnalra.
    //Il ridisegno deve partire con un certo ritardo altrimenti il cambiamento di dimensioni
    //della finestra del browser avviene dopo che la pivot grid e' stata ridisegnata.
    setTimeout(function () { ridimensionaPivotGrid(); }, 1000);
});

//Classe Cubo per rappresentare il cubo corrente
function Cubo() {
    this.getId = function () {
        return stringToIntValueOrDefault($("#hidIdCat").val());
    }

    this.hasValue = function () {
        return this.getId() > 0;
    }

    this.setId = function (cuboId) {
        $("#hidIdCat").val(cuboId);
    }

    this.getNomeFriendly = function () {
        return $("#hidNomeCat").val();
    }

    this.setNomeFriendly = function (nome) {
        $("#hidNomeCat").val(nome);
        //Scrive il nome del cubo nei link di navigazione
        $("#pathNomeCubo").html(nome);

        //Evidenzia il nome del cubo corrente
        evidenziaCuboCorrente()
    }

    //Nome del cubo corrispondente a quello del database
    this.getNome = function () {
        return $("#hidNomeCuboDatabase").val();
    }

    //Nome del cubo corrispondente a quello del database
    this.setNome = function (nome) {
        $("#hidNomeCuboDatabase").val(nome);
    }
}

//Classe per rappresentare il database corrente
function Database() {
    this.getNome = function () {
        return $("#hidNomeDatabase").val();
    }

    //Nome del cubo corrispondente a quello del database
    this.setNome = function (nome) {
        $("#hidNomeDatabase").val(nome);
    }

    this.getId = function () {
        return stringToIntValueOrDefault($("#hidIdDatabase").val());
    }

    this.setId = function (databaseId) {
        $("#hidIdDatabase").val(databaseId);
    }
}

//Classe per rappresentare il server Olap corrente
function ServerOlap() {
    this.getUrl = function () {
        return $("#hidServerOlapUrl").val();
    }

    this.setUrl = function (valore) {
        $("#hidServerOlapUrl").val(valore);
    }
}

//Classe Report per rappresentare il Report corrente
function Report() {
    //Restituisce l'identificativo del Report
    this.getId = function () {
        return stringToIntValueOrDefault($("#hidId").val());
    }

    //Permette di impostare l'identificativo del Report
    this.setId = function (filtroId) {
        $("#hidId").val(filtroId);
    }

    //Restituisce il nome del Report
    this.getNome = function () {
        return $("#hidNomeFiltro").val();
    }

    //Permette di impostare il nome del Report
    this.setNome = function (nomeFiltro) {
        //Memorizzazione del filtro
        $("#hidNomeFiltro").val(nomeFiltro);

        //Visualizzazione del filtro sulla texbox relativa al salvataggio del filtro
        $("#nomeFiltro").val(nomeFiltro);

        //Visualizzazione del filtro su cui sta lavorando l'utente
        var htmlReportSummary = '';
        if (reportCorrente.isNuovoReport()) {
            htmlReportSummary = '<b>' + cuboCorrente.getNomeFriendly() + ' - ' + nomeFiltro + '</b>';
        }
        else {
            htmlReportSummary = 'Report: <b>' + nomeFiltro + '</b>';
            if (reportCorrente.isReportCondiviso()) {
                htmlReportSummary += ' ' + '<br/><span id="reportInviatoDa">Inviato da ' + reportCorrente.getUtenteNomeCognome()
                    + '<br/>Cubo: ' + cuboCorrente.getNomeFriendly() + '</span>';
            }
        }

        $("#activeFilter").html(htmlReportSummary);

        //Seleziona l'elemento corrispondente al report corrente
        selectCurrentReportItem();
    }

    this.isNuovoReport = function () {
        return this.getId() < 0;
    }

    this.isReportCondiviso = function () {
        return getValueAsBoolean("hidIsReportCondiviso") == true;
    }

    this.setIsReportCondiviso = function (isReportCondiviso) {
        $("#hidIsReportCondiviso").val(isReportCondiviso);
    }

    //Restituisce true se il Report corrente corrisponde ad un Report memorizzato nel database
    this.isReportMemorizzato = function () {
        return this.getId() > 0;
    }

    //Restituisce l'identificativo dell'utente proprietario del Report corrente
    this.getUtenteId = function () {
        return $("#hidReportUtenteId").val();
    }

    //Permette di impostare l'identificativo dell'utente proprietario del Report corrente
    this.setUtenteId = function (value) {
        $("#hidReportUtenteId").val(value);
    }

    //Restituisce il nome dell'utente proprietario del Report
    this.getUtenteNome = function () {
        return $("#hidReportUtenteNome").val();
    }

    //Imposta il nome dell'utente proprietario del Report
    this.setUtenteNome = function (valore) {
        $("#hidReportUtenteNome").val(valore);
    }

    //Restituisce il cognome dell'utente proprietario del Report
    this.getUtenteCognome = function () {
        return $("#hidReportUtenteCognome").val();
    }

    //Imposta il cognome dell'utente proprietario del Report
    this.setUtenteCognome = function (valore) {
        $("#hidReportUtenteCognome").val(valore);
    }

    //Restituisce il nome e il cognome dell'utente proprietario del Report
    this.getUtenteNomeCognome = function () {
        return this.getUtenteNome() + ' ' + this.getUtenteCognome();
    }
}

//Classe per rappresentare l'utente corrente
function Utente() {
    this.getId = function () {
        return stringToIntValueOrDefault($("#hidUtenteId").val());
    }
}

var serverOlapCorrente = new ServerOlap();

//Dati del Database corrente
var databaseCorrente = new Database();

//Dati del Filtro corrente
var reportCorrente = new Report();

//Dati del Cubo corrente
var cuboCorrente = new Cubo();

//Dati dell'utente corrente
var utenteCorrente = new Utente();


// Mette in evidenza il cubo corrente
function evidenziaCuboCorrente() {
    var idCuboCorrente = cuboCorrente.getId();
    if (idCuboCorrente > 0) {
        //Toglie la selezione a tutti i nomi dei cubi
        $('.cubo-text').removeClass('selected');

        //Evidenzia il cubo corrente
        $('#cuboItemTitle_' + idCuboCorrente.toString()).addClass('selected');
    }
}

// Mette in evidenza il report corrente
function selectCurrentReportItem() {
    var idFiltroCorrente = reportCorrente.getId();

    //Toglie la selezione a tutte le voci del menu
    $('.menu-item').removeClass('selected');

    //Nasconde i cubi
    $('.pcoded-submenu').css('display', 'none');
    //Imposta l'indicatore di espansione: +
    $('.pcoded-hasmenu').removeClass('pcoded-trigger');

    if (idFiltroCorrente != 0) {

        //Evidenzia il report corrente
        $('#node_' + idFiltroCorrente.toString()).addClass('selected');

        $('#cubo_' + cuboCorrente.getId().toString()).addClass('selected');

        $('#database_' + databaseCorrente.getId().toString()).addClass('selected');

        espandiMenuReportCorrente();
    }
}

//Espande il menu in modo da mostrare il report corrente
function espandiMenuReportCorrente() {
    //Espande il database corrente
    $('#database_' + databaseCorrente.getId().toString()).next().css('display', 'block');

    //Imposta il database corrente con indicatore di espansione: -
    $('#database_' + databaseCorrente.getId().toString()).parent().addClass('pcoded-trigger');

    //Espande il cubo corrente
    $('#cat_' + cuboCorrente.getId().toString()).css('display', 'block');

    //Imposta il cubo corrente con indicatore di espansione: -
    $('#cubo_' + cuboCorrente.getId().toString()).parent().addClass('pcoded-trigger');
}

//Chiamato ad esempio quando l'utente effettua click sul nome di un Report. 
//Il parametro _id corrisponde all'identificativo univoco del Report.
//In caso di Nuovo report l'id corrispondera' all'identificativo del Cubo cambiato di segno.
//Ad esempio _id = -6 significa che occorre caricare un nuovo Report per il cubo 6.
function CambiaDataSource(_id) {
    if (erroreCaricamentoDati()) {
        reloadPage(_id);
    }

    var nomecolonne = [{ name: "", expand: true }];
    var nomerighe = [{ name: "", expand: true }];
    var nomemisure = [{ name: "", format: "n0", color: "rgb(0,62,117)", aggregate: "sum" }];
    var nomecategoria = "";
    var nomefiltro = "";
    var nomesort = "";

    clearTreeviewConfigurator();
    clearPivotGrid();

    var _par = {
        CuboId: 0,
        ReportId: _id
    };
    _par = stringify(_par);

    pivotGridLoading(false);
    jQuery.ajax({
        url: '/Analysis/GetReport',
        dataType: 'json',
        contentType: "application/json",
        cache: false,
        data: "{'_par': '" + _par + "'}",
        type: 'POST',
        error: function (jqXHR, textStatus, errorThrown) {
            gestisciEccezione("Errore durante il recupero del Report." + " " + ERR_SERVER_O_TIMEOUT);
            return;
        },
        success: function (risultato) {
            try {
                checkEsito(risultato);
                if (isNullOrUndefined(risultato.Dati))
                    throw "I dati del Report non sono stati restituiti";

                var data = risultato.Dati;
                cambiaDataSourceImpostaDati(data);
            }
            catch (err) {
                gestisciEccezione(err);
            }
        }
    });
}

function cambiaDataSourceImpostaDati(data) {
    nomecolonne = jQuery.parseJSON(data.Colonne);
    nomerighe = jQuery.parseJSON(data.Righe);
    nomemisure = jQuery.parseJSON(data.Misure);
    nomecategoria = data.NomeCategoria;
    id = data.Id;
    idcat = data.IdCategoria;
    nomefiltro = data.Nome;
    idutente = data.IdUtente;
    if (data.Filtri !== "") {
        nomefiltri = jQuery.parseJSON(data.Filtri);
    }
    else {
        nomefiltri = "";
    }
    if (data.Sort !== "") {
        nomesort = jQuery.parseJSON(data.Sort);
    }
    else {
        nomesort = "";
    }

    impostaReportCorrente(data);

    var _dSource = new kendo.data.PivotDataSource({
        type: "xmla",
        columns: impostaEspandiPrimoElemento(nomecolonne),
        rows: impostaEspandiPrimoElemento(nomerighe),
        measures: nomemisure,
        transport: {
            connection: {
                catalog: databaseCorrente.getNome(),
                cube: cuboCorrente.getNome()
            },
            read: serverOlapCorrente.getUrl() //olapUrl
        },
        schema: {
            type: "xmla"
        },
        filter: nomefiltri,
        sort: nomesort,
        error: function (e) {
            gestisciKendoException(e);

            return;
        }
    });

    dS = _dSource;

    //ImpostaPivot(nomefiltro, nomecolonne, nomerighe, nomemisure, id, idcat, idutente, nomefiltri, nomesort, nomecategoria);
    try {
        $("#pivotgrid").data("kendoPivotGrid").setDataSource(_dSource);

        //Il configurator usa il datasource della pivot grid. Se non è possibile impostare il datasource è inutile chiamare il configurator.
        caricaTreeviewConfigurator();
    }
    catch (err) {
        throw "Impossibile impostare il datasource della griglia. E' possibile che il cubo sia in manutenzione";
    }

    //Commentato 
    //$('.k-columns').remove();
    //$('#configurator').kendoPivotConfigurator({
    //    dataSource: _dSource,
    //    height: 570
    //});
    //impostaDataboundTreeviewConfigurator();

    //Codice aggiunto 
    //ReloadConfiguration(_dSource);

    // Il caricamente viene ritardato dopo databound pivot grid.
    // caricaTreeviewConfigurator(_dSource);

    //$("#activeFilter").html("Report: " + nomefiltro);
    //$("#hidNomeFiltro").val(nomefiltro);
    //$("#hidId").val(id);
    //$("#hidIdCat").val(idcat);
    //$("#hidIdUtente").val(idutente);

    //Commentato, codice gia presente sopra
    //$("#hidNomeFiltro").val(nomefiltro);
    //$("#hidIdUtente").val(idutente);

    //dS = _dSource;
    BloccaNuovoFiltro();
    //formattaConfigurator();
    visualizzaGrigliaDati(true);
}

function reset(pivot, config) {
    pivot.setDataSource(new kendo.data.PivotDataSource(config));
};

function BloccaNuovoFiltro() {
    //Deseleziona il controllo checkbox per la condivisione del Report
    $("#checkboxCondividi").prop('checked', false);

    //Nasconde i controlli per la condivisione del Report
    condivisioneReportShow(false);

    if (!reportCorrente.isReportMemorizzato() || reportCorrente.isNuovoReport() || reportCorrente.isReportCondiviso() || utenteCorrente.getId() != reportCorrente.getUtenteId()) {
        $("#chkSovrascrivi").prop('checked', false);
        $("#nomeFiltro").prop('readonly', false);
        switchery.handleOnchange(true);
        switchery.disable();
        $("#chkSovrascrivi").prop('disabled', true);
        $("#elimina").hide();
    }
    else {
        $("#chkSovrascrivi").prop('checked', true);
        $("#nomeFiltro").prop('readonly', true);
        switchery.handleOnchange(true);

        switchery.enable();
        $("#chkSovrascrivi").prop('disabled', false);
        $("#elimina").show();
    }
}

// Puo' sollevare eccezioni. 
// Chiamare la funzione usando sempre try catch
function ImpostaPivot(nomefiltro, nomecolonne, nomerighe, nomemisure, id, idcat, idutente, nomefiltri, nomesort, nomecategoria) {
    var _dSource = new kendo.data.PivotDataSource({
        type: "xmla",
        columns: impostaEspandiPrimoElemento(nomecolonne),
        rows: impostaEspandiPrimoElemento(nomerighe),
        measures: nomemisure,
        transport: {
            connection: {
                catalog: databaseCorrente.getNome(),
                cube: cuboCorrente.getNome()
            },
            read: serverOlapCorrente.getUrl() //olapUrl
        },
        schema: {
            type: "xmla"
        },
        filter: nomefiltri,
        sort: nomesort,
        error: function (e) {
            //Non lanciare eccezioni in questo punto perche' la funzione e' asincrona e nessuno catturera' l'eccezione
            gestisciKendoException(e);

            return;
        }
    });

    dS = _dSource;

    pivotgrid = $("#pivotgrid").kendoPivotGrid({
        dataCellTemplate: $("#dataCellTemplate").html(),
        columnHeaderTemplate: $("#headerTemplate").html(),
        rowHeaderTemplate: $("#headerTemplate").html(),
        excel: {
            fileName: "PivotGrid_Export.xlsx",
            proxyURL: "#",
            filterable: true
        },
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            var rows = sheet.rows;
            var indiceRiga, indiceColonna, celle, cella;
            var indiceColonna;

            for (indiceRiga = 0; indiceRiga < rows.length; indiceRiga++) {
                celle = rows[indiceRiga].cells;
                //if (rowIdx == 0) {
                //    cells[0].value = reportCorrente.getNome();
                //}

                for (indiceColonna = 0; indiceColonna < celle.length; indiceColonna++) {
                    cella = celle[indiceColonna]

                    //Aggiunge il titolo nella prima cella in alto a sinistra e lo formatta in modo da essere centrato
                    if (indiceRiga == 0 && indiceColonna == 0) {
                        cella.value = reportCorrente.getNome();
                        cella.hAlign = "center";
                        cella.vAlign = "center";
                        //cella.hAlign = "right";
                    }

                    if (cella.color == '#fff') {
                        //Caso in cui la cella e' un'intestazione
                        //cella.background = '#ff0000';
                    }
                    else {
                        //Caso in cui la cella e' un dato

                        cella.format = "#,##0";
                        //Esempio trovato su internet: https://jsfiddle.net/dwosrs0x/4/
                        //cella.format = "#,##0.00;[Red](#,##0.00);-";

                        //Usando questa istruzione la formattazione andrebbe bene ma Excel poi vedrebbe la cella come stringa riportando un avvertimento
                        //cella.value = kendo.toString(kendo.parseFloat(cella.value), "n0");
                    }
                }
            }
        },
        //Codice originale commentato
        //excelExport: function (e) {
        //    var sheet = e.workbook.sheets[0];
        //    var rows = sheet.rows;
        //    var rowIdx, colIdx, cells, cell;

        //    for (rowIdx = 0; rowIdx < rows.length; rowIdx++) {
        //        if (rows[rowIdx].type === "data") {
        //            cells = rows[rowIdx].cells;

        //            for (colIdx = sheet.freezePane.colSplit; colIdx < cells.length; colIdx++) {
        //                cell = cells[colIdx];

        //                cell.background = "#aabbcc";

        //                //cell.value = kendo.toString(cell.value, "n0");
        //                cell.value = kendo.toString(kendo.parseFloat(cell.value), "n0");
        //            }
        //        }
        //    }
        //},
        //autoBind: false,
        filterable: true,
        sortable: true,
        //gather the collapsed members
        collapseMember: function (e) {
            //Funzione chiamata quando l'utente richiede un'espansione della pivot grid
            var axis = collapsed[e.axis];
            var path = e.path;

            if (indexOfPath(path, axis) === -1) {
                axis.push(path);
            }

            refreshPivotGrid();
            ridimensionaPivotGrid();
        },
        //gather the expanded members
        expandMember: function (e) {

            //Funzione chiamata quando l'utente richiede una contrazione della pivot grid
            var axis = collapsed[e.axis];
            var index = indexOfPath(e.path, axis);

            if (index !== -1) {
                axis.splice(index, 1);
            }

            refreshPivotGrid();
            ridimensionaPivotGrid();
        },
        columnWidth: 100,

        //Modificato, valore originale: 600
        height: "100%",
        //height: 600,

        dataSource: _dSource,
        dataBinding: function (e) {
            visualizzaGrigliaDati(true);
            //    e.preventDefault(); //this will prevent the data bind action
        },
        dataBound: function () {
            //In questa funzione i dati della pivot grid dovrebbero essere stati popolati

            //$("#chart").css("display", "none");
            //$("#div_chart").css("display", "none");

            dopoDataboundPivotGrid();
            var fields = this.columnFields.add(this.rowFields).add(this.measureFields);
            fields.find(".k-button")
                .each(function (_, item) {
                    item = $(item);
                    //var text = item.data("name").split(".").slice(-1) + "";
                    var text = item.data("name");
                    var testoSemplificato = semplificaCampoCuboByString(text);
                    item.contents().eq(0).replaceWith(testoSemplificato);
                    //item.contents().eq(0).replaceWith(text.replace("[", "").replace("]", ""));
                });

            espandiRigaColonna(_dSource);

            ridimensionaPivotGrid();
        }
    }).data("kendoPivotGrid");

    caricaTreeviewConfigurator();

    //@@@paccosi da rivedere, codice commentato perche' troppo lento su alberature estese
    //Codice necessario per intercettare quando la finestra popup Includi Campi della pivot grid viene aperta
    //in modo da effettuare alcune operazioni per correggere un problema.
    //In particolare la finestra non visualizza correttamente le precedenti scelte effettuate dall'utente
    $('[data-role="pivotsettingtarget"]').each(function (indice, setting) {
        var fieldMenu = $(setting).data("kendoPivotSettingTarget").fieldMenu; //get setting FieldMenu
        if (fieldMenu) {
            fieldMenu.includeWindow.bind("open", function () {
                var treeView = fieldMenu.treeView;

                console.log('treeView:', treeView);

                if (treeView) {
                    //Salva il treeview nella variabile globale
                    treeviewCampiDaIncludere = treeView;

                    treeviewCampiDaIncludere.setOptions({ loadOnDemand: false });

                    var pippo = treeviewCampiDaIncludere.dataItem(treeviewCampiDaIncludere.findByText("[Date].[Calendar].[Date].&[20060101]"));
                    treeviewCampiDaIncludere.expandTo(pippo);

                    return;

                    //Disabilita la visualizzazione globale delle eccezioni. Durante il caricamento del treeview
                    //si possono verificare degli errori che non sembrano avere infuenza e 
                    //vengono nascosti all'utente
                    eccezioniAbilitate = false;

                    //Nasconde il treeview
                    treeviewCampiDaIncludere.element.hide();
                    //aumentaDimensioniFinestraPopupIncludiCampi();

                    //Mostra un icona progress
                    kendo.ui.progress($('.k-window-titlebar'), true);

                    //Funzione chiamata dopo che il treeview ha finito di recuperare i dati
                    treeView.bind("dataBound", function (e) {
                        try {
                            //Occorre verificare se il treeview e' visibile perche' dopo la compressione del 
                            //treeview la funzione corrente viene chiamata ancora una volta, non chiaro perche' 
                            if (treeviewCampiDaIncludere.element.is(":hidden")) {

                                //Richiede un espansione del treeview. Per espanderlo tutto saranno necessarie
                                //diverse chiamate alla funzione expand
                                treeviewCampiDaIncludere.expand(".k-item");

                                //Quando il treeview e' espanso tutto quanto non ci saranno piu' elementi
                                //di classe k-i-expand
                                if (treeviewCampiDaIncludere.element.find('span.k-i-expand').length == 0) {

                                    //Richiede la compressione del treeview. Curiosamente dopo questa
                                    //richiesta si verifica un ulteriore databound come se il treeview recuperasse dati
                                    treeviewCampiDaIncludere.element.data("kendoTreeView").collapse(".k-item");

                                    //Mostra il treeview
                                    treeviewCampiDaIncludere.element.show();

                                    //Nasconde l'icona progress
                                    kendo.ui.progress($('.k-window-titlebar'), false);

                                    //Aumenta le dimensioni della finestra popup perche' per default e' molto piccola
                                    aumentaDimensioniFinestraPopupIncludiCampi();

                                    //Abilita la visualizzazione globale delle eccezioni precedentemente disabilitate
                                    eccezioniAbilitate = true;
                                }
                            }
                        }
                        catch (err) {
                        }
                    });
                }
            });
        }
    });

    //Configurator
    //impostaConfigurator(_dSource);

    //$("#activeFilter").html("Report: " + nomefiltro);
    //$("#hidNomeFiltro").val(nomefiltro);
    //$("#hidId").val(id);
    //$("#hidIdCat").val(idcat);
    //$("#hidIdUtente").val(idutente);

    //dS = _dSource;
    BloccaNuovoFiltro();
    //formattaConfigurator();
}

//function espandi (tree) {
//    //var tree = $("#treeview").data("kendoTreeView"),
//        selected = tree.select(),
//        dataItem = tree.dataItem(selected);

//    var load = function (item) {
//        var that = this,
//            chain = $.Deferred().resolve();

//        chain = chain.then(function () {
//            that.expand(that.findByUid(item.uid));
//            return item.load();
//        }).then(function () {
//            if (item.hasChildren) {
//                var childrenChain = $.Deferred().resolve();

//                item.children.data().forEach(function (child) {
//                    (function (child) {
//                        childrenChain = childrenChain.then(function () {
//                            return load.bind(that)(child);
//                        })
//                    })(child)
//                })

//                return childrenChain;
//            }
//        });

//        return chain;
//    }

//    //if (selected.length == 0) {
//    //    alert("Select item first!");
//    //    return;
//    //}

//    load.bind(tree)(dataItem);
//}

// Expands entire tree
function expandTree(treeView) {
    //console.log("Expanding All...");
    //var tree = $("#treeview").data("kendoTreeView");
    //var nodes = document.getElementsByClassName('k-item');
    var nodes = $('[role="treeitem"]');
    for (var i = 0; i < nodes.length; i++) {
        expandNode(nodes[i], treeView);
    }
}

// Expand nodes recursively
function expandNode(htmlElement, tree) {
    //var tree = $("#treeview").data("kendoTreeView");
    var dataItem = tree.dataItem(htmlElement);
    //dataItem.set("expanded", true);
    //var children = dataItem.items;

    var children = dataItem.children();
    if (children) {
        for (var i = 0; i < dataItem.items.length; i++) {
            expandNode(tree.findByUid(dataItem.items[i].uid))
        }
    }
}


// Ottiene i dati per popolare la tabella drill through e mostra la tabella
function mostraDrillThrough(_e) {
    try {
        // Ottengo la cella selezionata a partire dalla prima pivot ----------------------------------------------------
        var target = $(_e.target);
        var grid = $("#pivotgrid").getKendoPivotGrid();

        //Cella della pivot grid premuta dall'utente
        //Attenzione perchè se viene specificata solo una misura, cellInfo.measure vale undefined e il nome 
        //  e il nome della misura deve essere letta dalla variabile globale dS._measures[0]
        //  è possibile ottenere informazioni della cella nel modo seguente ma i risultati non cambiano
        //  var cellInfo = pivotgrid.cellInfo(target[0].colSpan, target[0].rowSpan);
        var cellInfo = grid.cellInfoByElement(target);

        if (!isNullOrUndefined(cellInfo) && cellInfo.dataItem != undefined && cellInfo.dataItem.value != undefined) {
            var arrayString = new Array();

            pivotGridLoading(true);
            //arrayString.concat(drillThroughGetPropertyNameAsArray(dS._measures));

            //arrayStringMisure = drillThroughGetPropertyNameAsArray(dS._measures);
            //if (arrayStringMisure.length == 0)
            //    throw "Per effettuare il drill through è necessario specificare una misura";

            // L'operatore booleano deve essere && altrimenti viene mostrato il messaggio anche se la misura e' stata specificata
            if (isNullOrUndefined(cellInfo.measure) && isNullOrUndefined(dS._measures[0])) {
                showTooltipError("Per effettuare il drill through è necessario specificare una misura");
                return;
            }

            // Quando esiste una sola misura viene letta da dS._measures[0].name perchè cellInfo.measure vale undefined.
            // Quando esistono piu' misure il valore viene letto da cellInfo.measure.name.
            var misuraAsString = (cellInfo.measure === undefined) ? dS._measures[0].name : cellInfo.measure.name;
            if (isNullOrEmptyString(misuraAsString)) {
                showTooltipError("Per effettuare il drill through è necessario specificare una misura");
                return;
            }

            var arrayStringColonne = drillThroughGetMemberNames(cellInfo.columnTuple);
            if (arrayStringColonne.length == 0) {
                showTooltipError("Per effettuare il drill through è necessario specificare una colonna");
                return;
            }

            var arrayStringRighe = drillThroughGetMemberNames(cellInfo.rowTuple);
            if (arrayStringRighe.length == 0) {
                throw "Per effettuare il drill through è necessario specificare una riga";
                return;
            }


            //arrayString.push(arrayStringMisure);
            var arrayString = new Array();
            arrayString.push(misuraAsString);
            arrayString.push(arrayStringColonne);
            arrayString.push(arrayStringRighe);

            //drillThroughGetPropertyNameAsArray(arrayString, cellInfo.columnTuple.members);
            //drillThroughGetPropertyNameAsArray(arrayString, cellInfo.rowTuple.members);

            //@@Titolo finestra modale drill through, da sistemare
            //Annotazione: la seguente istruzione non funziona con IE10:
            // + ', ' + 'Valore: ' + new Intl.NumberFormat('it-IT', { maximumSignificantDigits: 15 }).format(cellInfo.dataItem.value)
            $("#drillThroughTitoloId").html("Dettaglio");
            $("#drillThroughRiepilogoId").html(
                'Riga: ' + '<b>' + semplificaCampoCuboByString(drillThroughGetLastRowNameByCell(cellInfo)) + '</b>'
                + ', ' + 'Colonna: ' + '<b>' + semplificaCampoCuboByString(drillThroughGetLastColumnNameByCell(cellInfo)) + '</b>'
                // ', ' + 'Valore: ' + '<b>' + cellInfo.dataItem.value + '</b>'
                + ', ' + 'Valore: ' + kendo.toString(kendo.parseFloat(cellInfo.dataItem.value), "n0")
            );
            //$("#drillThroughTitoloId").html("Dettaglio " + new Intl.NumberFormat('it-IT', { maximumSignificantDigits: 15 }).format(cellInfo.dataItem.value));

            var query =
                '<Envelope xmlns="http://schemas.xmlsoap.org/soap/envelope/">'
                + '<Header />'
                + '<Body>'
                + '<Execute xmlns = "urn:schemas-microsoft-com:xml-analysis">'
                + '<Command>'
                + '<Statement>'
                + drillThroughGetStatement(arrayString)
                + '</Statement>'
                + '</Command>'
                + '<Properties>'
                + '<PropertyList>'
                + '<Catalog>'
                + databaseCorrente.getNome()
                + '</Catalog>'
                + '<Format>Multidimensional</Format>'
                + '</PropertyList>'
                + '</Properties>'
                + '</Execute>'
                + '</Body>'
                + '</Envelope>';

            showTooltipSuccess('Drill through in elaborazione...');
            drillThroughShowModal(query);
        }
    }
    catch (err) {
        if (manageException)
            gestisciEccezione(err);
    }
    finally {
        pivotGridLoading(false);
    }
}

//Restituisce le colonne / righe necessarie per effettuare una ricerca drill through.
function drillThroughGetMemberNames(columnTupleOrRowTuple) {
    var result = new Array();
    try {
        var n = columnTupleOrRowTuple.members.length;
        for (var i = 0; i < n; i++) {

            //E' stato osservato che nelle colonne puo' mettere una misura. In questo caso va scartata
            //perche' questa procedura deve ricavare righe e colonne
            if (!columnTupleOrRowTuple.members[i].measure) {
                var nomeCampo = columnTupleOrRowTuple.members[i].name;
                nomeCampo = xmlEncode(nomeCampo);
                result.push(nomeCampo);
            }
        }
    }
    catch (err) {
    }

    return result;
}

function drillThroughGetLastColumnNameByCell(cellInfoSelected) {
    var result = '';
    try {
        result = cellInfoSelected.columnTuple.members[0].name;
        result = rimuoviCarattereAnd(result);
    }
    catch (err) {
    }

    return result;
}

function drillThroughGetLastRowNameByCell(cellInfoSelected) {
    var result = '';
    try {
        result = cellInfoSelected.rowTuple.members[0].name;
        result = rimuoviCarattereAnd(result);
    }
    catch (err) {
    }

    return result;
}

//Restituisce gli elementi name dell'array specificato.
//Sostituisce il carattere & con la stringa vuota.
//  pivotGridElement
//      Deve corrispondere a dS._rows oppure dS_comumn oppure _measures 
function pushPivotGridArrayProprietaName(membroPivotGrid) {
    var result = new Array();
    for (var i = 0; i < membroPivotGrid.length; i++) {
        //array
        var elementName = membroPivotGrid[i].name[0];
        result.push(elementName);
    }

    return result;
}

function drillThroughGetPropertyNameAsArray(membroPivotGrid) {
    var result = new Array();
    for (var i = 0; i < membroPivotGrid.length; i++) {
        var nomeCampo = membroPivotGrid[i].name;
        nomeCampo = xmlEncode(nomeCampo);

        result.push(nomeCampo);
    }

    return result;
}

function drillThroughGetStatement(arrayString) {
    return 'DRILLTHROUGH MAXROWS 1000 SELECT'
        + ' ('
        + toStringList(arrayString, ',')
        + ') on 0'
        + ' From'
        + ' [' + cuboCorrente.getNome() + ']';
}

function toStringList(stringList, carattereSeparatore) {
    var result = '';
    for (var i = 0; i < stringList.length; i++) {
        if (i == 0) {
            result = stringList[i];
        }
        else {
            result += carattereSeparatore + ' ' + stringList[i];
        }
    }

    return result;
}

function drillThroughShowModal(query) {
    drillThroughGetOlapResponse(query);
}


$(document).ready(function () {
    //Scrollbar orizzontale pivot grid
    //$("#horizontalSplitter").kendoSplitter({
    //    panes: [{
    //        size: "20%",
    //        collapsible: true
    //    }, {
    //        scrollable: false
    //    }]
    //});

    var nomecolonne = [];  //[{ name: "[Calendario].[Mese]", expand: true }];
    var nomerighe = []; //[{ name: "[Tipo Servizi].[Servizio]", expand: true }];
    var nomemisure = []; //[{ name: "[Measures].[Conteggio]", format: "n0", color: "rgb(0,62,117)", aggregate: "sum" }];
    var id = 0;
    var idcat = 1;
    var nomefiltro = "";
    var nomefiltri = "";
    var nomesort = "";

    $('.k-i-collapse').on('click', function () {
        ridimensionaPivotGrid();
        //console.log('ridimensionaPivotGrid ', 'si');
    });

    //Memorizza l'identificativo di Report riportato dal server. Se valorizzato e' l'identificativo del
    //Report da caricare.
    //A volte il codice javascript richiede il ricaricamento completo della pagina Web e il posizionamento su un
    //Report specifico. In tal caso l'identificativo del Report su cui riposizionarsi viene passato al server
    //dal codice javascript e a sua volta il server lo ripassa al codice javascript. 
    var reportIdDaCaricare = reportCorrente.getId();

    //Memorizza l'identificativo del Cubo riportato dal server
    var cuboId = cuboCorrente.getId();

    //Imposta i parametri per il metodo GetUltimoFiltro in modo da indicare l'identificativo del Report e 
    //l'identificativo del Cubo riportati dal server.
    var _par = {
        CuboId: cuboId,
        ReportId: reportIdDaCaricare
    };
    _par = stringify(_par);

    initReportCorrente();

    visualizzaGrigliaDati(true);

    var elemsingle = document.querySelector('.js-single');
    switchery = new Switchery(elemsingle, { color: '#4099ff', jackColor: '#fff' });

    jQuery.ajax({
        url: '/Analysis/GetReport',
        dataType: 'json',
        contentType: "application/json",
        cache: false,
        data: "{'_par': '" + _par + "'}",
        //data: { cuboId: cuboId },
        type: 'POST',
        //type: 'GET',
        error: function (jqXHR, textStatus, errorThrown) {
            try {
                gestisciEccezione("Errore durante il recupero dell'ultimo Report salvato");

                // caricamento di default
                ImpostaPivot(nomecolonne, nomerighe, nomemisure);
            }
            catch (err) {
                gestisciEccezione(err);
            }
        },
        success: function (risultato) {
            try {
                checkEsito(risultato);

                if (isNullOrUndefined(risultato.Dati))
                    throw "I dati dell'ultimo Report non sono stati restituiti";

                data = risultato.Dati;

                nomecolonne = jQuery.parseJSON(data.Colonne);
                nomerighe = jQuery.parseJSON(data.Righe);
                nomemisure = jQuery.parseJSON(data.Misure);
                id = data.Id;
                idcat = data.IdCategoria;
                nomefiltro = data.Nome;
                nomecategoria = data.NomeCategoria;

                //Identificativo utente del proprietario del Report
                idutente = data.IdUtente;

                if (data.Filtri !== "") {
                    nomefiltri = jQuery.parseJSON(data.Filtri);
                }
                else {
                    nomefiltri = "";
                }
                if (data.Sort !== "") {
                    nomesort = jQuery.parseJSON(data.Sort);
                }
                else {
                    nomesort = "";
                }

                impostaReportCorrente(data);

                ImpostaPivot(nomefiltro, nomecolonne, nomerighe, nomemisure, id, idcat, idutente, nomefiltri, nomesort, nomecategoria);

                //Se è stato richiesto il caricamento di un Report condiviso o in generale di un particolare Report viene caricato.
                //if (idFiltroIniziale != '') {
                //    CambiaDataSource(idFiltroIniziale);
                //}
            }
            catch (err) {
                gestisciEccezione(err);
            }
        }
    });

    //Codice da eseguire premendo il pulsante Salva
    $('#Modal-overflow').on('show.bs.modal', function (event) {
        //Imposta la finestra modale in modo da sovrascrivere il filtro.
        BloccaNuovoFiltro();
    })

    $('#chkSovrascrivi').change(function () {
        if ($(this).is(":checked")) {
            $('#nomeFiltro').attr('readonly', 'readonly').attr('onkeydown', "event.preventDefault()");
            $("#elimina").show();
            $('#tooltipElimina').show();
            $('#nomeFiltro').val($("#hidNomeFiltro").val());
        }
        else {
            $('#nomeFiltro').removeAttr('readonly').removeAttr('onkeydown');
            $("#elimina").hide();
            $('#tooltipElimina').hide();
            $('#nomeFiltro').val($("#hidNomeFiltro").val() + "_");
        }
    });

    $("#iexport").click(function () {
        pivotgrid.saveAsExcel();
    });

    $('#checkboxCondividi').change(function () {
        if ($(this).is(":checked")) {
            condivisioneReportShow(true);
        }
        else {
            condivisioneReportShow(false);
        }
    });

    //click su pulsante per richiedere di mostrare i dati in formato griglia
    $("#pulsanteMostraGrigliaPrincipale").click(function () {
        visualizzaGrigliaDati(true);

        //initChart(convertData(dS, collapsed));
        //$("#div_chart").css("display", "block");
    });

    // ------------------------------------------------------------------------------------------------------------
    // Eliminazione di un filtro

    /// Funzione chiamata quando l'utente conferma di voler eliminare un Report
    $("#btnConfermaEliminaReport").click(function () {
        eliminaReportCorrente();
    });

    /// Funzione chiamata quando l'utente chiede di cancellare la ricerca
    $("#cancellaRicercaOlap").click(function () {
        cancellaRicercaOlap();
    });

    // ------------------------------------------------------------------------------------------------------------

    $("#save").click(function () {
        salvaReportClick();
    });

    // ------------------------------------------------------------------------------------------------------------
    $("#salvaImpostazioni").click(function () {
        salvaReportClick();
    });

    // ------------------------------------------------------------------------------------------------------------
    //  function initChart(data, tipoGrafico) {
    //      if (tipoGrafico == undefined)
    //          tipoGrafico = 'line';

    //      var chart = $("#chart").data("kendoChart");
    //   var themes = kendo.dataviz.ui.themes;

    //      if (!chart) {
    //          var _kUITheme = kendo.deepExtend(
    //      	{},    
    //      	themes.silver,
    //      	{
    //        		chart: {
    //          			// Can contain any chart settings
    //          			seriesColors: ["#000022", "#000044", "#000066", "#000088", "#0000aa", "#0000cc", "#0000ee"]
    //        		}
    //      	}
    //    	    );
    //   themes._kUITheme = _kUITheme;

    //   $("#chart").kendoChart({
    //              //theme: "_kUITheme",
    //dataSource: {
    //                  data: data,
    //                  group: "row"
    //              },
    //              //chartArea: {
    //              //    width: 900,
    //              //    height: 400
    //              //},
    //              seriesDefaults: {
    //                  type: "line",
    //                  stack: true
    //              },
    //              series: [{
    //                  type: tipoGrafico, //"line" oppure "column"
    //                  field: "measure",
    //                  name: "#= group.value # ",
    //                  categoryField: "column"
    //              }],
    //              seriesClick: function (e) {
    //                  //console.log(e);
    //              },
    //              legend: {
    //                  position: "bottom"
    //              },
    //              categoryAxis: {
    //                  labels: {
    //                      rotation: 310
    //                  }
    //              },
    //              valueAxis: {
    //    //type: "log",
    //                  labels: {
    //                      format: "n0"
    //                  }
    //              },
    //              dataBound: function (e) {
    //                  //var categoryAxis = e.sender.options.categoryAxis;

    //                  //if (categoryAxis && categoryAxis.categories) {
    //                  //    categoryAxis.categories.sort();
    //                  //}
    //              },
    //              tooltip: {
    //                  visible: true,
    //    shared: false,
    //                  //format: "N0",
    //                  format: "{0}%",
    //                  template: "#= series.name #: #= value #"
    //              },
    //      	render: function(e) {
    //        		// Clear up the loading indicator for this chart
    //        		var loading = $(".chart-loading", e.sender.element.parent());
    //	kendo.resize($("#chart"));          		
    //	kendo.ui.progress(loading, false);
    //      	}
    //          });
    //      } else {
    //          chart.dataSource.data(data);
    //      }
    //  }

    $(".k-grid-content").kendoTooltip({
        filter: "td",
        content: mostraDrillThrough,
        width: 400,
        height: 100,
        position: "top"
    });

    //Assegna l'evento click ai valori della griglia principale per aprire il drill through
    //Attenzione:
    //  I tag <td> vengono generati dinamicamente dal componente kendo
    // 
    $("#pivotgrid").on('click', 'div.k-grid-content tr:not(.k-grid-footer) td', mostraDrillThrough);

    rimuoviTagDuplicati();
});
//FINE DOCUMENT READY -

/// Funzione chiamata quando si preme il pulsante Salva per salvare un Report
function salvaReportClick() {
    salvaReport(false);
}

function salvaReport(avviaCondivisioneReport) {
    var Controlli = false;
    var _id = 0;

    if ($("#chkSovrascrivi").is(":checked")) {
        _id = $("#hidId").val();
    }
    var nomefiltro = $('#nomeFiltro').val();

    try {
        checkCaratteriSpeciali(nomefiltro);

        if (isNullOrEmptyString(nomefiltro) && !$("#chkSovrascrivi").is(":checked")) {
            showTooltipError('Inserire il nome del Report');
        }
        else {
            Controlli = true;
        }

        if (Controlli === true) {
            var dataSource = pivotgrid.dataSource;

            var Filtri = dataSource.filter();
            var Sort = dataSource.sort();

            if (Filtri === null) {
                Filtri = "";
            }
            if (Sort === null) {
                Sort = "";
            }

            var _par = {
                Colonne: impostaEspandiPrimoElemento(dataSource.columns()),
                Righe: impostaEspandiPrimoElemento(dataSource.rows()),
                Misure: dataSource.measures(),
                Transport: dataSource.transport,
                Filtri: Filtri,
                Sort: Sort,
                Nome: $('#nomeFiltro').val(),
                Id: _id,
                IdCategoria: cuboCorrente.getId(),
                CondividiConUtenti: condivisioneReportGetUtentiSelezionati()
            };
            _par = stringify(_par);
            //_par = JSON.stringify(_par);
            jQuery.ajax({
                url: '/Analysis/SalvaFiltroUtente',
                dataType: 'json',
                contentType: "application/json",
                cache: false,
                data: "{'_par': '" + _par + "'}",
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {

                    if (textStatus == "parsererror") {
                        gestisciEccezione("Errore durante il salvataggio del Report." + " " + ERR_SERVER_O_TIMEOUT);
                    }
                    else
                        //@@Codice originale
                        gestisciEccezione(errorThrown);
                    //alert("ERRORE" + errorThrown);
                    //or you can put jqXHR.responseText somewhere as complete response. Its html.
                },
                success: function (risultato) {
                    try {
                        checkEsito(risultato);
                        showTooltipSuccess('Report salvato con successo');

                        //Nasconde la finestra che permette di riportare il nome del filtro
                        $('#Modal-overflow').modal('hide');

                        //Viene ricaricata la pagina in modo da aggiornare i controlli e posizionarsi sull'ultimo
                        //filtro che dovrebbe corrispondere a quello appena salvato.
                        if (avviaCondivisioneReport) {
                            $('#Modal-overflow-condivisioneReport').modal('show');
                        }
                        else
                            reloadPage();
                    }
                    catch (err) {
                        gestisciEccezione(err);
                    }
                }
            });
        }

    }
    catch (err) {
        showTooltipError(err);
    }
}

//Elimina il report corrente
function eliminaReportCorrente() {
    jQuery.ajax({
        url: '/Analysis/RimuoviFiltro',
        dataType: 'json',
        contentType: "application/json",
        cache: false,
        data: "{'_id': " + parseInt($("#hidId").val()) + "}",
        type: 'POST',
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus == "parsererror") {
                gestisciEccezione("Errore durante l'eliminazione del Report." + " " + ERR_SERVER_O_TIMEOUT);
            }
            else
                //@@Istruzione originale, controllare perche' riporta un'eccezione
                gestisciEccezione(errorThrown);
            //alert("ERRORE" + errorThrown);
            //or you can put jqXHR.responseText somewhere as complete response. Its html.
        },
        success: function (data) {
            try {
                checkEsito(data);

                window.location = '/Analysis/IndexSetCuboId/' + cuboCorrente.getId().toString();
                //Viene ricaricata la pagina. Le istruzionni successive sono state commentate.
                //location.reload();
            }
            catch (err) {
                gestisciEccezione(err);
            }
        }
    });

}

function initChart(data, tipoGrafico) {
    if (tipoGrafico == undefined)
        tipoGrafico = 'line';

    var chart = $("#chart").data("kendoChart");
    //Istruzione necessaria per ridisegnare il grafico con tipologia differente 
    chart = undefined;

    var themes = kendo.dataviz.ui.themes;

    if (!chart) {
        var _kUITheme = kendo.deepExtend(
            {},
            themes.silver,
            {
                chart: {
                    // Can contain any chart settings
                    seriesColors: ["#000022", "#000044", "#000066", "#000088", "#0000aa", "#0000cc", "#0000ee"]
                }
            }
        );
        themes._kUITheme = _kUITheme;

        $("#chart").kendoChart({
            //Abilitare la seguente istruzione per utilizzare i colori personalizzati scritti sopra
            //theme: "_kUITheme",

            dataSource: {
                data: data,
                group: "row"
            },
            //chartArea: {
            //    width: 900,
            //    height: 400
            //},
            seriesDefaults: {
                type: tipoGrafico, // Esempio: "line" oppure "bar"
                stack: true
            },
            series: [{
                type: tipoGrafico, // Esempio: "line" oppure "bar"
                field: "measure",
                name: "#= group.value # ",
                categoryField: "column"
            }],
            seriesClick: function (e) {
                //console.log(e);
            },
            legend: {
                position: "bottom"
            },
            categoryAxis: {
                labels: {
                    rotation: 310
                }
            },
            valueAxis: {
                //type: "log",
                labels: {
                    format: "n0"
                }
            },
            dataBound: function (e) {
                //var categoryAxis = e.sender.options.categoryAxis;

                //if (categoryAxis && categoryAxis.categories) {
                //    categoryAxis.categories.sort();
                //}
            },
            tooltip: {
                visible: true,
                shared: false,
                //format: "N0",
                format: "{0}%",
                template: "#= series.name #: #= value #"
            },
            render: function (e) {
                // Clear up the loading indicator for this chart
                var loading = $(".chart-loading", e.sender.element.parent());
                kendo.resize($("#chart"));
                kendo.ui.progress(loading, false);
            }
        });
    } else {
        chart.dataSource.data(data);
    }
}

//function flatten the tree of tuples that datasource returns
function impostaConfigurator(_dataSource) {
    clearTreeviewConfigurator();
    $("#configurator").kendoPivotConfigurator({
        dataSource: _dataSource,
        filterable: true,
        sortable: true,
        height: 450,
    });

    impostaDataboundTreeviewConfigurator();
}

function flattenTree(tuples) {
    tuples = tuples.slice();

    var result = [];
    var tuple = tuples.shift();
    var idx, length, spliceIndex, children, member;

    while (tuple) {
        //required for multiple measures
        if (tuple.dataIndex !== undefined) {
            result.push(tuple);
        }

        spliceIndex = 0;
        for (idx = 0, length = tuple.members.length; idx < length; idx++) {
            member = tuple.members[idx];
            children = member.children;
            if (member.measure) {
                [].splice.apply(tuples, [0, 0].concat(expandMeasures(children, tuple)));
            } else {
                [].splice.apply(tuples, [spliceIndex, 0].concat(children));
            }
            spliceIndex += children.length;
        }

        tuple = tuples.shift();
    }

    var _c = JSON.stringify(result);
    return result;
}

function clone(tuple, dataIndex) {
    var members = tuple.members.slice();

    return {
        dataIndex: dataIndex,
        members: $.map(members, function (m) {
            return $.extend({}, m, { children: [] });
        })
    };
}

function replaceLastMember(tuple, member) {
    tuple.members[tuple.members.length - 1] = member;
    return tuple;
};

function expandMeasures(measures, tuple) {
    return $.map(measures, function (measure) {
        return replaceLastMember(clone(tuple, measure.dataIndex), measure);
    });
}

//Check whether the tuple has been collapsed
function isCollapsed(tuple, collapsed) {
    var members = tuple.members;
    for (var idx = 0, length = collapsed.length; idx < length; idx++) {
        var collapsedPath = collapsed[idx];
        if (indexOfPath(fullPath(members, collapsedPath.length - 1), [collapsedPath]) !== -1) {
            return true;
        }
    }

    return false;
}

//Work with tuple names/captions
function indexOfPath(path, paths) {
    var path = path.join(",");

    for (var idx = 0; idx < paths.length; idx++) {
        if (paths[idx].join(",") === path) {
            return idx;
        }
    }

    return -1;
}

function fullPath(members, idx) {
    var path = [];
    var parentName = members[idx].parentName;
    idx -= 1;
    while (idx > -1) {
        path.push(members[idx].name);
        idx -= 1;
    }
    path.push(parentName);

    return path;
}

function memberCaption(member) {
    return member.caption
};

function extractCaption(members) {
    //var _c = JSON.stringify($.map(members, memberCaption).join(" "));
    return $.map(members, memberCaption).join(" ");
};

function fullPathCaptionName(members, dLength, idx) {
    var result = extractCaption(members.slice(0, idx + 1));
    var measureName = extractCaption(members.slice(dLength, members.mLength));

    if (measureName) {
        result += " - " + measureName;
    }
    return result;
}

//the main function that convert PivotDataSource data into understandable for the Chart widget format
function convertData(dataSource, collapsed) {

    var columnDimensionsLength = dataSource.columns().length;
    var rowDimensionsLength = dataSource.rows().length;

    var columnTuples = flattenTree(dataSource.axes().columns.tuples || [], collapsed.columns);
    var rowTuples = flattenTree(dataSource.axes().rows.tuples || [], collapsed.rows);
    var data = dataSource.data();
    var rowTuple, columnTuple;
    var idx = 0;
    var result = [];
    var columnsLength = columnTuples.length;

    for (var i = 0; i < rowTuples.length; i++) {
        rowTuple = rowTuples[i];

        if (!isCollapsed(rowTuple, collapsed.rows)) {
            for (var j = 0; j < columnsLength; j++) {
                columnTuple = columnTuples[j];

                if (!isCollapsed(columnTuple, collapsed.columns)) {
                    if (idx > columnsLength && idx % columnsLength !== 0) {
                        for (var ri = 0; ri < rowTuple.members.length; ri++) {
                            for (var ci = 0; ci < columnTuple.members.length; ci++) {
                                //do not add root tuple members, e.g. [CY 2005, All Employees]
                                //Add only children
                                if (!columnTuple.members[ci].parentName || !rowTuple.members[ri].parentName) {
                                    continue;
                                }

                                result.push({
                                    measure: Number(data[idx].value),
                                    column: fullPathCaptionName(columnTuple.members, columnDimensionsLength, ci),
                                    row: fullPathCaptionName(rowTuple.members, rowDimensionsLength, ri)
                                });
                            }
                        }
                    }
                }
                idx += 1;
            }
        }
    }
    var _c = JSON.stringify(result);
    return result;
}

/*
Formatta il Configurator presente nella finestra di ricerca a comparsa
Dettagli:
    Aggiunge un div vuoto:
        <div> 
        </div>
    
    Racchiude righe, colonne valori ognuno con un proprio div
    <div>
        RIGHE
        Righe inserite
    </div>
    <div>
        COLONNE
        Colonne inserite
    </div>
    <div>
        MISURE
        Misure inserite
    </div>
*/
function formattaConfigurator() {
    //Se' stata gia' effettuata la formattazione una volta non esegue istruzioni
    if (getById("kcBoxVuoto").length == 0) {
        //Operazione preliminare. Aggiunge la classe kc-colonne a COLONNE e alle colonne inserite
        $('div.k-list-container.k-reset:eq(0)').addClass('kc-colonne').prev().addClass('kc-colonne');

        //Operazione preliminare. Aggiunge la classe kc-righe a RIGHE e alle righe inserite
        $('div.k-list-container.k-reset:eq(1)').addClass('kc-righe').prev().addClass('kc-righe');

        //Operazione preliminare. Aggiunge la classe kc-misure a MISURE e alle misure inserite
        $('div.k-list-container.k-reset:eq(2)').addClass('kc-misure').prev().addClass('kc-misure');


        //Racchiude le colonne con un div
        $('.kc-colonne').wrapAll("<div class='kc-box' />");

        //Racchiude le righe con un div
        $('.kc-righe').wrapAll("<div class='kc-box' />");

        //Racchiude le misure con un div
        $('.kc-misure').wrapAll("<div class='kc-box' />");


        //Inserisce un div vuoto di classe kc-box prima del div precedentemente inserito
        $("div.kc-box:eq(0)").before('<div id="kcBoxVuoto" class="kc-box">&nbsp</div>');
        //Fine formattazione COLONNE, RIGHE, MISURE


        //
        //Regola la larghezza del riquadro contenente i campi/dimensioni che è possibile trascinare 
        //Regola il riquadro delle destinazioni (righe, colonne, valori)
        //
        $('.k-columns.k-state-default.k-floatwrap').children().first().css('width', '35%').css('height', '100%').next().css('width', '60%').css('height', '100%');
        //$('.k-columns.k-state-default.k-floatwrap').children().first().css('width', '35%').next().css('width', '60%');
    }
}

//Permette di impostare la visione della griglia (dati numerici) oppure del grafico.
//  mostraGriglia   
//      true per mostrare la griglia e nascondere il grafico
//      false per nascondere la grigila e mostrare il grafico
function visualizzaGrigliaDati(mostraGriglia) {
    var nomeClasse = 'pulsante-selected';
    $("#pulsanteMostraGrigliaPrincipale").removeClass(nomeClasse);
    $("#pulsanteMostraGrafico").css("border", "none").css("background-color", "white").css("color", "silver");
    //$("#pulsanteMostraGrafico").removeClass(nomeClasse);


    if (mostraGriglia) {
        $("#divContenitorePivotGrid").css("display", "block");
        $("#chart").css("display", "none");

        $("#pulsanteMostraGrigliaPrincipale").addClass(nomeClasse);
    }
    else {
        $("#divContenitorePivotGrid").css("display", "none");
        $("#chart").css("display", "block");
        $("#pulsanteMostraGrafico").css("border", "1px solid black").css("color", "black");
        //$("#pulsanteMostraGrafico").addClass(nomeClasse);
    }
}

function dopoDataboundPivotGrid() {
    //Istruzione non piu' necessaria, perche' e' necessario anticiparla altrimenti lo spazio riservato per le colonne
    //puo' essere calcolato male.
    //Viene chiamata dal file analisys Index.cshtml, nel template per righe/colonne
    //impostaColonneRicercaPivotGridSuUnaRiga();

    //Effettua la semplificazione dei campi del configurator. 
    //La semplificazione e' necessaria all'inizio e ogni volta che trasciniamo/togliamo un campo nel configurator.
    semplificaCampiCuboBySelettore("#configurator li.k-item.k-header");

    formattaPulsantiFinestrePopup();
}

// I pulsanti delle finestre popup della pivot grid ("Includi campi" e "Filtro per campi") originariamente sono grigi con testo bianco.
// Vengono formattati in modo da apparire identici al pulsante OK nella finestra popup del salvataggio del report
// (blu con testo bianco)
function formattaPulsantiFinestrePopup() {
    //Pulsanti ok ad esempio finestra popup Includi campi
    $('a.k-button-ok').removeClass('k-button').removeClass('k-primary').addClass('btn').addClass('btn-primary').addClass('btn-sm');

    //Pulsanti cancel ad esempio finestra popup Includi campi
    $('a.k-button-cancel').removeClass('k-button').addClass('btn').addClass('btn-primary').addClass('btn-sm').css('margin-left', '5px');

    //Pulsanti filtra, ad esempio finestra popup Filtro per campi
    $('a.k-button-filter').removeClass('k-button').removeClass('k-primary').addClass('btn').addClass('btn-primary').addClass('btn-sm');

    //Pulsanti Pulisci ad esempio finestra popup Filtro per campi
    $('a.k-button-clear').removeClass('k-button').addClass('btn').addClass('btn-primary').addClass('btn-sm').css('margin-left', '5px');
}

// Aumenta le dimensioni delle finestre popup Includi Campi
// Chiamare la funzione solo dopo aver individuato che la finestra popup e' stata aperta
// altrimenti il codice non ha effetto.
// Tenere presente che esistono piu' treeview. Il codice esclude il treeview appartenente al treeview configurator.
function aumentaDimensioniFinestraPopupIncludiCampi() {
    $('.k-popup-edit-form .k-treeview-lines').css('padding-bottom', '50px');
}

//Semplifica i valori delle Colonne, Righe e misure. Ad esempio [Calendario].[Anno] diventerà Anno
//     selettore = specificare un selettore jquery valido per identificare i tag contenenti i valori da sostituire.
//                  Esempio: ".k-item.k-header" per identificare i tag da modificare contenuti nella finestra a scomparsa
// @@Modificare in modo da chiamare la funzione semplificaCampoCuboByString
function semplificaCampiCuboBySelettore(selettore) {
    $(selettore)
        .each(function (indice, item) {
            var s = semplificaCampoCuboByString(item.innerText);
            item = $(item);
            item.contents().eq(0).replaceWith(s);
        });
}

//Semplifica il valore di una riga, colonna o misura.
//Esempio:
//  [Calendario].[Anno]
//      diventa
//  Anno
function semplificaCampoCuboByString(nomeCampo) {
    var result = "";
    var campoSemplificato = rimuoviCaratteriCampoCubo(nomeCampo);
    var campoSplitted = campoSemplificato.split(".");

    var ultimoIndice = campoSplitted.length - 1;
    var ultimoCampo = campoSplitted[ultimoIndice];

    result = ultimoCampo;

    //Caso in cui il campo e' formato almeno da una coppia, esempio:
    //  Data Acquisizione
    //  Data
    if (campoSplitted.length > 1) {
        var penultimoCampo = campoSplitted[ultimoIndice - 1];

        //Caso in cui il penultimo campo contiene l'ultimo. Restituisce il penultimo campo
        if (containsWithoutCase(penultimoCampo, ultimoCampo)) {
            result = penultimoCampo;
        }
        else
            //Caso in cui l'ultimo campo contiene il penultimo. Restituisce l'ultimo campo
            if (containsWithoutCase(ultimoCampo, penultimoCampo) || isOlapCommonName(penultimoCampo)) {
                result = ultimoCampo;
            }
            else
                //Caso in cui entrambi i campi contengono informazioni differenti. Restituisce entrambi i campi.
                result = penultimoCampo + '.' + ultimoCampo;
    }

    return result;
}

//Restituisce true se la prima stringa contiene la seconda,
//altrimenti restituisce false.
//Il confronto è case insensitive.
function containsWithoutCase(mainString, secondString) {
    var result = false;
    if (!isNullOrEmptyString(mainString) && !isNullOrEmptyString(secondString)) {
        var regExp = new RegExp(secondString, 'i');
        result = mainString.search(regExp) != -1;
    }

    return result;

}

//Restituisce true se il campo specificato corrisponde ad un nome comune usato in ambito Olap.
//Altrimenti restituisce false.
function isOlapCommonName(nomeCampo) {
    var result = false;
    if (!isNullOrEmptyString(nomeCampo)) {
        var arrayOfNames = getOlapCommonNames();
        for (var item in arrayOfNames) {
            if (arrayOfNames[item].toLowerCase() == nomeCampo.toLowerCase()) {
                result = true;
                break;
            }
        }
    }

    return result;
}

//Restituisce un array di stringhe contenenti nomi comuni usati in ambito Olap
function getOlapCommonNames() {
    return [
        "Measures"
        , "Measure"
        , "Misure"
        , "Misura"
    ];
}



function rimuoviCaratteriCampoCubo(nomeCampo) {
    var result = rimuoviParentisiQuadre(nomeCampo);
    result = rimuoviCarattereDollaro(result);

    return result;
}

function drillThroughGetOlapResponse(query) {
    try {
        var url = serverOlapCorrente.getUrl();
        if (isNullOrEmptyString(url))
            throw "Server da contattare non valorizzato. Configurare il server utilizzando le funzionalità di amministrazione dell'applicazione";

        jQuery.ajax({
            url: serverOlapCorrente.getUrl(), //olapUrl,
            dataType: 'text',
            contentType: "text/xmla",
            cache: false,
            data: query,
            type: 'POST',
            timeout: DRILL_THROUGH_TIMEOUT,
            error: function (jqXHR, textStatus, errorThrown) {
                pivotGridLoading(false);

                if (textStatus == "timeout") {
                    gestisciEccezione("Timeout della richiesta drill through");
                }
                else
                    gestisciEccezione(errorThrown);
            },
            success: function (data) {
                //Ricordare che è una risposta asincrona e non contiene un esito
                pivotGridLoading(false);

                if (drillThroughElaboraOlapResponse(data)) {
                    $('#drillThroughModalId').modal('show');
                    //Originale:
                    //$('#modal-2').modal('show');
                }

            }
        });

    }
    catch (err) {
        gestisciEccezione(err);
    }
}

//Funzione da chiamare per elaborare la risposta Olap drill through
//Restituisce true se l'operazione è andata a buon fine
function drillThroughElaboraOlapResponse(httpResponse) {
    var result = false;
    var parser = new DOMParser();
    var xmlDocument = parser.parseFromString(httpResponse, "text/xml");
    var xsdRowSequence = drillThroughGetXsdRowSequence(xmlDocument);
    if (xsdRowSequence == undefined) {
        gestisciEccezione('Drill through non disponibile');
    }
    else {

        var colonne = new Array();
        var colonnaId;
        var nomeColonna;
        var valoreColonna;

        //Costruisce la struttura dati colonne, un array associativo.
        //      Esempio: 
        //          colonne['_x005B__x0024_Dettaglio_x0020_Cliente_x005D_._x005B_Codice_x0020_Fiscale_x005D_']
        //              = 
        //                  {   
        //                      nome: "[$Dettaglio Cliente].[Codice Fiscale]", 
        //                      indice: 0
        //                  }
        for (var i = 0; i < xsdRowSequence.length; i++) {
            var nomeColonna = xsdRowSequence[i].attributes['sql:field'].value;
            colonnaId = xsdRowSequence[i].attributes['name'].value;

            colonne[colonnaId] = {
                nome: nomeColonna,
                indice: i
            }
        }

        //Costuisce la struttura dati righe.
        //Esempio: 
        //    righe[1][0] = '35'
        //      significa assegna alla riga 1 colonna 0 la stringa '35'
        var righe = new Array();
        var rowItems = xmlDocument.getElementsByTagName("row");
        for (var i = 0; i < rowItems.length; i++) {
            righe[i] = new Array();

            for (var j = 0; j < rowItems[i].childNodes.length; j++) {
                colonnaId = rowItems[i].childNodes[j].tagName;
                valoreColonna = rowItems[i].childNodes[j].textContent;
                indiceColonna = colonne[colonnaId].indice;

                righe[i][indiceColonna] = valoreColonna;
            }
        }

        drillThroughDisposeControls();
        drillThroughMostraTabella(righe, colonne);

        result = true;
    }

    return result;
}

//Restituisce l'elemento xml rowSequence contenente le colonne del drill through
function drillThroughGetXsdRowSequence(xmlDocument) {
    var result = undefined;
    try {
        var complexTypeRow = xmlDocument.getElementsByTagNameNS("http://www.w3.org/2001/XMLSchema", "complexType");
        //@@Modificare una volta selezionato precisamente il tag sopra
        if (complexTypeRow.length > 2) {
            result = complexTypeRow[2].childNodes[0].childNodes
            //result = complexTypeRow[2].firstElementChild.children;
        }
    }
    catch (err) {
    }

    return result;
}

//Funzione da chiamare per eliminare i controlli previsti per la visualizzazione drill through
function drillThroughDisposeControls() {
    drillThroughRemoveTable();
}

//Funzione da chiamare per eliminare il tag contenente la tabella drill through
function drillThroughRemoveTable() {
    //removeTag(DRILL_THROUGH_SECONDO_DIV_ID);
    removeTag('drillThroughTable_wrapper');
}

//Funzione da chiamare per mostrare una specifica tabella drill through
function drillThroughMostraTabella(righe, colonne) {
    $('#' + DRILL_THROUGH_DIV_PRINCIPALE_ID).append(drillThroughGetHtmlTableAsString(righe, colonne));

    //Formatta la tabella utilizzando un plugin.
    $('#' + DRILL_THROUGH_TABLE_ID).DataTable({
        //Le seguenti istruzioni implementano lo scroll dellta tabella e sono state commentate.
        //Lo scroll funziona male.
        //"scrollY": true,
        //"scrollX": true,
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'copy',
                text: 'Copia'
            },
            {
                extend: 'csv',
                text: 'Salva CSV'
            },
            {
                extend: 'excel',
                text: 'Salva Excel'
            },
            {
                extend: 'pdf',
                text: 'Salva PDF'
            }
        ],
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
            }
        }
    });

    //Istruzioni aggiuntive per aggiustare la formattazione della tabella
    aggiustaTabellaDrillThrough();
}

function aggiustaTabellaDrillThrough() {
    //Inserisce un div in modo da racchiudere il tag <table> contenente la tabella drill through.
    //L'istruzione serve per implementare lo scroll della tabella.
    getById(DRILL_THROUGH_TABLE_ID).wrapAll("<div id='divDrillThroughTable' style='overflow: auto;' />");

    //La seguente istruzione corregge un problema osservato su Edge.
    //La scrollbar della tabella drillthrough non permette di leggere bene i dati dell'ultima riga.
    getById('divDrillThroughTable').append('<br />');
}

//Restituisce la stringa html contenente la tabella drill through con le righe e le colonne specificate
function drillThroughGetHtmlTableAsString(righe, colonne) {
    var result =
        '<table id="' + DRILL_THROUGH_TABLE_ID + '" class="display">'
        + '<thead>'
        + '<tr>'
        + drillThroughGetTagThList(colonne)
        + '</tr>'
        + '</thead>'
        + '<tbody>'
        + drillThroughGetTagTrList(righe, colonne)
        + '</tbody>'
        + '</table>'

    return result;
}

//Restituisce la stringa html contenente le intestazioni della  tabella drill through specificata
function drillThroughGetTagThList(colonne) {
    var result = '';
    for (var key in colonne) {
        var encodedValue = htmlEncode(colonne[key].nome);
        //@@Codificare la chiave in modo da poter essere visualizzata sempre
        result +=
            '<th>'
            + drillThroughSemplificaNomeColonna(encodedValue)
            + '</th>';
    }

    return result;
}

//Semplifica il nome della colonna.
//Esempio:
//  nomeColonnaEffettiva="[$DettaglioCliente].[CodiceFiscale]"
//          diventa:
//    DettaglioCliente
//    CodiceFiscale
function drillThroughSemplificaNomeColonna(nomeColonnaEffettiva) {
    //var s = rimuoviParentisiQuadre(nomeColonnaEffettiva);
    //s = rimuoviCarattereDollaro(s);
    var result = semplificaCampoCuboByString(nomeColonnaEffettiva);
    result = result.replace('.', '<br />')

    return result;
}

function rimuoviParentisiQuadre(stringValue) {
    return stringValue.replace(/\[/g, '').replace(/\]/g, '');
}

function rimuoviCarattereDollaro(stringValue) {
    return stringValue.replace(/\$/g, '');
}

function rimuoviCarattereAnd(stringValue) {
    return stringValue.replace(/&/g, '');
}

//Restituisce la stringa html contenente le righe della tabella drill through specificata
function drillThroughGetTagTrList(righe, colonne) {
    var result = '';
    for (var i = 0; i < righe.length; i++) {
        result +=
            '<tr>'
            + drillThroughGetTagTdList(righe[i], colonne)
            + '</tr>'
    }

    return result;
}

//Restituisce la stringa html contenente i valori della tabella drill through specificata
function drillThroughGetTagTdList(riga, colonne) {
    var result = '';
    var numeroColonne = getNumeroElementiArrayAssociativo(colonne);
    for (var j = 0; j < numeroColonne; j++) {
        var value = riga[j];
        var encodedValue = htmlEncode(value);

        result += '<td>' + encodedValue + '</td>';
    }

    return result;
}

//Elimina il tag corrispondente all'id specificato.
//Se il tag non esiste non saranno effettuate operazioni.
function removeTag(tagId) {
    var id = '#' + tagId;
    var element = $(id);
    if (element.length > 0) {
        element.remove();
        //element.remove(id);
    }
}


//***************
//*************** FINE CODICE PER VISUALIZZARE LA TABELLA DRILL THROUGH ***************
//***************
function pivotGridLoading(mostraLoading) {
    if (mostraLoading) {
        getById("pivotGridLoading").show();
    }
    else
        getById("pivotGridLoading").hide();
    //cursoreImpostaByStyle('wait');
}

function mostraGrafico(tipoGrafico) {
    var dati = convertData(dS, collapsed);
    //$('#chart').hide();
    //$('#chart').show();
    //kendo.resize($(".chart-wrapper"));

    if (dati.length == 0) {
        showTooltipError('Per disegnare il grafico è necessario espandere righe e colonne');
    }
    else {
        $('#chart').hide();
        $('#chart').show();
        kendo.resize($(".chart-wrapper"));

        //initChart(dati);
        // Spin all loading indicators on the page
        kendo.ui.progress($(".chart-loading"), true);

        $(document).bind("kendo:skinChange", initChart(dati, tipoGrafico));

        visualizzaGrigliaDati(false);
    }
}

//Imposta la funzione da chiamare quando il treeview contenente i campi di ricerca ha recuperato i dati.
function impostaDataboundTreeviewConfigurator() {
    getTreeviewConfigurator().bind("dataBound", databoundTreeviewConfigurator);
}

//Funzione chiamata normalmente quando il Treeview contenente i campi di ricerca ha recuperato i dati.
//Quando il treeview è popolato nel caso l'utente espanda un nodo il seguente metodo viene
//chiamato in quando avviene un recupero di dati dal server olap.
//E' stato osservato che e' possibile che il componenente sollevi l'evento databound diverse volte durante il caricamento
//degli elementi. Per tale motivo occorre assicurarsi:
//  -Che il treeview e' stato popolato
//  -Che il primo elemento non corrisponda a Measures
function databoundTreeviewConfigurator(e) {
    //Quando e.node è diverso da undefined non bisogna effettuare il riordinamento del treeview perchè
    //  è stato già effettuato. In particolare il primo livello del treeview
    //  è già popolato e l'utente ha semplicemente chiesto l'espansione di un nodo
    if (isNullOrUndefined(e.node) && isTreeviewConfiguratorFilled() && !treeviewConfiguratorHasOrdineCorretto()) {
        formattaConfigurator();
        riordinaTreeview()
    }
}

//Imposta espandi=true per il primo elemento dell'array.
//E' richiesto un array contenente oggetti con le seguenti due proprietà: Name, Expand
function impostaEspandiPrimoElemento(arrayOfNameExpand) {
    if (arrayOfNameExpand.length > 0) {
        arrayOfNameExpand[0].expand = true;
    }

    return arrayOfNameExpand;
}

//Espande la prima colonna e la prima riga della ricerca.
function espandiRigaColonna(source) {
    espandiElemento(source, true);
    espandiElemento(source, false);
}

//Escluso codice expandColumn e expandRow.
//Causa un bug nella pivot grid.
//Dettagli:
//  Cubo: Afiniti, Misura: Attivazioni nette, righe: Pista, colonna: DataAttivazione.data
//  Trascinando DataAttiEvazione e portandola sotto Pista e poi trascinando Pista e portandola
//  come colonna il componente effettua una query errata e viene visualizzato l'errore:
//      "the measures hierarchyalready appears in the axis0"
function espandiElemento(source, espandiColonna) {
    try {
        var arrayOfNameExpand = espandiColonna ? source._columns : source._rows;
        if (arrayOfNameExpand.length > 0) {
            var oggettoOfNameExpand = arrayOfNameExpand[0];
            oggettoOfNameExpand.expand = true;
        }
    }
    catch (err) {
    }
}

//Codice originale:
//function espandiElemento(source, espandiColonna) {
//    try {
//        var arrayOfNameExpand = espandiColonna ? source._columns : source._rows;
//        if (arrayOfNameExpand.length > 0 && !arrayOfNameExpand[0].expand) {
//            var oggettoOfNameExpand = arrayOfNameExpand[0];
//            oggettoOfNameExpand.expand = true;
//            if (espandiColonna)
//                source.expandColumn(oggettoOfNameExpand.name[0]); //l'argomento corrisponde ad una stringa, esempio: "[Ambito].[Ambito]"
//            else
//                source.expandRow(oggettoOfNameExpand.name[0]);
//        }
//    }
//    catch (err) {
//    }
//}

//Riordina il treeview Kendo che permette di trascinare campi e costruire ricerche.
//In particolare imposta Measures/Misure come primo elemento seguito da KPIs
function riordinaTreeview() {
    impostaComePrimoElementoTreeviewConfigurator(KPIS_TEXT);
    impostaComePrimoElementoTreeviewConfigurator(MEASURES_TEXT);
    impostaClassePrimoUltimoElementoTreeviewConfigurator();
}

function impostaComePrimoElementoTreeviewConfigurator(testoElemento) {
    $('#configurator ul.k-treeview-lines > li > div > span').filter(function () {
        return ($(this).text().toLowerCase() === testoElemento.toLowerCase());
    }).parent().parent().detach().prependTo($('#configurator ul.k-treeview-lines'));
}

function impostaClassePrimoUltimoElementoTreeviewConfigurator() {
    $('#configurator ul.k-treeview-lines > li').removeClass('k-first').removeClass('k-last');
    $('#configurator ul.k-treeview-lines > li > div').removeClass('k-top').removeClass('k-bot');

    $('#configurator ul.k-treeview-lines > li > div').addClass('k-mid');

    $('#configurator ul.k-treeview-lines > li:first-child').addClass('k-first');
    $('#configurator ul.k-treeview-lines > li:first-child > div').removeClass('k-mid').addClass('k-top');;

    $('#configurator ul.k-treeview-lines > li:last-child').addClass('k-last');
    $('#configurator ul.k-treeview-lines > li:last-child > div').removeClass('k-mid').addClass('k-bot');
}

// Restituisce true se il treeview configurator è stato popolato
// La funzione e' stata introdotta perche' sembra che il treeview configurator a volte sollevi l'evento databound
// mentre si sta disegnando
function isTreeviewConfiguratorFilled() {
    return getConfiguratorItems().last().hasClass('k-last');
    //$('ul.k-treeview-lines > li:last-child').hasClass('k-last');
}

//Restituisce true se il primo elemento del Treeview Configurator corrisponde a Measures e il secondo a Kpis.
// La funzione e' stata introdotta perche' e' stato osservato che il treeview configurator a volte solleva
//l'evento databound mentre si sta disegnando
function treeviewConfiguratorHasOrdineCorretto() {
    var result = false;
    try {
        var items = getConfiguratorItems();
        var firstText = items.first().text();
        var secondText = items.first().next().text();
        if (!isNullOrUndefined(firstText) && !isNullOrUndefined(secondText)) {
            result =
                stringEqualsWithoutCase(firstText, MEASURES_TEXT)
                &&
                stringEqualsWithoutCase(secondText, KPIS_TEXT);
        }
    }
    catch (err) {
    }

    return result;
}

//Restituisce gli elementi del treeview configurator di primo livello
function getConfiguratorItems() {
    return $('#configurator ul.k-treeview-lines').children()
}

//Carica i campi che permetteranno di effettuare ricerche
function caricaTreeviewConfigurator() {
    //Ottiene il datasource della pivotgrid
    var dataSource = pivotgrid.dataSource;

    var configurator = getConfigurator();
    if (isNullOrUndefined(configurator)) {
        clearTreeviewConfigurator();
        impostaConfigurator(dataSource);
    }
    else {
        //Attenzione perche' dopo aver impostato il nuovo datasource curiosamente
        //e' stato osservato che il componente puo' caricare subito il treeview usando il vecchio datasource
        configurator.setDataSource(dataSource);

        //Istruzione aggiunta in quanto la precedente istruzione sembra a volte caricare il treeview con il vecchio datasource
        clearTreeviewConfigurator();

        var tree = getTreeviewConfigurator();
        //tree.dataSource.trigger("stateReset");

        //Popola il treeview
        tree.dataSource.read();
    }
}

//Elimina gli elementi del treeview configurator
//L'operazione sembra necessaria perche' a volte nel treeview i campi vengono duplicati.
function clearTreeviewConfigurator() {
    $('#configurator li[role="treeitem"]').remove();
}

//Inizializza il Report corrente con valori vuoti
function initReportCorrente() {
    serverOlapCorrente.setUrl('');

    databaseCorrente.setId(-1);
    databaseCorrente.setNome('');

    cuboCorrente.setId(0);
    cuboCorrente.setNome('');
    cuboCorrente.setNomeFriendly('');

    //L'identificativo del report corrente deve essere inizializzato a zero e indica nessun report.
    //Il valore -1 non e' indicato in quanto corrisponderebbe al nuovo report del cubo con identificativo 1 
    reportCorrente.setId(0);
    reportCorrente.setNome('');
    reportCorrente.setUtenteId(0);
    reportCorrente.setUtenteCognome('');
    reportCorrente.setUtenteNome('');
}

function impostaReportCorrente(data) {
    var cuboIdOld = cuboCorrente.getId();

    serverOlapCorrente.setUrl(data.ServerOlapUrl);

    databaseCorrente.setId(data.IdDatabase);
    databaseCorrente.setNome(data.DbName);

    cuboCorrente.setId(data.IdCategoria);
    cuboCorrente.setNome(data.CubeName);
    cuboCorrente.setNomeFriendly(data.NomeCategoria);

    reportCorrente.setId(data.Id);
    reportCorrente.setUtenteId(data.IdUtente);
    reportCorrente.setUtenteNome(data.UtenteNome);
    reportCorrente.setUtenteCognome(data.UtenteCognome);
    reportCorrente.setIsReportCondiviso(data.Condiviso);

    chiudiFinestrePopup();

    //Al momento deve essere l'ultima istruzione.
    reportCorrente.setNome(data.Nome);

    if (data.IdCategoria != cuboIdOld)
        onCuboChange();
}

// Se la finestra "Includi campi" rimane aperta dopo il cambiamento di un report riporta un contenuto errato
// Per tale motivo quando avviene il cambiamento di un report e' necessario chiuderla
function chiudiFinestrePopup() {
    //Al momento la seguente selezione individua piu' tag oltre a quello della finestra "Includi campi"
    $('div.k-widget.k-window').hide();
}

//Funzione chiamata quando e' stato rilevato che il cubo a cui appartiene il Report e' cambiato
function onCuboChange() {
    condivisioneReportClear();
}

// Gestisce l'eccezione contenente il messaggio di errore specificato.
function gestisciEccezione(errorMessage) {
    var logEntry = new LogEntry();
    logEntry.Messaggio = errorMessage + eccezioniAbilitate ? "" : " messaggio non visibile all'utente";
    writeLogWithoutException(logEntry);
    if (eccezioniAbilitate) {
        showTooltipError(errorMessage);
    }
}

// Gestisce l'eccezione specificato. Il parametro deve corrispondere all'oggetto restituito dal componente 
// kendo su evento errore. 
//Esempio di valore dell'oggetto:
//xhr: null
//status: "customerror"
//errorThrown: "custom error"
//errors: Array(1)
//0: { faultstring: "Query (1, 19) The dimension '[Causale Chiusura666]…sale Chiusura666].[Causale Chiusura], was parsed.", faultcode: "XMLAnalysisError.0xc10a0006" }
//length: 1
function gestisciKendoException(e) {
    if (!isNullOrUndefined(e) && !isNullOrUndefined(e.errors) && !isNullOrUndefined(e.errors[0])) {
        var errorMessage = toStringNotNull(e.errors[0].faultstring) + ' - Codice errore: ' + toStringNotNull(e.errors[0].faultcode);
        gestisciEccezione(errorMessage);
    }
    else {
        showTooltipError(MSG_ERRORE_DETTAGLIO_NON_DISPONIBILE);
    }
}

// Carica gli utenti con ruolo User abilitati al cubo corrente e popola l'area di condivisione Report
function condivisioneReportLoad() {
    condivisioneReportClear();
    if (cuboCorrente.hasValue()) {
        jQuery.ajax({
            url: '/Analysis/GetUtentiInvioReport',
            dataType: 'json',
            contentType: "application/json",
            cache: false,
            data: { id: cuboCorrente.getId() },
            type: 'GET',
            error: function (jqXHR, textStatus, errorThrown) {
                gestisciEccezione("Errore durante il recupero degli utenti." + " " + ERR_SERVER_O_TIMEOUT);
                return;
            },
            success: function (risultato) {
                try {
                    checkEsito(risultato);
                    if (isNullOrUndefined(risultato.Dati))
                        throw "I dati degli utenti non sono stati restituiti";

                    condivisioneReportfillControl(risultato.Dati);
                }
                catch (err) {
                    gestisciEccezione(err);
                }
            }
        });
    }
}

//Aggiunge gli Utenti al componente condivisione Report
function condivisioneReportfillControl(listaUtenti) {
    var $listaUtenti = condivisioneReportGetContent();
    $listaUtenti.children().remove();
    for (var i in listaUtenti) {
        var utente = listaUtenti[i];
        $listaUtenti.append('<option value="' + utente.PkUtenteId + '">' + utente.Cognome + ' ' + utente.Nome + '</option>');
    }

    condivisioneReportDrawControl();
}

//Restituisce il contenuto del controllo per la condivisione del Report come oggetto Jquery.
function condivisioneReportGetContent() {
    return $('#select2CondivisioneReportListaUtenti');
}

// Elimina il contenuto del controllo per la condivisione del Report
function condivisioneReportClear() {
    var $listaUtenti = condivisioneReportGetContent();
    $listaUtenti.children().remove();
}

//Disegna il componente per la condivisione dei Report
function condivisioneReportDrawControl() {
    //Istruzione necessaria per il rendering del componente select2.
    //Il componente viene utilizzato per permettere di specificare gli utenti a cui va inviato un Report
    $("#select2CondivisioneReportListaUtenti").select2({
        //La seguente istruzione è necessaria perche' il componente si trova all'interno di una finestra di dialogo
        //Modal-overflow = Id finestra di dialogo (finestra che appare premendo il pulsante salva)
        dropdownParent: $("#Modal-overflow")
    });
}

function condivisioneReportGetUtentiSelezionati() {
    var result = new Array();
    if (checkBoxCondivisiIsChecked()) {
        result = $('#select2CondivisioneReportListaUtenti').val();
    }

    return result;
}

function checkBoxCondivisiIsChecked() {
    return $('#checkboxCondividi').is(":checked");
}

//Mostra o nasconde gli utenti per la condivisione del Report
function condivisioneReportShow(boolValue) {
    divCondisione = $('#divCondivisione');
    if (boolValue) {
        var $listaUtenti = condivisioneReportGetContent();
        if ($listaUtenti.children().length == 0) {
            condivisioneReportLoad();
        }
        divCondisione.show();
    }
    else
        divCondisione.hide();
}

function responsiveHeightResize() {
    kendo.resize($("#pivotgrid"));
}

function getTreeviewConfigurator() {
    return $('#configurator div[data-role="treeview"]').data("kendoTreeView");
}

function getConfigurator() {
    return $("#configurator").data("kendoPivotConfigurator");
}

// Restituisce true se il caricamento dei dati della pagina non e' andato a buon fine.
// Al momento restituisce true se nel configurator e' visibile il pulsante 
//  che permette di riprovare ad effettuare il caricamento del configurator.
function erroreCaricamentoDati() {
    return (
        $('button.k-request-retry').length > 0
        ||
        isPivotGridUndefined()
    );
}

//Restituisce true se la pivot grid contiene dei valori anomali.
function isPivotGridUndefined() {
    return isNullOrUndefined($("#pivotgrid").data("kendoPivotGrid"));
}

//Ricarica la pagina di Analysis.
//Se reportId non viene specificato si posiziona sull'ultimo filtro salvato
//altrimenti cerca di riposizionarsi sul Report specificato.
function reloadPage(reportId) {
    if (isNullOrUndefined(reportId)) {
        location.reload();
    }
    else {
        var url = "/Analysis/IndexReportId/" + reportId.toString();
        window.open(url, '_self');
    }
}

//Imposta le colonne di ricerca sulla pivot grid in modo da farle apparire su un'unica riga.
//Implementazione:
//  rimuove i ritorni a capo (<br />) aggiunti automaticamente dal componente kendo.
//La funzione viene chiamata dal file Analisys Index.cshtml, template per righe e colonne. 
function impostaColonneRicercaPivotGridSuUnaRiga() {
    $("div .k-pivot-toolbar.k-header.k-settings-columns.k-pivot-setting > br").remove();

    //Corregge l'altezza della cella altrimenti in presenza di campi apparirebbe molto alta. 
    //Aumenta la spaziatura tra le righe altrimenti in presenza di molti campi apparirebbero troppo vicini
    //Consente al contenuto della cella di andare a capo. Scavalca una regola css (style.css) che impedisce alle celle di andare a capo.
    $("div .k-pivot-toolbar.k-header.k-settings-columns.k-pivot-setting").css('line-height', '2em').css('white-space', 'normal'); //css('height', 'auto');//
}

//Cancella il datasource della pivot grid
function clearPivotGrid() {
    var grid = $("#pivotgrid").data("kendoPivotGrid");
    var newDataSource = new kendo.data.PivotDataSource({
        data: []
    });
    grid.setDataSource(newDataSource);
}

//Cancella la ricerca corrente mantenendo il report corrente
function cancellaRicercaOlap() {
    var data = new Object();
    data.Colonne = '[]';
    data.Righe = '[]';
    data.Misure = '[]';
    data.Filtri = '';
    data.Sort = '';

    data.ServerOlapUrl = serverOlapCorrente.getUrl();

    data.IdCategoria = cuboCorrente.getId();
    data.CubeName = cuboCorrente.getNome();
    data.NomeCategoria = cuboCorrente.getNomeFriendly();

    data.IdDatabase = databaseCorrente.getId();
    data.DbName = databaseCorrente.getNome();

    data.Id = reportCorrente.getId();
    data.Nome = reportCorrente.getNome();
    data.IdUtente = reportCorrente.getUtenteId();
    data.UtenteNome = reportCorrente.getUtenteNome();
    data.UtenteCognome = reportCorrente.getUtenteCognome();
    data.Condiviso = reportCorrente.isReportCondiviso();
   
    cambiaDataSourceImpostaDati(data);
}

function ridimensionaPivotGrid() {
    kendo.resize($("#pivotgrid"));
    //pivotgrid.refresh();

    //console.log('ridimensionaPivotGrid ', 'si');
    //$('div.k-settings-measures').css("height", "");
    //$('div.k-settings-columns').css("height", "");

    //ridimensionaPivotGridWithDelay();
}

function refreshPivotGrid() {
    //pivotgrid.refresh();
}

//function ridimensionaPivotGridWithDelay() {
//    setTimeout(function () {
//        ridimensionaPivotGrid();
//    }, 1000);
//}



